using DXSharp.Interop;
using Windows.Win32.Graphics.Direct3D12;
using Windows.Win32.Graphics.Dxgi.Common;

namespace DXSharp.Graphics;

/// <summary>A single-plane, single-sample color texture, optionally an array or cube array.</summary>
/// <param name="Width">Base mip width.</param>
/// <param name="Height">Base mip height.</param>
/// <param name="Format">A typed DXGI color format supported by the device.</param>
/// <param name="MipLevels">Number of supplied mip levels; no mip generation is performed.</param>
/// <param name="ArraySize">Number of array slices, or faces for cubes.</param>
/// <param name="Cube">Whether the SRV represents cubes; requires square faces and a multiple of six slices.</param>
public readonly record struct Texture2DDescription( uint Width, uint Height, DXGI_FORMAT Format, ushort MipLevels = 1, ushort ArraySize = 1, bool Cube = false ) {
    /// <summary>The number of mip/array subresources, ordered mip + arraySlice * MipLevels.</summary>
    public uint SubresourceCount => checked((uint)MipLevels * ArraySize);
    internal D3D12_RESOURCE_DESC NativeDescription( ) {
        if ( Width == 0 || Height == 0 || Width > 16384 || Height > 16384 ) throw new ArgumentOutOfRangeException( nameof(Width) );
        int maximumMips = 1 + System.Numerics.BitOperations.Log2( Math.Max( Width, Height ) );
        if ( MipLevels == 0 || MipLevels > maximumMips ) throw new ArgumentOutOfRangeException( nameof(MipLevels) );
        if ( ArraySize == 0 || ArraySize > 2048 ) throw new ArgumentOutOfRangeException( nameof(ArraySize) );
        if ( Cube && (Width != Height || ArraySize % 6 != 0) ) throw new ArgumentException( "Cubes require square faces and an array size divisible by six." );
        if ( !Enum.IsDefined( Format ) || Format == DXGI_FORMAT.DXGI_FORMAT_UNKNOWN || Format.ToString( ).Contains( "TYPELESS", StringComparison.Ordinal ) ||
             Format is DXGI_FORMAT.DXGI_FORMAT_D16_UNORM or DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT or DXGI_FORMAT.DXGI_FORMAT_D24_UNORM_S8_UINT or DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT_S8X24_UINT )
            throw new NotSupportedException( "Use a typed color format; depth and typeless views require a separate resource/view contract." );
        return new( ) { Dimension = D3D12_RESOURCE_DIMENSION.D3D12_RESOURCE_DIMENSION_TEXTURE2D, Width = Width, Height = Height,
            DepthOrArraySize = ArraySize, MipLevels = MipLevels, Format = Format, SampleDesc = new( ) { Count = 1 } };
    }
}

/// <summary>CPU bytes for one subresource. RowPitch includes any source row padding; compressed formats use rows of blocks.</summary>
/// <param name="Data">The bytes copied synchronously during upload recording.</param>
/// <param name="RowPitch">Source row stride in bytes.</param>
public readonly record struct TextureSubresourceData( ReadOnlyMemory<byte> Data, uint RowPitch );

/// <summary>A sampled texture with explicit mip/array layout and a pooled SRV.</summary>
/// <remarks>Initial state is CopyDestination. Callers track transitions and cross-queue ordering. No mip generation or file decoding is implicit.</remarks>
public sealed class Texture2D : GraphicsObject {
    internal readonly GpuResource Resource;
    internal readonly DescriptorAllocation View;
    internal readonly TextureTransferLayout Layout;
    /// <summary>The immutable resource and sampling description.</summary>
    public Texture2DDescription Description { get; }
    internal Texture2D( GpuResource resource, DescriptorAllocation view, Texture2DDescription description, TextureTransferLayout layout ) : base( resource ) {
        Resource = resource; View = view; Description = description; Layout = layout; DeviceIdentity = resource.DeviceIdentity;
    }
    internal override ComReference Retain( ) => Resource.Retain( );
    /// <summary>Releases public resource and descriptor owners. Recordings retain their own leases.</summary>
    public override void Dispose( ) { View.Dispose( ); base.Dispose( ); }
}

