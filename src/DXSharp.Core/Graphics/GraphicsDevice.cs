using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct3D;
using Windows.Win32.Graphics.Direct3D12;
using Windows.Win32.Graphics.Dxgi;
using DXSharp.Interop;

namespace DXSharp.Graphics;

/// <summary>A Direct3D 12 device backed entirely by generated COM interfaces.</summary>
public sealed unsafe partial class GraphicsDevice : GraphicsObject {
    internal readonly ComOwner<ID3D12Device> Native;
    private readonly DeviceOptions _options;
    /// <summary>The selected adapter description.</summary>
    public string AdapterName { get; }
    private GraphicsDevice( ComOwner<ID3D12Device> owner, DeviceOptions options, string adapter ) : base( owner ) {
        Native = owner; _options = options; AdapterName = adapter;
    }
    internal override ComReference Retain( ) => Native.Retain( );
    internal GraphicsDevice Clone( ) => new( Native.Clone( ), _options, AdapterName ) { DeviceIdentity = DeviceIdentity };

    /// <summary>Creates a device without changing global Agility configuration.</summary>
    public static GraphicsDevice Create( DeviceOptions? options = null ) {
        NativeRuntime.Initialize( );
        options ??= new( );
        Guid factoryId = typeof(IDXGIFactory7).GUID;
        Results.Check( PInvoke.CreateDXGIFactory2( 0, &factoryId, out nint factoryPointer ), "CreateDXGIFactory2" );
        using var factory = ComOwner<IDXGIFactory7>.Adopt( factoryPointer );
        nint adapterPointer;
        if ( options.UseWarp ) {
            Guid adapterId = typeof(IDXGIAdapter4).GUID;
            Results.Check( factory.Interface.EnumWarpAdapter( &adapterId, out adapterPointer ), "EnumWarpAdapter" );
        } else {
            Guid adapterId = typeof(IDXGIAdapter4).GUID;
            Results.Check( factory.Interface.EnumAdapterByGpuPreference( 0, DXGI_GPU_PREFERENCE.DXGI_GPU_PREFERENCE_HIGH_PERFORMANCE,
                &adapterId, out adapterPointer ), "EnumAdapterByGpuPreference" );
        }
        using var adapter = ComOwner<IDXGIAdapter4>.Adopt( adapterPointer );
        DXGI_ADAPTER_DESC3 description;
        Results.Check( adapter.Interface.GetDesc3( &description ), "GetDesc3" );
        string name = description.Description.ToString( );
        Guid deviceId = typeof(ID3D12Device).GUID;
        nint devicePointer = 0;
        if ( options.UseAgility ) {
            Guid configurationClass = new( "7cda6aca-a03e-49c8-9458-0334d20e07ce" );
            Guid configurationId = typeof(ID3D12SDKConfiguration1).GUID;
            Results.Check( PInvoke.D3D12GetInterface( &configurationClass, &configurationId, out nint configurationPointer ), "D3D12GetInterface(SDKConfiguration1)" );
            using var configuration = ComOwner<ID3D12SDKConfiguration1>.Adopt( configurationPointer );
            Guid deviceFactoryId = typeof(ID3D12DeviceFactory).GUID;
            string sdkPath = Path.Combine( AppContext.BaseDirectory, "D3D12" ) + Path.DirectorySeparatorChar;
            using var path = new Utf8String( sdkPath );
            Results.Check( configuration.Interface.CreateDeviceFactory( 620, new PCSTR( path.Pointer ), &deviceFactoryId, out nint deviceFactoryPointer ), "CreateDeviceFactory(Agility 620)" );
            using var deviceFactory = ComOwner<ID3D12DeviceFactory>.Adopt( deviceFactoryPointer );
            if ( options.EnableDebugLayer ) {
                Guid debugClass = new( "f2352aeb-dd84-49fe-b97b-a9dcfdcc1b4f" ), debugId = typeof(ID3D12Debug).GUID;
                Results.Check( deviceFactory.Interface.GetConfigurationInterface( &debugClass, &debugId, out nint debugPointer ), "GetConfigurationInterface(Debug)" );
                using var debug = ComOwner<ID3D12Debug>.Adopt( debugPointer );
                debug.Interface.EnableDebugLayer( );
            }
            Results.Check( deviceFactory.Interface.CreateDevice( adapter.Interface, D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0, &deviceId, (void**)&devicePointer ), "DeviceFactory.CreateDevice" );
        } else {
            if ( options.EnableDebugLayer ) {
                Guid debugId = typeof(ID3D12Debug).GUID;
                Results.Check( PInvoke.D3D12GetDebugInterface( &debugId, out nint debugPointer ), "D3D12GetDebugInterface" );
                using var debug = ComOwner<ID3D12Debug>.Adopt( debugPointer );
                debug.Interface.EnableDebugLayer( );
            }
            Results.Check( PInvoke.D3D12CreateDevice( adapter.Interface, D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0, &deviceId, out devicePointer ), "D3D12CreateDevice" );
        }
        return new( ComOwner<ID3D12Device>.Adopt( devicePointer ), options, name );
    }

