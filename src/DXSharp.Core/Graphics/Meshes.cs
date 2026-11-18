using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.Graphics.Direct3D12;
using Windows.Win32.Graphics.Dxgi.Common;

namespace DXSharp.Graphics;

/// <summary>Element formats accepted by a graphics input layout.</summary>
public enum VertexFormat {
    /// <summary>Two 32-bit floating-point components.</summary>
    Float2 = 16,
    /// <summary>Three 32-bit floating-point components.</summary>
    Float3 = 6,
    /// <summary>Four 32-bit floating-point components.</summary>
    Float4 = 2,
    /// <summary>Four 32-bit unsigned integer components, such as joint indices.</summary>
    UInt4 = 3,
    /// <summary>Four normalized unsigned bytes, such as vertex colors or weights.</summary>
    Unorm8x4 = 28
}

/// <summary>An attribute in a vertex or instance stream. Offsets and slots are explicit.</summary>
/// <param name="Semantic">ASCII shader semantic name, such as POSITION or TEXCOORD.</param>
/// <param name="Format">Storage format of the attribute.</param>
/// <param name="Offset">Byte offset from the beginning of a stream element.</param>
/// <param name="SemanticIndex">Semantic suffix, such as 1 for TEXCOORD1.</param>
/// <param name="Slot">Input-assembler stream slot, from 0 through 31.</param>
/// <param name="InstanceStepRate">Zero reads per vertex. A positive value advances once per this many instances.</param>
public readonly record struct VertexElement( string Semantic, VertexFormat Format, uint Offset,
    uint SemanticIndex = 0, uint Slot = 0, uint InstanceStepRate = 0 );

/// <summary>The storage width of an index-buffer element.</summary>
public enum IndexFormat {
    /// <summary>An unsigned 16-bit vertex index.</summary>
    UInt16 = 57,
    /// <summary>An unsigned 32-bit vertex index.</summary>
    UInt32 = 42
}

/// <summary>Owns an interleaved vertex buffer and an index buffer in GPU working memory.</summary>
/// <remarks>Upload commands must execute before drawing on the same direct queue.
/// Dispose after the final recording. Recorded commands retain independent native references.</remarks>
public sealed class Mesh : IDisposable {
    internal GpuResource Vertices { get; }
    internal GpuResource Indices { get; }
    /// <summary>The number of vertices in the vertex buffer.</summary>
    public uint VertexCount { get; }
    /// <summary>The size of one vertex in bytes, including struct padding.</summary>
    public uint VertexStride { get; }
    /// <summary>The number of indices in the index buffer.</summary>
    public uint IndexCount { get; }
    /// <summary>The storage format of the indices.</summary>
    public IndexFormat IndexFormat { get; }
    /// <summary>Whether the public buffer owners have been disposed.</summary>
    public bool IsDisposed => Vertices.IsDisposed || Indices.IsDisposed;

    internal Mesh( GpuResource vertices, GpuResource indices, uint vertexCount, uint stride, uint indexCount, IndexFormat format ) {
        Vertices = vertices;
        Indices = indices;
        VertexCount = vertexCount;
        VertexStride = stride;
        IndexCount = indexCount;
        IndexFormat = format;
    }

    /// <summary>Releases both public buffer owners. Submission references retire at their queue fence.</summary>
    public void Dispose( ) {
        Indices.Dispose( );
        Vertices.Dispose( );
    }
}

public sealed unsafe partial class GraphicsDevice {
    /// <summary>Records a mesh upload with 16-bit indices into an open direct command list on this device.</summary>
    /// <remarks>Multiple uploads can share one recording and submission. Upload staging is retained through fence completion.
    /// Vertices must have a fixed unmanaged layout matching the pipeline's input elements. Every index is range checked.</remarks>
    public Mesh CreateMesh<TVertex>( GraphicsCommandList commands, ReadOnlySpan<TVertex> vertices,
        ReadOnlySpan<ushort> indices ) where TVertex : unmanaged =>
        CreateMesh<TVertex, ushort>( commands, vertices, indices, IndexFormat.UInt16 );

