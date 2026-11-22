using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text.Json;

if ( args.Length == 1 ) {
    using var stream = File.OpenRead( args[0] );
    using var pe = new PEReader( stream );
    var reader = pe.GetMetadataReader( );
    foreach ( string ns in reader.TypeDefinitions.Select( handle => reader.GetString( reader.GetTypeDefinition( handle ).Namespace ) )
        .Where( value => value.StartsWith( "Windows.Win32.", StringComparison.Ordinal ) ).Distinct( ).Order( StringComparer.Ordinal ) )
        Console.WriteLine( ns );
    return 0;
}

if ( args.Length is < 2 or > 3 ) {
    Console.Error.WriteLine( "Usage: ApiCoverage <metadata.winmd[;other.winmd]> <DXSharp.Native.dll[;other.dll]> [report.json]" );
    return 2;
}

var sourceTypes = new Dictionary<string, ApiType>( StringComparer.Ordinal );
string[] references = args[0].Split( ';', StringSplitOptions.RemoveEmptyEntries );
foreach ( string file in references ) {
    using var metadataStream = File.OpenRead( file );
    using var metadataPe = new PEReader( metadataStream );
    foreach ( var type in ReadTypes( metadataPe.GetMetadataReader( ), false ) ) {
        if ( !type.Value.Namespace.StartsWith( "Windows.Win32.", StringComparison.Ordinal ) ) continue;
        if ( !sourceTypes.TryAdd( type.Key, type.Value ) )
            throw new InvalidDataException( $"Overlapping metadata definition: {type.Key}. Select one authoritative source for each API family." );
    }
}
var builtTypes = new Dictionary<string, ApiType>( StringComparer.Ordinal );
foreach ( string file in args[1].Split( ';', StringSplitOptions.RemoveEmptyEntries ) ) {
    using var assemblyStream = File.OpenRead( file );
    using var assemblyPe = new PEReader( assemblyStream );
    foreach ( var type in ReadTypes( assemblyPe.GetMetadataReader( ), true ) )
        builtTypes.TryAdd( type.Key, type.Value );
}

string[] families = [
    "Windows.Win32.Graphics.Direct3D12",
    "Windows.Win32.Graphics.Dxgi",
    "Windows.Win32.Graphics.Direct3D",
    "Windows.Win32.Graphics.Direct3D.Dxc",
    "Windows.Win32.Graphics.Direct3D.Fxc",
    "Windows.Win32.Graphics.Direct3D11on12",
    "Windows.Win32.Graphics.Direct3D9on12",
    "Windows.Win32.AI.MachineLearning.DirectML",
    "Windows.Win32.Graphics.Direct3D11",
    "Windows.Win32.Graphics.Direct3D9",
    "Windows.Win32.Graphics.Direct3D10",
    "Windows.Win32.Graphics.Direct2D",
    "Windows.Win32.Graphics.DirectWrite",
    "Windows.Win32.Graphics.Imaging",
    "Windows.Win32.Media.Audio",
    "Windows.Win32.UI.Input",
    "Windows.Win32.Devices.HumanInterfaceDevice",
    "Windows.Win32.UI.Animation",
    "Windows.Win32.Graphics.DirectComposition",
    "Windows.Win32.UI.Input.XboxController",
    "Windows.Win32.UI.Input.GameInput",
    "Windows.Win32.Storage.DirectStorage"
];

var exportedFunctions = new HashSet<string>( StringComparer.Ordinal );
foreach ( var type in builtTypes.Values.Where( value => (value.Name.EndsWith( ".PInvoke", StringComparison.Ordinal ) || value.Name.EndsWith( ".WindowsApis", StringComparison.Ordinal )) ) )
    exportedFunctions.UnionWith( type.Members );
foreach ( var type in builtTypes.Values ) exportedFunctions.UnionWith( type.Constants );

