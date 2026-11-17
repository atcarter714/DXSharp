using Windows.Win32.Graphics.Direct3D12;
using Windows.Win32.Graphics.Dxgi.Common;
using DXSharp.Interop;

namespace DXSharp.Graphics;

/// <summary>Typed depth and stencil formats supported by depth targets.</summary>
public enum DepthStencilFormat {
    /// <summary>Sixteen normalized depth bits.</summary>
    D16Unorm = 55,
    /// <summary>Twenty-four normalized depth bits and eight stencil bits.</summary>
    D24UnormS8Uint = 45,
    /// <summary>Thirty-two floating-point depth bits.</summary>
    D32Float = 40,
    /// <summary>Thirty-two floating-point depth bits and eight stencil bits.</summary>
    D32FloatS8X24Uint = 20
}

/// <summary>The portions of a depth-stencil target to clear.</summary>
[Flags]
public enum DepthStencilClear {
    /// <summary>Clear depth.</summary>
    Depth = 1,
    /// <summary>Clear stencil.</summary>
    Stencil = 2,
    /// <summary>Clear depth and stencil together.</summary>
    DepthAndStencil = Depth | Stencil
}

/// <summary>An owning depth-stencil resource and its descriptor.</summary>
public sealed class DepthTarget : IDisposable {
    internal GpuResource Resource { get; }
    internal DescriptorHeap Heap { get; }
    /// <summary>The target's typed depth-stencil format.</summary>
    public DepthStencilFormat Format { get; }
    /// <summary>Width in pixels.</summary>
    public uint Width { get; }
    /// <summary>Height in pixels.</summary>
    public uint Height { get; }
    /// <summary>Whether this target includes an eight-bit stencil component.</summary>
    public bool HasStencil => Format is DepthStencilFormat.D24UnormS8Uint or DepthStencilFormat.D32FloatS8X24Uint;

    internal DepthTarget( GpuResource resource, DescriptorHeap heap, DepthStencilFormat format, uint width, uint height ) {
        Resource = resource; Heap = heap; Format = format; Width = width; Height = height;
    }

    /// <summary>Releases the public owners. Recorded command lists retain their own references.</summary>
    public void Dispose( ) {
        Heap.Dispose( );
        Resource.Dispose( );
    }
}

public sealed unsafe partial class GraphicsDevice {
    /// <summary>Creates a D32_FLOAT depth target in the depth-write state, with an optimized clear value of 1.</summary>
    public DepthTarget CreateDepthTarget( uint width, uint height ) =>
        CreateDepthTarget( width, height, DepthStencilFormat.D32Float );

    /// <summary>Creates a single-sample depth-stencil target in the depth-write state.</summary>
    /// <param name="width">Width in pixels, between 1 and 16384.</param>
    /// <param name="height">Height in pixels, between 1 and 16384.</param>
    /// <param name="format">A typed depth-stencil format supported by the device.</param>
    /// <param name="optimizedDepth">Optimized depth clear value between 0 and 1.</param>
    /// <param name="optimizedStencil">Optimized stencil clear value. Use zero with a depth-only format.</param>
    /// <returns>An independently owned resource and depth-stencil view.</returns>
    public DepthTarget CreateDepthTarget( uint width, uint height, DepthStencilFormat format,
        float optimizedDepth = 1, byte optimizedStencil = 0 ) {
        ArgumentOutOfRangeException.ThrowIfZero( width );
        ArgumentOutOfRangeException.ThrowIfZero( height );
        if ( width > 16384 || height > 16384 ) throw new ArgumentException( "Depth target dimensions exceed the D3D12 Texture2D limit." );
        if ( !Enum.IsDefined( format ) ) throw new ArgumentOutOfRangeException( nameof(format) );
        if ( !float.IsFinite( optimizedDepth ) || optimizedDepth < 0 || optimizedDepth > 1 )
            throw new ArgumentOutOfRangeException( nameof(optimizedDepth) );
        if ( optimizedStencil != 0 && format is DepthStencilFormat.D16Unorm or DepthStencilFormat.D32Float )
            throw new ArgumentException( "The selected format has no stencil component.", nameof(optimizedStencil) );

        D3D12_RESOURCE_DESC description = new( ) {
            Dimension = D3D12_RESOURCE_DIMENSION.D3D12_RESOURCE_DIMENSION_TEXTURE2D,
            Width = width, Height = height, DepthOrArraySize = 1, MipLevels = 1, Format = (DXGI_FORMAT)format,
            SampleDesc = new( ) { Count = 1 },
            Flags = D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_ALLOW_DEPTH_STENCIL | D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_DENY_SHADER_RESOURCE
        };
        D3D12_HEAP_PROPERTIES memory = new( ) { Type = D3D12_HEAP_TYPE.D3D12_HEAP_TYPE_DEFAULT, CreationNodeMask = 1, VisibleNodeMask = 1 };
        D3D12_CLEAR_VALUE clear = new( ) { Format = (DXGI_FORMAT)format };
        clear.Anonymous.DepthStencil.Depth = optimizedDepth;
        clear.Anonymous.DepthStencil.Stencil = optimizedStencil;
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
                return new( resource, heap, format, width, height );
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