internal sealed class TextureTransferLayout {
    internal readonly D3D12_PLACED_SUBRESOURCE_FOOTPRINT[] Footprints;
    internal readonly uint[] Rows;
    internal readonly ulong[] RowBytes;
    internal readonly ulong Size;
    internal TextureTransferLayout( D3D12_PLACED_SUBRESOURCE_FOOTPRINT[] footprints, uint[] rows, ulong[] rowBytes, ulong size ) {
        Footprints = footprints; Rows = rows; RowBytes = rowBytes; Size = size;
    }
    internal TextureTransferLayout Select( uint first, uint count ) {
        if ( count == 0 || first >= Footprints.Length || count > Footprints.Length - first ) throw new ArgumentOutOfRangeException( nameof(count) );
        int start = checked((int)first), length = checked((int)count);
        var footprints = Footprints.AsSpan( start, length ).ToArray( );
        ulong origin = footprints[0].Offset;
        for ( int i = 0; i < length; ++i ) footprints[i].Offset -= origin;
        var rows = Rows.AsSpan( start, length ).ToArray( ); var bytes = RowBytes.AsSpan( start, length ).ToArray( );
        ulong size = checked(footprints[^1].Offset + (rows[^1] - 1UL) * footprints[^1].Footprint.RowPitch + bytes[^1]);
        return new( footprints, rows, bytes, size );
    }
    internal void ValidateData( ReadOnlySpan<TextureSubresourceData> data ) {
        if ( data.Length != Footprints.Length ) throw new ArgumentException( "Supply one data record for each selected subresource.", nameof(data) );
        for ( int i = 0; i < data.Length; ++i ) {
            if ( data[i].RowPitch < RowBytes[i] || RowBytes[i] > int.MaxValue ) throw new ArgumentException( "Source row pitch is smaller than the native row size.", nameof(data) );
            ulong required = checked((Rows[i] - 1UL) * data[i].RowPitch + RowBytes[i]);
            if ( required > (ulong)data[i].Data.Length ) throw new ArgumentException( "Subresource data is truncated.", nameof(data) );
        }
    }
}

/// <summary>An owning readback result with native row padding removed when copied to CPU memory.</summary>
public sealed unsafe class TextureReadback : IDisposable {
    internal readonly ReadbackAllocation Allocation;
    private readonly TextureTransferLayout _layout;
    /// <summary>The first source texture subresource represented by this result.</summary>
    public uint FirstSubresource { get; }
    /// <summary>The number of captured subresources.</summary>
    public uint SubresourceCount => (uint)_layout.Footprints.Length;
    internal TextureReadback( ReadbackAllocation allocation, TextureTransferLayout layout, uint first ) { Allocation = allocation; _layout = layout; FirstSubresource = first; }
    /// <summary>Gets tightly packed bytes required for one relative subresource.</summary>
    public int GetDataSize( uint subresource ) {
        if ( subresource >= SubresourceCount ) throw new ArgumentOutOfRangeException( nameof(subresource) );
        return checked((int)(_layout.RowBytes[subresource] * _layout.Rows[subresource]));
    }
    /// <summary>Copies a completed relative subresource into packed CPU rows. Reads before retirement throw.</summary>
    public void Read( uint subresource, Span<byte> destination ) {
        int size = GetDataSize( subresource );
        if ( destination.Length != size ) throw new ArgumentException( "Supply exactly GetDataSize bytes.", nameof(destination) );
        lock ( Allocation.Lifetime.Gate ) {
            Allocation.Lifetime.RequireIdle( );
            var footprint = _layout.Footprints[subresource]; int rowBytes = checked((int)_layout.RowBytes[subresource]);
            ulong begin = checked(Allocation.Offset + footprint.Offset);
            D3D12_RANGE read = new( ) { Begin = checked((nuint)begin), End = checked((nuint)(begin + (_layout.Rows[subresource] - 1UL) * footprint.Footprint.RowPitch + (uint)rowBytes)) };
            void* pointer = null;
            Results.Check( Allocation.Buffer.Native.Interface.Map( 0, &read, &pointer ), "Map(texture readback)" );
            try {
                for ( uint row = 0; row < _layout.Rows[subresource]; ++row )
                    new ReadOnlySpan<byte>( (byte*)pointer + checked((nint)(begin + row * (ulong)footprint.Footprint.RowPitch)), rowBytes )
                        .CopyTo( destination.Slice( checked((int)row * rowBytes), rowBytes ) );
            } finally { D3D12_RANGE written = default; Allocation.Buffer.Native.Interface.Unmap( 0, &written ); }
        }
    }
    /// <summary>Releases the CPU result owner; pending GPU writes retain their range.</summary>
    public void Dispose( ) => Allocation.Dispose( );
}