var reports = new List<FamilyReport>( );
foreach ( string family in families ) {
    bool recursive = family is not "Windows.Win32.UI.Input" and not "Windows.Win32.Graphics.Direct3D";
    var types = sourceTypes.Values.Where( value => value.Namespace == family ||
        (recursive && value.Namespace.StartsWith( family + ".", StringComparison.Ordinal )) ).ToArray( );
    var missingTypes = new List<string>( );
    var missingMembers = new List<string>( );
    var iidMismatches = new List<string>( );
    var expectedIids = new Dictionary<string, string>( StringComparer.Ordinal );
    int expectedTypes = 0, availableTypes = 0, expectedMembers = 0, availableMembers = 0;

    foreach ( var type in types ) {
        bool api = type.Name.EndsWith( ".Apis", StringComparison.Ordinal );
        var expected = type.Members;
        HashSet<string>? actual;
        if ( api ) {
            actual = exportedFunctions;
        } else {
            expectedTypes++;
            if ( builtTypes.TryGetValue( type.Name, out var implementation ) ) {
                availableTypes++;
                actual = implementation.Members;
                if ( type.Iid is Guid iid ) {
                    expectedIids.Add( type.Name, iid.ToString( "D" ) );
                    if ( implementation.Iid != iid ) iidMismatches.Add( type.Name );
                }
            } else {
                missingTypes.Add( type.Name );
                actual = null;
            }
        }

        foreach ( string member in expected ) {
            // Callback delegate Invoke signatures become function-pointer wrappers.
            if ( type.IsDelegate && member == "Invoke" && actual is not null ) continue;
            expectedMembers++;
            if ( actual?.Contains( member ) == true ) availableMembers++;
            else missingMembers.Add( type.Name + "." + member );
        }
    }

    var report = new FamilyReport( family, expectedTypes, availableTypes, expectedMembers, availableMembers,
        missingTypes.Order( StringComparer.Ordinal ).ToArray( ),
        missingMembers.Order( StringComparer.Ordinal ).ToArray( ),
        family != "Windows.Win32.Storage.DirectStorage",
        expectedTypes == 0 && expectedMembers == 0 ? "MissingReference" :
            missingTypes.Count == 0 && missingMembers.Count == 0 && iidMismatches.Count == 0 ? "NameAndIidCoverageComplete" : "Incomplete",
        references.Select( file => Path.GetFileName( file )! ).ToArray( ), expectedIids,
        iidMismatches.Order( StringComparer.Ordinal ).ToArray( ) );
    reports.Add( report );
    Console.WriteLine( expectedTypes == 0 && expectedMembers == 0
        ? $"{family}: reference metadata unavailable ({(report.Required ? "required; gate failed" : "unsupported; not qualified")})"
        : $"{family}: types {availableTypes}/{expectedTypes}, member names {availableMembers}/{expectedMembers}, IIDs {expectedIids.Count - iidMismatches.Count}/{expectedIids.Count}" );
}

var options = new JsonSerializerOptions { WriteIndented = true };
if ( args.Length == 3 ) File.WriteAllText( args[2], JsonSerializer.Serialize( reports, options ) + Environment.NewLine );
return reports.Any( report => report.Required && report.Status != "NameAndIidCoverageComplete" ) ? 1 : 0;