    /// <summary>Queries the highest Shader Model supported by this adapter and runtime, up to 6.10.</summary>
    /// <returns>The reported major and minor version. Shader compilation is a separate capability.</returns>
    public Version GetHighestShaderModel( ) {
        const int invalidArgument = unchecked((int)0x80070057);
        for ( int model = (int)D3D_SHADER_MODEL.D3D_SHADER_MODEL_6_10; model >= (int)D3D_SHADER_MODEL.D3D_SHADER_MODEL_5_1;
              model = model == (int)D3D_SHADER_MODEL.D3D_SHADER_MODEL_6_0 ? (int)D3D_SHADER_MODEL.D3D_SHADER_MODEL_5_1 : model - 1 ) {
            var data = new D3D12_FEATURE_DATA_SHADER_MODEL { HighestShaderModel = (D3D_SHADER_MODEL)model };
            int result = Native.Interface.CheckFeatureSupport( D3D12_FEATURE.D3D12_FEATURE_SHADER_MODEL, &data,
                (uint)sizeof(D3D12_FEATURE_DATA_SHADER_MODEL) );
            if ( result == invalidArgument ) continue;
            Results.Check( result, "CheckFeatureSupport(SHADER_MODEL)" );
            int reported = (int)data.HighestShaderModel;
            return new( reported >> 4, reported & 0xF );
        }
        throw new GraphicsException( "CheckFeatureSupport(SHADER_MODEL)", invalidArgument );
    }

    /// <summary>Creates a command allocator. Reset it only after all submissions using it complete.</summary>
    public CommandAllocator CreateCommandAllocator( QueueKind kind = QueueKind.Direct ) {
        Guid id = typeof(ID3D12CommandAllocator).GUID;
        Results.Check( Native.Interface.CreateCommandAllocator( (D3D12_COMMAND_LIST_TYPE)kind, &id, out nint pointer ), "CreateCommandAllocator" );
        return new( ComOwner<ID3D12CommandAllocator>.Adopt( pointer ), kind ) { DeviceIdentity = DeviceIdentity };
    }
    /// <summary>Creates an initially recording command list.</summary>
    public GraphicsCommandList CreateCommandList( CommandAllocator allocator ) {
        RequireSameDevice( allocator );
        Guid id = typeof(ID3D12GraphicsCommandList).GUID;
        Results.Check( Native.Interface.CreateCommandList( 0, (D3D12_COMMAND_LIST_TYPE)allocator.Kind, allocator.Native.Interface, null!, &id, out nint pointer ), "CreateCommandList" );
        return new( ComOwner<ID3D12GraphicsCommandList>.Adopt( pointer ), allocator ) { DeviceIdentity = DeviceIdentity };
    }
    /// <summary>Creates a queue with a dedicated completion fence.</summary>
    public CommandQueue CreateCommandQueue( QueueKind kind = QueueKind.Direct ) {
        D3D12_COMMAND_QUEUE_DESC description = new( ) { Type = (D3D12_COMMAND_LIST_TYPE)kind };
        Guid id = typeof(ID3D12CommandQueue).GUID;
        Results.Check( Native.Interface.CreateCommandQueue( &description, &id, out nint pointer ), "CreateCommandQueue" );
        var owner = ComOwner<ID3D12CommandQueue>.Adopt( pointer );
        try {
            Guid fenceId = typeof(ID3D12Fence).GUID;
            Results.Check( Native.Interface.CreateFence( 0, 0, &fenceId, out nint fencePointer ), "CreateFence" );
            return new( owner, ComOwner<ID3D12Fence>.Adopt( fencePointer ), this, kind ) { DeviceIdentity = DeviceIdentity };
        } catch { owner.Dispose( ); throw; }
    }

    /// <summary>Returns a bounded snapshot of the device's validation messages and removal reason.</summary>
    public GraphicsDiagnostics GetDiagnostics( ) {
        var messages = new List<string>( );
        if ( Native.TryQuery<ID3D12InfoQueue>( typeof(ID3D12InfoQueue).GUID, out var owner ) ) {
            using ( owner ) {
                ulong count = owner!.Interface.GetNumStoredMessagesAllowedByRetrievalFilter( );
                for ( ulong i = count > 128 ? count - 128 : 0; i < count; ++i ) {
                    nuint length = 0;
                    Results.Check( owner.Interface.GetMessage( i, null, ref length ), "InfoQueue.GetMessage(size)" );
                    if ( length > 1024 * 1024 ) continue;
                    void* buffer = NativeMemory.Alloc( length );
                    try {
                        Results.Check( owner.Interface.GetMessage( i, (D3D12_MESSAGE*)buffer, ref length ), "InfoQueue.GetMessage" );
                        D3D12_MESSAGE* message = (D3D12_MESSAGE*)buffer;
                        messages.Add( $"{message->Severity}: {Marshal.PtrToStringUTF8( (nint)message->pDescription, checked((int)message->DescriptionByteLength) ).TrimEnd('\0')}" );
                    } finally { NativeMemory.Free( buffer ); }
                }
            }
        }
        return new( AdapterName, _options.UseAgility, Native.Interface.GetDeviceRemovedReason( ).Value, messages );
    }
}

internal sealed unsafe class Utf8String : IDisposable {
    internal byte* Pointer { get; private set; }
    internal Utf8String( string value ) => Pointer = (byte*)Marshal.StringToCoTaskMemUTF8( value );
    public void Dispose( ) { Marshal.FreeCoTaskMem( (nint)Pointer ); Pointer = null; }
}
