using Windows.Win32.Graphics.Direct3D12;
using Windows.Win32.Graphics.Dxgi.Common;
using DXSharp.Interop;

namespace DXSharp.Graphics;

/// <summary>An immutable RGBA8 texture and its shader-visible view.</summary>
public sealed class SampledTexture2D : IDisposable {
    internal GpuResource Resource { get; }
    internal DescriptorHeap Heap { get; }
    /// <summary>Texture width in pixels.</summary>
    public uint Width { get; }
    /// <summary>Texture height in pixels.</summary>
    public uint Height { get; }

    internal SampledTexture2D( GpuResource resource, DescriptorHeap heap, uint width, uint height ) {
        Resource = resource;
        Heap = heap;
        Width = width;
        Height = height;
    }

    /// <summary>Releases the public owners. Recorded command lists retain their own references.</summary>
    public void Dispose( ) {
        Heap.Dispose( );
        Resource.Dispose( );
    }
}

public sealed unsafe partial class GraphicsDevice {
    /// <summary>Uploads tightly packed, single-mip RGBA8 pixels and waits until they can be sampled by a pixel shader.
    /// Use a direct queue on this device. Row padding is supplied by the SDK.</summary>
    public SampledTexture2D CreateSampledTexture2D( CommandQueue queue, uint width, uint height, ReadOnlySpan<byte> rgba ) {
        RequireSameDevice( queue );
        if ( queue.Kind != QueueKind.Direct ) throw new ArgumentException( "Texture upload requires a direct queue.", nameof(queue) );
        using var allocator = CreateCommandAllocator( );
        using var commands = CreateCommandList( allocator );
        var texture = CreateSampledTexture2D( commands, width, height, rgba );
        try {
            commands.Close( );
            queue.Wait( queue.Submit( commands ) );
            return texture;
        } catch {
            texture.Dispose( );
            throw;
        }
    }

    /// <summary>Records a single-mip RGBA8 upload and transition to pixel shader resource state on a direct list.</summary>
    /// <param name="commands">The recording direct command list on this device.</param>
    /// <param name="width">Texture width in pixels.</param>
    /// <param name="height">Texture height in pixels.</param>
    /// <param name="rgba">Tightly packed RGBA pixels in row order.</param>
    /// <param name="srgb">Whether sampling converts the RGB channels from sRGB to linear light.</param>
    /// <returns>The texture and its view. The command list retains the upload through execution.</returns>
    public SampledTexture2D CreateSampledTexture2D( GraphicsCommandList commands, uint width, uint height,
        ReadOnlySpan<byte> rgba, bool srgb = false ) {
        RequireSameDevice( commands );
        commands.RequireDirectRecording( );
        ArgumentOutOfRangeException.ThrowIfZero( width );
        ArgumentOutOfRangeException.ThrowIfZero( height );
        if ( width > 16384 || height > 16384 ) throw new ArgumentException( "Texture dimensions exceed the D3D12 Texture2D limit." );
        int rowBytes = checked((int)width * 4);
        if ( rgba.Length != checked(rowBytes * (int)height) )
            throw new ArgumentException( "Supply exactly four bytes per pixel.", nameof(rgba) );

        D3D12_RESOURCE_DESC description = new( ) {
            Dimension = D3D12_RESOURCE_DIMENSION.D3D12_RESOURCE_DIMENSION_TEXTURE2D,
            Width = width, Height = height, DepthOrArraySize = 1, MipLevels = 1,
            Format = srgb ? DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM_SRGB : DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM,
            SampleDesc = new( ) { Count = 1 }
        };
        D3D12_PLACED_SUBRESOURCE_FOOTPRINT footprint;
        ulong bytes;
        Native.Interface.GetCopyableFootprints( &description, 0, 1, 0, &footprint, null, null, &bytes );
        D3D12_HEAP_PROPERTIES memory = new( ) { Type = D3D12_HEAP_TYPE.D3D12_HEAP_TYPE_DEFAULT, CreationNodeMask = 1, VisibleNodeMask = 1 };
        Guid id = typeof(ID3D12Resource).GUID;
        nint pointer = 0;
        Results.Check( Native.Interface.CreateCommittedResource( &memory, 0, &description,
            D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COPY_DEST, null, &id, (void**)&pointer ), "CreateCommittedResource(Texture2D)" );
        var texture = new GpuResource( ComOwner<ID3D12Resource>.Adopt( pointer ), 0, BufferMemory.Default ) { DeviceIdentity = DeviceIdentity };
        try {
            var type = D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_CBV_SRV_UAV;
            D3D12_DESCRIPTOR_HEAP_DESC heapDescription = new( ) {
                Type = type, NumDescriptors = 1, Flags = D3D12_DESCRIPTOR_HEAP_FLAGS.D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE
            };
            id = typeof(ID3D12DescriptorHeap).GUID;
            Results.Check( Native.Interface.CreateDescriptorHeap( &heapDescription, &id, out pointer ), "CreateDescriptorHeap(Texture2D)" );
            var heap = new DescriptorHeap( ComOwner<ID3D12DescriptorHeap>.Adopt( pointer ), 1,
                Native.Interface.GetDescriptorHandleIncrementSize( type ) ) { DeviceIdentity = DeviceIdentity };
            try {
                Native.Interface.CreateShaderResourceView( texture.Native.Interface, null, heap.CpuHandle( 0 ) );
                using var upload = CreateBuffer( bytes, BufferMemory.Upload );
                WriteTextureRows( upload, footprint, rgba, rowBytes, (int)height );
                commands.CopyTextureFromUpload( upload, texture, footprint );
                commands.Transition( texture, ResourceState.CopyDestination, ResourceState.PixelShaderResource );
                return new( texture, heap, width, height );
            } catch {
                heap.Dispose( );
                throw;
            }
        } catch {
            texture.Dispose( );
            throw;
        }
    }

    private static void WriteTextureRows( GpuResource upload, D3D12_PLACED_SUBRESOURCE_FOOTPRINT footprint,
        ReadOnlySpan<byte> rgba, int rowBytes, int rows ) {
        D3D12_RANGE read = default;
        void* mapping = null;
        Results.Check( upload.Native.Interface.Map( 0, &read, &mapping ), "Resource.Map(texture upload)" );
        ulong end = footprint.Offset + (ulong)(rows - 1) * footprint.Footprint.RowPitch + (uint)rowBytes;
        try {
            for ( int y = 0; y < rows; ++y ) {
                ulong offset = footprint.Offset + (ulong)y * footprint.Footprint.RowPitch;
                rgba.Slice( y * rowBytes, rowBytes ).CopyTo( new Span<byte>( (byte*)mapping + checked((nint)offset), rowBytes ) );
            }
        } finally {
            D3D12_RANGE written = new( ) { Begin = checked((nuint)footprint.Offset), End = checked((nuint)end) };
            upload.Native.Interface.Unmap( 0, &written );
        }
    }
}