static Dictionary<string, ApiType> ReadTypes( MetadataReader reader, bool publicOnly ) {
    var result = new Dictionary<string, ApiType>( StringComparer.Ordinal );
    foreach ( var handle in reader.TypeDefinitions ) {
        var type = reader.GetTypeDefinition( handle );
        if ( !type.GetDeclaringType( ).IsNil ) continue;
        if ( publicOnly && (type.Attributes & TypeAttributes.VisibilityMask) != TypeAttributes.Public ) continue;
        string ns = reader.GetString( type.Namespace );
        string name = reader.GetString( type.Name );
        var members = new HashSet<string>( StringComparer.Ordinal );
        foreach ( var methodHandle in type.GetMethods( ) ) {
            var method = reader.GetMethodDefinition( methodHandle );
            if ( publicOnly && (method.Attributes & MethodAttributes.MemberAccessMask) != MethodAttributes.Public ) continue;
            string member = reader.GetString( method.Name );
            if ( (method.Attributes & MethodAttributes.PinvokeImpl) != 0 )
                members.Add( reader.GetString( method.GetImport( ).Name ) );
            if ( !member.StartsWith( ".", StringComparison.Ordinal ) ) members.Add( member );
        }
        foreach ( var propertyHandle in type.GetProperties( ) ) {
            var property = reader.GetPropertyDefinition( propertyHandle );
            var accessors = property.GetAccessors( );
            bool publicAccessor = new[] { accessors.Getter, accessors.Setter }.Any( accessor =>
                !accessor.IsNil && (reader.GetMethodDefinition( accessor ).Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public );
            if ( !publicOnly || publicAccessor ) members.Add( reader.GetString( property.Name ) );
        }
        var constants = new HashSet<string>( StringComparer.Ordinal );
        foreach ( var fieldHandle in type.GetFields( ) ) {
            var field = reader.GetFieldDefinition( fieldHandle );
            if ( publicOnly && (field.Attributes & FieldAttributes.FieldAccessMask) != FieldAttributes.Public ) continue;
            string member = reader.GetString( field.Name );
            if ( member != "value__" ) members.Add( member );
            if ( (field.Attributes & FieldAttributes.Static) != 0 ) constants.Add( member );
        }
        string full = ns + "." + name;
        bool isDelegate = false;
        if ( type.BaseType.Kind == HandleKind.TypeReference )
            isDelegate = reader.GetString( reader.GetTypeReference( (TypeReferenceHandle)type.BaseType ).Name ) == "MulticastDelegate";
        result[full] = new( full, ns, members, constants, isDelegate, ReadIid( reader, type, requireInterface: !publicOnly ) );
    }
    return result;
}

static Guid? ReadIid( MetadataReader reader, TypeDefinition type, bool requireInterface ) {
    // Native interfaces can project as pointer structs; source coclass GUIDs are not IIDs.
    if ( requireInterface && (type.Attributes & TypeAttributes.Interface) == 0 ) return null;
    foreach ( var handle in type.GetCustomAttributes( ) ) {
        var attribute = reader.GetCustomAttribute( handle );
        EntityHandle parent = attribute.Constructor.Kind switch {
            HandleKind.MemberReference => reader.GetMemberReference( (MemberReferenceHandle)attribute.Constructor ).Parent,
            HandleKind.MethodDefinition => reader.GetMethodDefinition( (MethodDefinitionHandle)attribute.Constructor ).GetDeclaringType( ),
            _ => default
        };
        string? name = parent.Kind switch {
            HandleKind.TypeReference => reader.GetString( reader.GetTypeReference( (TypeReferenceHandle)parent ).Name ),
            HandleKind.TypeDefinition => reader.GetString( reader.GetTypeDefinition( (TypeDefinitionHandle)parent ).Name ),
            _ => null
        };
        if ( name != "GuidAttribute" ) continue;
        var blob = reader.GetBlobReader( attribute.Value );
        if ( blob.ReadUInt16( ) != 1 ) throw new InvalidDataException( "Invalid GUID attribute prolog." );
        // WinMD may encode the native GUID fields; CLR GuidAttribute stores a string.
        if ( blob.RemainingBytes == 18 ) return new Guid( blob.ReadInt32( ), blob.ReadInt16( ), blob.ReadInt16( ),
            blob.ReadByte( ), blob.ReadByte( ), blob.ReadByte( ), blob.ReadByte( ),
            blob.ReadByte( ), blob.ReadByte( ), blob.ReadByte( ), blob.ReadByte( ) );
        return Guid.Parse( blob.ReadSerializedString( ) ?? throw new InvalidDataException( "Missing GUID attribute value." ) );
    }
    return null;
}

internal sealed record ApiType( string Name, string Namespace, HashSet<string> Members, HashSet<string> Constants, bool IsDelegate, Guid? Iid );
internal sealed record FamilyReport( string Namespace, int ExpectedTypes, int PublicTypes,
    int ExpectedMemberNames, int PublicMemberNames, string[] MissingTypes, string[] MissingMemberNames,
    bool Required, string Status, string[] ReferenceFiles, Dictionary<string, string> ExpectedIids, string[] IidMismatches );
