using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct3D12;
using Windows.Win32.Graphics.Dxgi;
using Windows.Win32.Graphics.Dxgi.Common;
using DXSharp.Interop;

namespace DXSharp.Graphics;

/// <summary>An owning flip-model HWND swap chain.</summary>
public sealed unsafe class SwapChain : GraphicsObject {
    private readonly ComOwner<IDXGISwapChain4> _native;
    /// <summary>The number of back buffers.</summary>
    public uint BufferCount { get; }
    /// <summary>The presentation format.</summary>
    public PixelFormat Format { get; }
    internal SwapChain( ComOwner<IDXGISwapChain4> owner, uint count, PixelFormat format ) : base( owner ) { _native = owner; BufferCount = count; Format = format; }
    internal override ComReference Retain( ) => _native.Retain( );
    /// <summary>The current back-buffer index.</summary>
    public uint CurrentBackBufferIndex => _native.Interface.GetCurrentBackBufferIndex( );
    /// <summary>Acquires an independently owned back-buffer resource. Dispose all such owners before resizing.</summary>
    public GpuResource AcquireBackBuffer( uint index ) {
        if ( index >= BufferCount ) throw new ArgumentOutOfRangeException( nameof(index) );
        Guid id = typeof(ID3D12Resource).GUID;
        Results.Check( _native.Interface.GetBuffer( index, &id, out nint pointer ), "SwapChain.GetBuffer" );
        return new( ComOwner<ID3D12Resource>.Adopt( pointer ), 0, BufferMemory.Default ) { DeviceIdentity = DeviceIdentity };
    }
    /// <summary>Presents, preserving non-failing statuses such as occlusion.</summary>
    public int Present( uint syncInterval = 1 ) {
        if ( syncInterval > 4 ) throw new ArgumentOutOfRangeException( nameof(syncInterval) );
        int hr = _native.Interface.Present( syncInterval, 0 ); Results.Check( hr, "SwapChain.Present" ); return hr;
    }
    /// <summary>Resizes after the caller drains the presentation queue with <see cref="CommandQueue.WaitIdle"/>
    /// and releases all back-buffer references.</summary>
    public void Resize( uint width, uint height ) {
        if ( width == 0 || height == 0 ) throw new ArgumentOutOfRangeException( nameof(width) );
        Results.Check( _native.Interface.ResizeBuffers( BufferCount, width, height, (DXGI_FORMAT)Format, 0 ), "SwapChain.ResizeBuffers" );
    }
}

public sealed unsafe partial class GraphicsDevice {
    /// <summary>Creates a swap chain for a direct command queue and a live HWND.</summary>
    public SwapChain CreateSwapChain( CommandQueue queue, nint window, uint width, uint height,
        PixelFormat format = PixelFormat.Rgba8Unorm, uint bufferCount = 2 ) {
        RequireSameDevice( queue );
        if ( queue.Kind != QueueKind.Direct ) throw new ArgumentException( "Presentation requires a direct queue." );
        if ( window == 0 ) throw new ArgumentNullException( nameof(window) );
        if ( width == 0 || height == 0 || bufferCount < 2 || bufferCount > 16 ) throw new ArgumentOutOfRangeException( nameof(width) );
        Guid id = typeof(IDXGIFactory7).GUID;
        Results.Check( PInvoke.CreateDXGIFactory2( 0, &id, out nint pointer ), "CreateDXGIFactory2" );
        using var factory = ComOwner<IDXGIFactory7>.Adopt( pointer );
        DXGI_SWAP_CHAIN_DESC1 description = new( ) {
            Width = width, Height = height, Format = (DXGI_FORMAT)format,
            SampleDesc = new( ) { Count = 1 }, BufferUsage = DXGI_USAGE.DXGI_USAGE_RENDER_TARGET_OUTPUT,
            BufferCount = bufferCount, SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_FLIP_DISCARD,
            Scaling = DXGI_SCALING.DXGI_SCALING_STRETCH, AlphaMode = DXGI_ALPHA_MODE.DXGI_ALPHA_MODE_IGNORE
        };
        factory.TryQuery<IFactory2Raw>( typeof(IFactory2Raw).GUID, out var rawFactory );
        using var raw = rawFactory ?? throw new NotSupportedException( "DXGI 1.2 is required." );
        using var queueLease = queue.Native.Borrow( );
        Results.Check( raw.Interface.CreateSwapChainForHwnd( queueLease.Pointer, new HWND( window ), description, null, 0, out nint swapPointer ), "CreateSwapChainForHwnd" );
        var swap = ComOwner<IDXGISwapChain4>.Adopt( swapPointer );
        try {
            Results.Check( factory.Interface.MakeWindowAssociation( new HWND( window ), DXGI_MWA_FLAGS.DXGI_MWA_NO_ALT_ENTER ), "MakeWindowAssociation" );
            return new( swap, bufferCount, format ) { DeviceIdentity = DeviceIdentity };
        } catch { swap.Dispose( ); throw; }
    }
}
