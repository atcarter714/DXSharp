using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using DXSharp.Interop;
using DXSharp.Shaders;
using DXSharp.MachineLearning;
using Windows.Win32.AI.MachineLearning.DirectML;
using Windows.Win32.Graphics.Direct3D.Dxc;

namespace InteropQualification;

[StructLayout( LayoutKind.Sequential )]
internal struct Pair { internal ulong First, Second; }

[GeneratedComInterface]
[Guid( "96446a17-3c17-451c-bc9d-5bb559180b54" )]
internal unsafe partial interface IFixture {
    [PreserveSig] int Echo( int value );
    [PreserveSig] void SetValue( int value );
    [PreserveSig] int GetValue( );
    [PreserveSig] unsafe int Fill( byte* output, uint count );
}

[GeneratedComInterface]
[Guid( "1659e483-98ab-4e6c-89ab-0f42f2702c71" )]
internal unsafe partial interface IDerived : IFixture {
    [PreserveSig] Pair ReadPair( );
}

[GeneratedComInterface]
[Guid( "a06f1049-f293-4285-bf27-2c66b35aef32" )]
internal partial interface IUnsupported { [PreserveSig] int Missing( ); }

[GeneratedComInterface]
[Guid( "afac9b8c-37e7-40b1-999b-6f65ada83a73" )]
internal partial interface ICallback { [PreserveSig] int Invoke( int value ); }

[GeneratedComClass]
internal partial class Callback : ICallback {
    public int Invoke( int value ) { GC.Collect( ); return value * 3; }
}

internal static partial class Native {
    [LibraryImport( "dxsharp_abi", EntryPoint = "fixture_create" )] internal static partial nint Create( );
    [LibraryImport( "dxsharp_abi", EntryPoint = "fixture_alive" )] internal static partial int Alive( );
    [LibraryImport( "dxsharp_abi", EntryPoint = "fixture_destroyed" )] internal static partial int Destroyed( );
    [LibraryImport( "dxsharp_abi", EntryPoint = "fixture_size" )] internal static partial int Size( int index );
    [LibraryImport( "dxsharp_abi", EntryPoint = "fixture_retain_callback" )] internal static partial void RetainCallback( nint value );
    [LibraryImport( "dxsharp_abi", EntryPoint = "fixture_invoke_callback" )] internal static partial int InvokeCallback( int value );
    [LibraryImport( "dxsharp_abi", EntryPoint = "fixture_include" )] internal static partial int Include( nint handler );
}

internal static class Program {
    private static int _checks;
    private static void Check( bool condition, string name ) { if ( !condition ) throw new InvalidOperationException( name ); ++_checks; }

