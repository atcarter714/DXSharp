using Windows.Win32.Graphics.Direct3D12;
using Windows.Win32.Graphics.Dxgi.Common;
using DXSharp.Interop;

namespace DXSharp.Graphics;

/// <summary>An owning D32 depth resource and its depth-stencil view.</summary>
public sealed class DepthTarget : IDisposable {
    internal GpuResource Resource { get; }
    internal DescriptorHeap Heap { get; }
    internal DepthTarget( GpuResource resource, DescriptorHeap heap ) { Resource = resource; Heap = heap; }

    /// <summary>Releases the public owners. Recorded command lists retain their own references.</summary>
    public void Dispose( ) {
        Heap.Dispose( );
        Resource.Dispose( );
    }
}

public sealed unsafe partial class GraphicsDevice {
    /// <summary>Creates a D32_FLOAT depth target in the depth-write state, with a clear value of 1.</summary>
    public DepthTarget CreateDepthTarget( uint width, uint height ) {
        ArgumentOutOfRangeException.ThrowIfZero( width );
        ArgumentOutOfRangeException.ThrowIfZero( height );
        if ( width > 16384 || height > 16384 ) throw new ArgumentException( "Depth target dimensions exceed the D3D12 Texture2D limit." );
        var format = DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT;
        D3D12_RESOURCE_DESC description = new( ) {
            Dimension = D3D12_RESOURCE_DIMENSION.D3D12_RESOURCE_DIMENSION_TEXTURE2D,
            Width = width, Height = height, DepthOrArraySize = 1, MipLevels = 1, Format = format,
            SampleDesc = new( ) { Count = 1 },
            Flags = D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_ALLOW_DEPTH_STENCIL | D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_DENY_SHADER_RESOURCE
        };
        D3D12_HEAP_PROPERTIES memory = new( ) { Type = D3D12_HEAP_TYPE.D3D12_HEAP_TYPE_DEFAULT, CreationNodeMask = 1, VisibleNodeMask = 1 };
        D3D12_CLEAR_VALUE clear = new( ) { Format = format };
        clear.Anonymous.DepthStencil.Depth = 1;
        Guid id = typeof(ID3D12Resource).GUID;
        nint pointer = 0;
        Results.Check( Native.Interface.CreateCommittedResource( &memory, 0, &description,
            D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_DEPTH_WRITE, &clear, &id, (void**)&pointer ), "CreateCommittedResource(Depth)" );
        var resource = new GpuResource( ComOwner<ID3D12Resource>.Adopt( pointer ), 0, BufferMemory.Default ) { DeviceIdentity = DeviceIdentity };
        try {
            var type = D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_DSV;
            D3D12_DESCRIPTOR_HEAP_DESC heapDescription = new( ) { Type = type, NumDescriptors = 1 };
            id = typeof(ID3D12DescriptorHeap).GUID;
            Results.Check( Native.Interface.CreateDescriptorHeap( &heapDescription, &id, out pointer ), "CreateDescriptorHeap(Depth)" );
            var heap = new DescriptorHeap( ComOwner<ID3D12DescriptorHeap>.Adopt( pointer ), 1,
                Native.Interface.GetDescriptorHandleIncrementSize( type ) ) { DeviceIdentity = DeviceIdentity };
            try {
                Native.Interface.CreateDepthStencilView( resource.Native.Interface, null, heap.CpuHandle( 0 ) );
                return new( resource, heap );
            } catch {
                heap.Dispose( );
                throw;
            }
        } catch {
            resource.Dispose( );
            throw;
        }
    }
}