public sealed unsafe partial class GraphicsDevice {
    /// <summary>Creates a typed sampled texture in CopyDestination state, with supplied mip/array counts and no initialized data.</summary>
    public Texture2D CreateTexture2D( Texture2DDescription description, DescriptorPool descriptors ) {
        ArgumentNullException.ThrowIfNull( descriptors ); RequireSameDevice( descriptors.Heap );
        if ( descriptors.Kind != DescriptorKind.Resource || !descriptors.ShaderVisible ) throw new ArgumentException( "Use a shader-visible resource descriptor pool.", nameof(descriptors) );
        var native = description.NativeDescription( );
        D3D12_FEATURE_DATA_FORMAT_INFO info = new( ) { Format = description.Format };
        Results.Check( Native.Interface.CheckFeatureSupport( D3D12_FEATURE.D3D12_FEATURE_FORMAT_INFO, &info, (uint)sizeof(D3D12_FEATURE_DATA_FORMAT_INFO) ), "CheckFeatureSupport(format planes)" );
        if ( info.PlaneCount != 1 ) throw new NotSupportedException( "This texture transfer contract supports single-plane color formats." );
        D3D12_FEATURE_DATA_FORMAT_SUPPORT support = new( ) { Format = description.Format };
        Results.Check( Native.Interface.CheckFeatureSupport( D3D12_FEATURE.D3D12_FEATURE_FORMAT_SUPPORT, &support, (uint)sizeof(D3D12_FEATURE_DATA_FORMAT_SUPPORT) ), "CheckFeatureSupport(texture format)" );
        if ( (support.Support1 & D3D12_FORMAT_SUPPORT1.D3D12_FORMAT_SUPPORT1_TEXTURE2D) == 0 ||
             (support.Support1 & (D3D12_FORMAT_SUPPORT1.D3D12_FORMAT_SUPPORT1_SHADER_LOAD | D3D12_FORMAT_SUPPORT1.D3D12_FORMAT_SUPPORT1_SHADER_SAMPLE)) == 0 )
            throw new NotSupportedException( "The device cannot use this format as a sampled Texture2D." );
        if ( description.Cube && (support.Support1 & D3D12_FORMAT_SUPPORT1.D3D12_FORMAT_SUPPORT1_TEXTURECUBE) == 0 )
            throw new NotSupportedException( "The device cannot sample this format as a cube." );
        int count = checked((int)description.SubresourceCount);
        var footprints = new D3D12_PLACED_SUBRESOURCE_FOOTPRINT[count]; var rows = new uint[count]; var bytes = new ulong[count]; ulong size;
        fixed ( D3D12_PLACED_SUBRESOURCE_FOOTPRINT* f = footprints ) fixed ( uint* r = rows ) fixed ( ulong* b = bytes )
            Native.Interface.GetCopyableFootprints( &native, 0, (uint)count, 0, f, r, b, &size );
        if ( size == ulong.MaxValue || size == 0 ) throw new NotSupportedException( "The resource has no valid copy layout." );
        using var resourceCleanup = new ResourceCreation( );
        var memory = new D3D12_HEAP_PROPERTIES { Type = D3D12_HEAP_TYPE.D3D12_HEAP_TYPE_DEFAULT, CreationNodeMask = 1, VisibleNodeMask = 1 };
        var resource = CreateCommittedResource( native, memory, ResourceState.CopyDestination ); resourceCleanup.Resource = resource;
        var view = descriptors.Allocate( );
        try {
            D3D12_SHADER_RESOURCE_VIEW_DESC? srv = null;
            if ( description.Cube ) {
                var cube = new D3D12_SHADER_RESOURCE_VIEW_DESC { Format = description.Format, Shader4ComponentMapping = 5768,
                    ViewDimension = description.ArraySize == 6 ? D3D12_SRV_DIMENSION.D3D12_SRV_DIMENSION_TEXTURECUBE : D3D12_SRV_DIMENSION.D3D12_SRV_DIMENSION_TEXTURECUBEARRAY };
                if ( description.ArraySize == 6 ) cube.Anonymous.TextureCube.MipLevels = description.MipLevels;
                else { cube.Anonymous.TextureCubeArray.MipLevels = description.MipLevels; cube.Anonymous.TextureCubeArray.NumCubes = (uint)description.ArraySize / 6; }
                srv = cube;
            }
            CreateShaderResourceView( resource, view, description: srv );
            var result = new Texture2D( resource, view, description, new( footprints, rows, bytes, size ) );
            resourceCleanup.Resource = null; return result;
        } catch { view.Dispose( ); throw; }
    }
    private sealed class ResourceCreation : IDisposable {
        internal GpuResource? Resource;
        public void Dispose( ) => Resource?.Dispose( );
    }
}

