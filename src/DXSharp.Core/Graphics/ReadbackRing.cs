namespace DXSharp.Graphics;

/// <summary>A bounded ring of CPU-readable buffer slices with independent recording and owner lifetimes.</summary>
public sealed class ReadbackRing : IDisposable {
    private readonly AllocationArena _arena;
    internal readonly GpuResource Buffer;
    /// <summary>The backing buffer's byte capacity.</summary>
    public ulong Capacity => _arena.Capacity;
    /// <summary>Free bytes before alignment and fragmentation constraints.</summary>
    public ulong AvailableBytes => _arena.Available;
    /// <summary>Whether new reservations have been disabled.</summary>
    public bool IsDisposed => _arena.IsDisposed;
    internal ReadbackRing( GpuResource buffer ) { Buffer = buffer; _arena = new( buffer.SizeInBytes, buffer ); }
    /// <summary>Reserves an aligned contiguous slice, throwing on exhaustion or fragmentation.</summary>
    public ReadbackAllocation Allocate( ulong size, ulong alignment = UploadRing.TexturePlacementAlignment ) =>
        TryAllocate( size, out var allocation, alignment ) ? allocation! : throw new InvalidOperationException( "No contiguous readback slice is available." );
    /// <summary>Attempts a nonblocking reservation with power-of-two alignment.</summary>
    public bool TryAllocate( ulong size, out ReadbackAllocation? allocation, ulong alignment = UploadRing.TexturePlacementAlignment ) {
        allocation = null;
        if ( !_arena.TryReserve( size, alignment, out ulong offset, out var lifetime ) ) return false;
        allocation = new( lifetime!, Buffer, offset, size ); return true;
    }
    /// <summary>Disables new reservations. Existing allocations and recordings retain the buffer.</summary>
    public void Dispose( ) { _arena.Dispose( ); GC.SuppressFinalize( this ); }
    /// <summary>Releases an abandoned ring owner.</summary>
    ~ReadbackRing( ) => _arena.Dispose( );
}

/// <summary>A readback slice whose CPU reads are rejected while any declared GPU use is recorded or pending.</summary>
/// <remarks>Contents are undefined until copied. Wait for all writing submissions before reading. Native uses must declare this allocation through RetainAllocation.</remarks>
public sealed class ReadbackAllocation : GraphicsAllocation {
    internal readonly GpuResource Buffer;
    internal override GraphicsObject Backing => Buffer;
    /// <summary>The byte offset in the backing buffer.</summary>
    public ulong Offset { get; }
    /// <summary>The reserved byte length.</summary>
    public ulong SizeInBytes { get; }
    internal ReadbackAllocation( AllocationLifetime lifetime, GpuResource buffer, ulong offset, ulong size ) : base( lifetime ) {
        Buffer = buffer; Offset = offset; SizeInBytes = size;
    }
    /// <summary>Copies completed GPU results into a bounded CPU span.</summary>
    public void Read<T>( Span<T> destination, ulong byteOffset = 0 ) where T : unmanaged {
        lock ( Lifetime.Gate ) {
            Lifetime.RequireIdle( );
            ValidateRange( byteOffset, checked((ulong)destination.Length * (ulong)System.Runtime.CompilerServices.Unsafe.SizeOf<T>( )) );
            Buffer.Read( destination, checked(Offset + byteOffset) );
        }
    }
    internal void ValidateRange( ulong offset, ulong size ) {
        if ( offset > SizeInBytes || size > SizeInBytes - offset ) throw new ArgumentOutOfRangeException( nameof(size), "The range exceeds the readback slice." );
    }
}

public sealed partial class GraphicsDevice {
    /// <summary>Creates a bounded readback ring. GPU completion remains the caller's synchronization responsibility.</summary>
    public ReadbackRing CreateReadbackRing( ulong capacity ) => new( CreateBuffer( capacity, BufferMemory.Readback ) );
}

public sealed partial class GraphicsCommandList {
    /// <summary>Copies a buffer region into a retained readback slice.</summary>
    public void CopyBuffer( GpuResource source, ReadbackAllocation destination, ulong size, ulong sourceOffset = 0, ulong destinationOffset = 0 ) {
        ArgumentNullException.ThrowIfNull( destination ); Recording( ); RequireSameDevice( source );
        source.ValidateRange( sourceOffset, size ); destination.ValidateRange( destinationOffset, size );
        RetainAllocation( destination ); CopyBuffer( source, destination.Buffer, size, sourceOffset, checked(destination.Offset + destinationOffset) );
    }
    /// <summary>Copies between retained upload/readback slices without releasing either range before GPU completion.</summary>
    public void CopyBuffer( UploadAllocation source, ReadbackAllocation destination, ulong size, ulong sourceOffset = 0, ulong destinationOffset = 0 ) {
        ArgumentNullException.ThrowIfNull( source ); ArgumentNullException.ThrowIfNull( destination ); Recording( );
        source.ValidateRange( sourceOffset, size ); destination.ValidateRange( destinationOffset, size );
        RequireSameDevice( source.Buffer ); RequireSameDevice( destination.Buffer );
        RetainAllocation( source ); RetainAllocation( destination );
        CopyBuffer( source.Buffer, destination.Buffer, size, checked(source.Offset + sourceOffset), checked(destination.Offset + destinationOffset) );
    }
}