    public static unsafe int Main( ) {
        try {
            uint[] dimensions = [1, 2, 3, 4];
            var shape = new TensorShape( dimensions ); dimensions[1] = 99;
            Check( shape.ElementCount == 24 && shape.SizeInBytes == 96 && shape.Dimensions[1] == 2, "immutable dense tensor layout" );
            bool rejected = false;
            try { _ = new TensorShape( 1, 0 ); } catch ( ArgumentOutOfRangeException ) { rejected = true; }
            Check( rejected, "zero tensor extent rejected" );
            rejected = false;
            try { _ = new TensorShape( uint.MaxValue, uint.MaxValue, uint.MaxValue ); } catch ( OverflowException ) { rejected = true; }
            Check( rejected, "tensor extent multiplication checked" );
            using ( var includes = new IncludeHandler( name => {
                Check( name == "fixture.hlsli", "UTF-16 include name" );
                GC.Collect( ); return System.Text.Encoding.UTF8.GetBytes( "generated include" );
            } ) ) {
                void* pointer = ComInterfaceMarshaller<IDxcIncludeHandler>.ConvertToUnmanaged( includes );
                try { Check( Native.Include( (nint)pointer ) == 1, "generated DXC include and blob callbacks" ); }
                finally { ComInterfaceMarshaller<IDxcIncludeHandler>.Free( pointer ); }
            }
            using ( var failingInclude = new IncludeHandler( _ => throw new FileNotFoundException( "Missing shader include." ) ) ) {
                void* pointer = ComInterfaceMarshaller<IDxcIncludeHandler>.ConvertToUnmanaged( failingInclude );
                try { Check( Native.Include( (nint)pointer ) == -1, "include failure translates to HRESULT" ); }
                finally { ComInterfaceMarshaller<IDxcIncludeHandler>.Free( pointer ); }
            }
            Check( Native.Size( 0 ) == Unsafe.SizeOf<Guid>( ), "GUID layout" );
            Check( Native.Size( 1 ) == Unsafe.SizeOf<Pair>( ), "structure layout" );
            Check( Native.Size( 2 ) == IntPtr.Size, "pointer width" );
            Check( Native.Size( 3 ) == Unsafe.SizeOf<DML_BUFFER_BINDING>( ), "DirectML buffer binding ABI" );
            Check( Native.Size( 4 ) == Unsafe.SizeOf<DML_BUFFER_TENSOR_DESC>( ), "DirectML tensor descriptor ABI" );
            using ( var root = ComOwner<IFixture>.Adopt( Native.Create( ) ) ) {
                Check( root.Interface.Echo( 42 ) == (42 ^ 0x13579), "scalar return and base slots" );
                root.Interface.SetValue( 77 );
                Check( root.Interface.GetValue( ) == 77, "void call and stored value" );
                Span<byte> bytes = stackalloc byte[257];
                fixed ( byte* pointer = bytes ) Check( root.Interface.Fill( pointer, (uint)bytes.Length ) == 0, "array call" );
                for ( int i = 0; i < bytes.Length; ++i ) Check( bytes[i] == (byte)(i ^ 0x5a), "array length and contents" );
                Check( root.Interface.Fill( null, 0 ) == 0, "empty input" );
                Check( root.TryQuery<IDerived>( typeof(IDerived).GUID, out var derived ), "derived QI" );
                using ( derived ) {
                    Pair pair = derived!.Interface.ReadPair( );
                    Check( pair.First == 0x1122334455667788UL && pair.Second == 0x8877665544332211UL, "C++ structure return ABI" );
                    Check( derived.Interface.Echo( 3 ) == (3 ^ 0x13579), "inherited slots" );
                }
                Check( !root.TryQuery<IUnsupported>( typeof(IUnsupported).GUID, out var missing ) && missing is null, "failed QI" );
                using var clone = root.Clone( );
                root.Dispose( );
                root.Dispose( );
                Check( clone.Interface.GetValue( ) == 77, "clone survives source disposal" );
                try { _ = root.Interface; throw new InvalidOperationException( "disposed access accepted" ); }
                catch ( ObjectDisposedException ) { ++_checks; }
            }
            Check( Native.Alive( ) == 0, "balanced deterministic ownership" );

            var owner = ComOwner<IFixture>.Adopt( Native.Create( ) );
            using ( var borrow = owner.Borrow( ) ) {
                owner.Dispose( );
                Check( Native.Alive( ) == 1, "borrow retains object during owner disposal" );
                Check( UnknownCalls.AddRef( borrow.Pointer ) > 0, "borrowed reference is callable" );
                UnknownCalls.Release( borrow.Pointer );
            }
            Check( Native.Alive( ) == 0, "borrow released" );
            try { using var invalid = ComOwner<IUnsupported>.Adopt( Native.Create( ) ); throw new InvalidOperationException( "invalid projection accepted" ); }
            catch ( InvalidCastException ) { ++_checks; }
            Check( Native.Alive( ) == 0, "failed projection cleans native output" );

            for ( int i = 0; i < 2000; ++i ) {
                using var item = ComOwner<IFixture>.Adopt( Native.Create( ) );
                using var copy = item.Clone( );
                if ( i % 100 == 0 ) { GC.Collect( ); GC.WaitForPendingFinalizers( ); }
                Check( copy.Interface.Echo( i ) == (i ^ 0x13579), "GC stress" );
            }
            Check( Native.Alive( ) == 0, "stress teardown" );
            ForgetOwner( );
            for ( int i = 0; i < 4; ++i ) { GC.Collect( ); GC.WaitForPendingFinalizers( ); }
            Check( Native.Alive( ) == 0, "finalizer fallback" );

            RetainCallback( );
            GC.Collect( ); GC.WaitForPendingFinalizers( );
            Check( Native.InvokeCallback( 17 ) == 51, "native-held generated callback survives GC" );
            Native.RetainCallback( 0 );
            Check( Native.Destroyed( ) >= 2004, "native destructor accounting" );
            if ( OperatingSystem.IsWindows( ) ) MeshQualification.Run( );
            Console.WriteLine( $"PASS: {_checks} checks; {Native.Destroyed( )} native objects destroyed; no live objects." );
            return 0;
        } catch ( Exception exception ) { Console.Error.WriteLine( exception ); return 1; }
    }

    [MethodImpl( MethodImplOptions.NoInlining )]
    private static void ForgetOwner( ) { _ = ComOwner<IFixture>.Adopt( Native.Create( ) ); }

    [MethodImpl( MethodImplOptions.NoInlining )]
    private static void RetainCallback( ) {
        var callback = new Callback( );
        var wrappers = new StrategyBasedComWrappers( );
        nint unknown = wrappers.GetOrCreateComInterfaceForObject( callback, CreateComInterfaceFlags.None );
        try {
            ResultsCheck( UnknownCalls.QueryInterface( unknown, typeof(ICallback).GUID, out nint pointer ) );
            try { Native.RetainCallback( pointer ); }
            finally { UnknownCalls.Release( pointer ); }
        } finally { UnknownCalls.Release( unknown ); }
        GC.KeepAlive( callback );
    }
    private static void ResultsCheck( int value ) { if ( value < 0 ) throw new COMException( "Callback QI", value ); }
}