public sealed unsafe partial class GraphicsCommandList {
    /// <summary>Transitions all mip/array subresources. Applications supply the actual before/after states.</summary>
    public void Transition( Texture2D texture, ResourceState before, ResourceState after ) { RequireSameDevice( texture ); Transition( texture.Resource, before, after ); }
    /// <summary>Copies supplied mip/array data into a texture already in CopyDestination state. No transition is implicit.</summary>
    public void UploadTexture( Texture2D texture, UploadRing uploads, ReadOnlySpan<TextureSubresourceData> data, uint firstSubresource = 0 ) {
        Recording( ); RequireSameDevice( texture ); ArgumentNullException.ThrowIfNull( uploads ); RequireSameDevice( uploads.Buffer );
        var layout = texture.Layout.Select( firstSubresource, checked((uint)data.Length) ); layout.ValidateData( data );
        using var slice = uploads.Allocate( layout.Size, UploadRing.TexturePlacementAlignment );
        lock ( slice.Lifetime.Gate ) {
            slice.Lifetime.RequireWritable( ); D3D12_RANGE read = default; void* pointer = null;
            Results.Check( slice.Buffer.Native.Interface.Map( 0, &read, &pointer ), "Map(texture subresources)" );
            try {
                for ( int i = 0; i < data.Length; ++i ) for ( uint row = 0; row < layout.Rows[i]; ++row ) {
                    ulong offset = checked(slice.Offset + layout.Footprints[i].Offset + row * (ulong)layout.Footprints[i].Footprint.RowPitch);
                    data[i].Data.Span.Slice( checked((int)((ulong)row * data[i].RowPitch)), checked((int)layout.RowBytes[i]) )
                        .CopyTo( new Span<byte>( (byte*)pointer + checked((nint)offset), checked((int)layout.RowBytes[i]) ) );
                }
            } finally {
                D3D12_RANGE written = new( ) { Begin = checked((nuint)slice.Offset), End = checked((nuint)(slice.Offset + layout.Size)) };
                slice.Buffer.Native.Interface.Unmap( 0, &written );
            }
        }
        RetainAllocation( slice ); Track( texture.Resource );
        for ( int i = 0; i < data.Length; ++i ) CopyTextureSlice( texture, firstSubresource + (uint)i, slice.Buffer, layout.Footprints[i], slice.Offset, upload: true );
    }
    /// <summary>Copies selected subresources from a CopySource texture into a bounded readback ring.</summary>
    /// <remarks>A zero count selects all remaining subresources. Wait for GPU completion before reading the returned owner.</remarks>
    public TextureReadback ReadbackTexture( Texture2D texture, ReadbackRing readbacks, uint firstSubresource = 0, uint subresourceCount = 0 ) {
        Recording( ); RequireSameDevice( texture ); ArgumentNullException.ThrowIfNull( readbacks ); RequireSameDevice( readbacks.Buffer );
        if ( firstSubresource >= texture.Description.SubresourceCount ) throw new ArgumentOutOfRangeException( nameof(firstSubresource) );
        if ( subresourceCount == 0 ) subresourceCount = texture.Description.SubresourceCount - firstSubresource;
        var layout = texture.Layout.Select( firstSubresource, subresourceCount );
        var slice = readbacks.Allocate( layout.Size );
        try {
            RetainAllocation( slice ); Track( texture.Resource );
            for ( uint i = 0; i < subresourceCount; ++i ) CopyTextureSlice( texture, firstSubresource + i, slice.Buffer, layout.Footprints[i], slice.Offset, upload: false );
            return new( slice, layout, firstSubresource );
        } catch { slice.Dispose( ); throw; }
    }
    private void CopyTextureSlice( Texture2D texture, uint subresource, GpuResource buffer, D3D12_PLACED_SUBRESOURCE_FOOTPRINT footprint, ulong baseOffset, bool upload ) {
        using var image = texture.Resource.Native.Borrow( ); using var staging = buffer.Native.Borrow( );
        D3D12_TEXTURE_COPY_LOCATION tex = new( ) { pResource = (ID3D12Resource_unmanaged*)image.Pointer, Type = D3D12_TEXTURE_COPY_TYPE.D3D12_TEXTURE_COPY_TYPE_SUBRESOURCE_INDEX };
        tex.Anonymous.SubresourceIndex = subresource;
        D3D12_TEXTURE_COPY_LOCATION buf = new( ) { pResource = (ID3D12Resource_unmanaged*)staging.Pointer, Type = D3D12_TEXTURE_COPY_TYPE.D3D12_TEXTURE_COPY_TYPE_PLACED_FOOTPRINT };
        footprint.Offset = checked(footprint.Offset + baseOffset); buf.Anonymous.PlacedFootprint = footprint;
        if ( upload ) Native.Interface.CopyTextureRegion( tex, 0, 0, 0, buf, null ); else Native.Interface.CopyTextureRegion( buf, 0, 0, 0, tex, null );
    }
    /// <summary>Binds the texture's SRV to a graphics table and retains its descriptor range.</summary>
    public void SetGraphicsTexture( Texture2D texture, uint parameterIndex = 0 ) {
        RequireDirectRecording( ); RequireSameDevice( texture ); Track( texture.Resource );
        SetDescriptorHeaps( texture.View ); SetGraphicsDescriptorTable( parameterIndex, texture.View );
    }
    /// <summary>Binds consecutive single-texture root tables in one shared heap, preserving all material tables and retaining each resource/range.</summary>
    public void SetGraphicsTextureTables(uint firstParameter, ReadOnlySpan<Texture2D> textures) {
        RequireDirectRecording();
        if (textures.IsEmpty) throw new ArgumentException("Supply at least one texture.", nameof(textures));
        foreach (var texture in textures) {
            RequireSameDevice(texture);
            if (!ReferenceEquals(texture.View.Heap, textures[0].View.Heap)) throw new ArgumentException("Material textures must share one descriptor heap.", nameof(textures));
        }
        SetDescriptorHeaps(textures[0].View);
        for (int i=0;i<textures.Length;++i) { Track(textures[i].Resource); SetGraphicsDescriptorTable(checked(firstParameter+(uint)i), textures[i].View); }
    }

}