    /// <summary>Records a mesh upload with 32-bit indices into an open direct command list on this device.</summary>
    /// <remarks>Execute the upload before drawing on the same queue. Staging memory is retained by the recording.
    /// The caller supplies a matching input layout and synchronization for any subsequent buffer modifications.</remarks>
    public Mesh CreateMesh<TVertex>( GraphicsCommandList commands, ReadOnlySpan<TVertex> vertices,
        ReadOnlySpan<uint> indices ) where TVertex : unmanaged =>
        CreateMesh<TVertex, uint>( commands, vertices, indices, IndexFormat.UInt32 );

    private Mesh CreateMesh<TVertex, TIndex>( GraphicsCommandList commands, ReadOnlySpan<TVertex> vertices,
        ReadOnlySpan<TIndex> indices, IndexFormat format ) where TVertex : unmanaged where TIndex : unmanaged, IBinaryInteger<TIndex> {
        RequireSameDevice( commands );
        commands.RequireDirectRecording( );
        if ( vertices.IsEmpty ) throw new ArgumentException( "At least one vertex is required.", nameof(vertices) );
        if ( indices.IsEmpty ) throw new ArgumentException( "At least one index is required.", nameof(indices) );
        uint stride = checked((uint)Unsafe.SizeOf<TVertex>( ));
        if ( stride > 2048 ) throw new ArgumentException( "Vertex stride exceeds 2048 bytes.", nameof(vertices) );
        foreach ( TIndex index in indices ) {
            if ( uint.CreateChecked( index ) >= (uint)vertices.Length )
                throw new ArgumentOutOfRangeException( nameof(indices), "An index references a vertex outside the supplied span." );
        }

        uint vertexBytes = checked((uint)MemoryMarshal.AsBytes( vertices ).Length);
        uint indexBytes = checked((uint)MemoryMarshal.AsBytes( indices ).Length);
        ulong indexOffset = ((ulong)vertexBytes + 3) & ~3UL;
        var vertexBuffer = CreateBuffer( vertexBytes );
        try {
            var indexBuffer = CreateBuffer( indexBytes );
            try {
                using var upload = CreateBuffer( checked(indexOffset + indexBytes), BufferMemory.Upload );
                upload.Write( vertices );
                upload.Write( indices, indexOffset );
                commands.Transition( vertexBuffer, ResourceState.Common, ResourceState.CopyDestination );
                commands.Transition( indexBuffer, ResourceState.Common, ResourceState.CopyDestination );
                commands.CopyBuffer( upload, vertexBuffer, vertexBytes );
                commands.CopyBuffer( upload, indexBuffer, indexBytes, sourceOffset: indexOffset );
                commands.Transition( vertexBuffer, ResourceState.CopyDestination, ResourceState.VertexAndConstantBuffer );
                commands.Transition( indexBuffer, ResourceState.CopyDestination, ResourceState.IndexBuffer );
                return new( vertexBuffer, indexBuffer, (uint)vertices.Length, stride, (uint)indices.Length, format );
            } catch {
                indexBuffer.Dispose( );
                throw;
            }
        } catch {
            vertexBuffer.Dispose( );
            throw;
        }
    }
}

public sealed unsafe partial class GraphicsCommandList {
    internal void RequireDirectRecording( ) {
        Recording( );
        if ( Kind != QueueKind.Direct ) throw new InvalidOperationException( "Input-assembler and mesh commands require a direct command list." );
    }

    /// <summary>Binds a vertex or instance stream and retains the buffer through submission retirement.</summary>
    /// <param name="slot">Input-assembler slot, from 0 through 31.</param>
    /// <param name="buffer">A buffer in vertex/constant-buffer or generic-read state.</param>
    /// <param name="stride">Distance in bytes between stream elements, from 1 through 2048.</param>
    /// <param name="byteOffset">Beginning of the view within the buffer.</param>
    /// <param name="sizeInBytes">View length. Zero selects the remaining buffer bytes, which must fit in a uint.</param>
    public void SetVertexBuffer( uint slot, GpuResource buffer, uint stride, ulong byteOffset = 0, uint sizeInBytes = 0 ) {
        RequireDirectRecording( );
        RequireSameDevice( buffer );
        if ( slot >= 32 ) throw new ArgumentOutOfRangeException( nameof(slot) );
        if ( stride is 0 or > 2048 ) throw new ArgumentOutOfRangeException( nameof(stride) );
        uint size = BufferViewSize( buffer, byteOffset, sizeInBytes );
        if ( size < stride ) throw new ArgumentException( "The view must contain at least one complete stream element.", nameof(sizeInBytes) );
        D3D12_VERTEX_BUFFER_VIEW view = new( ) {
            BufferLocation = checked(buffer.GpuAddress + byteOffset), SizeInBytes = size, StrideInBytes = stride
        };
        Track( buffer );
        Native.Interface.IASetVertexBuffers( slot, 1, &view );
    }

    /// <summary>Binds a 16-bit or 32-bit index view and retains its buffer. The caller supplies index-buffer or generic-read state.</summary>
    /// <param name="buffer">The source buffer.</param>
    /// <param name="format">Width of each unsigned index.</param>
    /// <param name="byteOffset">Beginning of the view, aligned to the index width.</param>
    /// <param name="sizeInBytes">View length. Zero selects the remaining buffer bytes.</param>
    public void SetIndexBuffer( GpuResource buffer, IndexFormat format, ulong byteOffset = 0, uint sizeInBytes = 0 ) {
        RequireDirectRecording( );
        RequireSameDevice( buffer );
        uint width = format switch {
            IndexFormat.UInt16 => 2,
            IndexFormat.UInt32 => 4,
            _ => throw new ArgumentOutOfRangeException( nameof(format) )
        };
        uint size = BufferViewSize( buffer, byteOffset, sizeInBytes );
        if ( byteOffset % width != 0 || size % width != 0 )
            throw new ArgumentException( "The index view must align to complete index elements." );
        D3D12_INDEX_BUFFER_VIEW view = new( ) {
            BufferLocation = checked(buffer.GpuAddress + byteOffset), SizeInBytes = size, Format = (DXGI_FORMAT)format
        };
        Track( buffer );
        Native.Interface.IASetIndexBuffer( &view );
    }

    /// <summary>Draws from the currently bound index and vertex buffers. The caller validates index and instance ranges.</summary>
    public void DrawIndexed( uint indexCount, uint instanceCount = 1, uint startIndex = 0, int baseVertex = 0, uint startInstance = 0 ) {
        RequireDirectRecording( );
        Native.Interface.DrawIndexedInstanced( indexCount, instanceCount, startIndex, baseVertex, startInstance );
    }

    /// <summary>Binds the mesh at vertex slot zero and draws all its indices, retaining both buffers through fence completion.</summary>
    /// <remarks>The pipeline must match the vertex struct. Execute the mesh upload first on the same direct queue.</remarks>
    public void DrawMesh( Mesh mesh, uint instanceCount = 1, uint startInstance = 0 ) {
        ArgumentNullException.ThrowIfNull( mesh );
        RequireDirectRecording( );
        RequireSameDevice( mesh.Vertices );
        RequireSameDevice( mesh.Indices );
        SetVertexBuffer( 0, mesh.Vertices, mesh.VertexStride );
        SetIndexBuffer( mesh.Indices, mesh.IndexFormat );
        DrawIndexed( mesh.IndexCount, instanceCount, startInstance: startInstance );
    }

    private static uint BufferViewSize( GpuResource buffer, ulong offset, uint requested ) {
        if ( buffer.Memory == BufferMemory.Readback || buffer.SizeInBytes == 0 )
            throw new ArgumentException( "A GPU-readable buffer is required.", nameof(buffer) );
        buffer.ValidateRange( offset, 0 );
        uint size = requested == 0 ? checked((uint)(buffer.SizeInBytes - offset)) : requested;
        buffer.ValidateRange( offset, size );
        if ( size == 0 ) throw new ArgumentOutOfRangeException( nameof(requested) );
        return size;
    }
}
