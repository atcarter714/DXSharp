using DXSharp.Interop;
using Windows.Win32.Graphics.Direct3D12;

namespace DXSharp.Graphics;

// Ranges are reserved by their public owner and every recording/submission that uses them.
// Completion releases a recording lease; owner disposal alone never makes a busy range reusable.
internal sealed class AllocationLifetime( Action release ) : IDisposable {
    internal readonly object Gate = new( );
    private readonly Dictionary<uint, IDisposable> _dependencies = new( );
    private bool _disposed;
    private int _recordings;
    private bool _released;
    internal bool IsDisposed { get { lock ( Gate ) return _disposed; } }
    internal void RequireOwner( ) => ObjectDisposedException.ThrowIf( _disposed, this );
    internal void RequireWritable( ) {
        RequireOwner( );
        if ( _recordings != 0 ) throw new InvalidOperationException( "The allocation is referenced by a recording or pending submission. Retire or reset those recordings before writing." );
    }
    internal IDisposable Acquire( ) {
        lock ( Gate ) { RequireOwner( ); ++_recordings; return new RecordingLease( this ); }
    }
    internal void ReplaceDependency( uint index, IDisposable dependency ) {
        if ( _dependencies.Remove( index, out var previous ) ) previous.Dispose( );
        _dependencies.Add( index, dependency );
    }
    public void Dispose( ) {
        lock ( Gate ) { if ( _disposed ) return; _disposed = true; ReleaseIfUnused( ); }
    }
    private void Retire( ) {
        lock ( Gate ) { --_recordings; ReleaseIfUnused( ); }
    }
    private void ReleaseIfUnused( ) {
        if ( !_disposed || _recordings != 0 || _released ) return;
        _released = true;
        try { foreach ( var dependency in _dependencies.Values ) dependency.Dispose( ); }
        finally { _dependencies.Clear( ); release( ); }
    }
    private sealed class RecordingLease( AllocationLifetime owner ) : IDisposable {
        private AllocationLifetime? _owner = owner;
        public void Dispose( ) => Interlocked.Exchange( ref _owner, null )?.Retire( );
    }
}

// A circular next-fit allocator. Live allocations can retire in any order; adjacent free
// ranges coalesce, and a reservation never straddles the end of the backing allocation.
internal sealed class AllocationArena : IDisposable {
    private readonly ulong _capacity;
    private readonly IDisposable _backing;
    internal AllocationArena( ulong capacity, IDisposable backing ) {
        _capacity = capacity; _backing = backing; _available = capacity; _free = [(0, capacity)];
    }
    private readonly object _gate = new( );
    private readonly List<(ulong Offset, ulong Size)> _free;
    private ulong _cursor;
    private ulong _available;
    private int _owners = 1;
    private bool _disposed;
    internal ulong Capacity => _capacity;
    internal bool IsDisposed { get { lock ( _gate ) return _disposed; } }
    internal ulong Available { get { lock ( _gate ) return _available; } }
    internal bool TryReserve( ulong size, ulong alignment, out ulong offset, out AllocationLifetime? lifetime ) {
        if ( size == 0 || size > _capacity ) throw new ArgumentOutOfRangeException( nameof(size) );
        if ( alignment == 0 || (alignment & (alignment - 1)) != 0 ) throw new ArgumentOutOfRangeException( nameof(alignment), "Alignment must be a power of two." );
        lock ( _gate ) {
            ObjectDisposedException.ThrowIf( _disposed, this );
            offset = 0; lifetime = null;
            if ( !Find( _cursor, _capacity, size, alignment, out int index, out offset ) &&
                 !Find( 0, _capacity, size, alignment, out index, out offset ) ) return false;
            var range = _free[index];
            ulong end = offset + size, rangeEnd = range.Offset + range.Size;
            _free.RemoveAt( index );
            if ( end < rangeEnd ) _free.Insert( index, (end, rangeEnd - end) );
            if ( offset > range.Offset ) _free.Insert( index, (range.Offset, offset - range.Offset) );
            _cursor = end == _capacity ? 0 : end;
            _available -= size; ++_owners;
            ulong reservedOffset = offset;
            lifetime = new( ( ) => Return( reservedOffset, size ) );
            return true;
        }
    }
    private bool Find( ulong begin, ulong end, ulong size, ulong alignment, out int index, out ulong offset ) {
        for ( int i = 0; i < _free.Count; ++i ) {
            var range = _free[i];
            ulong start = Math.Max( begin, range.Offset ), limit = Math.Min( end, range.Offset + range.Size );
            ulong mask = alignment - 1;
            if ( start > ulong.MaxValue - mask ) continue;
            start = (start + mask) & ~mask;
            if ( start <= limit && size <= limit - start ) { index = i; offset = start; return true; }
        }
        index = -1; offset = 0; return false;
    }
    private void Return( ulong offset, ulong size ) {
        lock ( _gate ) {
            int index = 0;
            while ( index < _free.Count && _free[index].Offset < offset ) ++index;
            _free.Insert( index, (offset, size) );
            if ( index > 0 && _free[index - 1].Offset + _free[index - 1].Size == offset ) {
                var previous = _free[index - 1];
                _free[index - 1] = (previous.Offset, previous.Size + size); _free.RemoveAt( index ); --index;
            }
            if ( index + 1 < _free.Count && _free[index].Offset + _free[index].Size == _free[index + 1].Offset ) {
                var current = _free[index];
                _free[index] = (current.Offset, current.Size + _free[index + 1].Size); _free.RemoveAt( index + 1 );
            }
            _available += size;
            if ( --_owners == 0 ) _backing.Dispose( );
        }
    }
    public void Dispose( ) {
        lock ( _gate ) {
            if ( _disposed ) return;
            _disposed = true;
            if ( --_owners == 0 ) _backing.Dispose( );
        }
    }
}

/// <summary>A suballocation whose range remains reserved until its owner and all recorded uses are released.</summary>
/// <remarks>Dispose the owner after its final CPU use. Recording freezes managed writes until the recording is reset or its submission retires.</remarks>
public abstract class GraphicsAllocation : IDisposable {
    internal readonly AllocationLifetime Lifetime;
    internal abstract GraphicsObject Backing { get; }
    internal GraphicsAllocation( AllocationLifetime lifetime ) => Lifetime = lifetime;
    /// <summary>Whether the public allocation owner has been disposed.</summary>
    public bool IsDisposed => Lifetime.IsDisposed;
    /// <summary>Releases the public reservation; pending recordings keep the range and backing object alive.</summary>
    public void Dispose( ) { Lifetime.Dispose( ); GC.SuppressFinalize( this ); }
    /// <summary>Reclaims an abandoned owner's reservation without releasing live recording leases.</summary>
    ~GraphicsAllocation( ) => Lifetime.Dispose( );
}

/// <summary>The native descriptor heap family.</summary>
public enum DescriptorKind {
    /// <summary>Constant-buffer, shader-resource and unordered-access descriptors.</summary>
    Resource = 0,
    /// <summary>Sampler descriptors.</summary>
    Sampler = 1,
    /// <summary>Render-target descriptors; not shader-visible.</summary>
    RenderTarget = 2,
    /// <summary>Depth-stencil descriptors; not shader-visible.</summary>
    DepthStencil = 3
}

/// <summary>A bounded, thread-safe descriptor range allocator backed by one native heap.</summary>
/// <remarks>Disposal prevents new allocations. Existing allocations and GPU uses retain the backing heap. Native descriptor writes must obey the same no-overwrite contract as managed writes.</remarks>
public sealed class DescriptorPool : IDisposable {
    private readonly AllocationArena _arena;
    internal readonly DescriptorHeap Heap;
    /// <summary>The descriptor family accepted by this heap.</summary>
    public DescriptorKind Kind { get; }
    /// <summary>Whether the heap can be bound to shaders.</summary>
    public bool ShaderVisible { get; }
    /// <summary>The total descriptor count.</summary>
    public uint Capacity => Heap.Capacity;
    /// <summary>The number of free descriptors, which may be fragmented.</summary>
    public uint AvailableCount => (uint)_arena.Available;
    /// <summary>Whether new allocations have been disabled by disposal.</summary>
    public bool IsDisposed => _arena.IsDisposed;
    internal DescriptorPool( DescriptorHeap heap, DescriptorKind kind, bool shaderVisible ) {
        Heap = heap; Kind = kind; ShaderVisible = shaderVisible; _arena = new( heap.Capacity, heap );
    }
    /// <summary>Reserves a contiguous range, throwing if the heap is exhausted or fragmented.</summary>
    public DescriptorAllocation Allocate( uint count = 1 ) => TryAllocate( count, out var allocation ) ? allocation! :
        throw new InvalidOperationException( "No contiguous descriptor range is available. Retire pending uses or dispose unused allocation owners." );
    /// <summary>Attempts a nonblocking reservation. Returns false for exhaustion or fragmentation.</summary>
    public bool TryAllocate( uint count, out DescriptorAllocation? allocation ) {
        allocation = null;
        if ( !_arena.TryReserve( count, 1, out ulong offset, out var lifetime ) ) return false;
        allocation = new( lifetime!, Heap, Kind, ShaderVisible, (uint)offset, count ); return true;
    }
    /// <summary>Stops new allocations and releases the heap after its last allocation retires.</summary>
    public void Dispose( ) { _arena.Dispose( ); GC.SuppressFinalize( this ); }
    /// <summary>Releases an abandoned pool owner.</summary>
    ~DescriptorPool( ) => _arena.Dispose( );
}

/// <summary>A contiguous descriptor range. Recording retains its slots and descriptors' managed resource dependencies.</summary>
public sealed class DescriptorAllocation : GraphicsAllocation {
    internal readonly DescriptorHeap Heap;
    internal override GraphicsObject Backing => Heap;
    /// <summary>The heap family of this range.</summary>
    public DescriptorKind Kind { get; }
    /// <summary>Whether this range has GPU-visible handles.</summary>
    public bool ShaderVisible { get; }
    /// <summary>The first descriptor index in its backing heap.</summary>
    public uint StartIndex { get; }
    /// <summary>The number of contiguous descriptors reserved.</summary>
    public uint Count { get; }
    internal DescriptorAllocation( AllocationLifetime lifetime, DescriptorHeap heap, DescriptorKind kind, bool visible, uint start, uint count ) : base( lifetime ) {
        Heap = heap; Kind = kind; ShaderVisible = visible; StartIndex = start; Count = count;
    }
    /// <summary>Gets a native CPU handle. Native callers must not overwrite a descriptor referenced by any recording or pending submission.</summary>
    public D3D12_CPU_DESCRIPTOR_HANDLE GetCpuHandle( uint index = 0 ) {
        lock ( Lifetime.Gate ) { Lifetime.RequireOwner( ); ValidateIndex( index ); return Heap.CpuHandle( StartIndex + index ); }
    }
    /// <summary>Gets a GPU handle for a shader-visible allocation.</summary>
    public D3D12_GPU_DESCRIPTOR_HANDLE GetGpuHandle( uint index = 0 ) {
        lock ( Lifetime.Gate ) {
            Lifetime.RequireOwner( ); ValidateIndex( index );
            if ( !ShaderVisible ) throw new InvalidOperationException( "This descriptor heap is not shader-visible." );
            var handle = Heap.Native.Interface.GetGPUDescriptorHandleForHeapStart( );
            handle.ptr = checked(handle.ptr + (ulong)(StartIndex + index) * Heap.Stride); return handle;
        }
    }
    internal void ValidateIndex( uint index ) { if ( index >= Count ) throw new ArgumentOutOfRangeException( nameof(index) ); }
}

/// <summary>A bounded, thread-safe next-fit ring of upload-buffer slices. Exhaustion never blocks or silently grows the backing buffer.</summary>
public sealed class UploadRing : IDisposable {
    private readonly AllocationArena _arena;
    internal readonly GpuResource Buffer;
    /// <summary>The D3D12 constant-buffer placement alignment in bytes.</summary>
    public const ulong ConstantBufferAlignment = 256;
    /// <summary>The D3D12 texture-copy placement alignment in bytes.</summary>
    public const ulong TexturePlacementAlignment = 512;
    /// <summary>The size of the backing upload buffer.</summary>
    public ulong Capacity => _arena.Capacity;
    /// <summary>Free bytes before alignment and fragmentation constraints.</summary>
    public ulong AvailableBytes => _arena.Available;
    /// <summary>Whether new allocations have been disabled by disposal.</summary>
    public bool IsDisposed => _arena.IsDisposed;
    internal UploadRing( GpuResource buffer ) { Buffer = buffer; _arena = new( buffer.SizeInBytes, buffer ); }
    /// <summary>Reserves a contiguous aligned slice, throwing on exhaustion or fragmentation.</summary>
    public UploadAllocation Allocate( ulong size, ulong alignment = ConstantBufferAlignment ) => TryAllocate( size, out var allocation, alignment ) ? allocation! :
        throw new InvalidOperationException( "No contiguous upload slice is available. Retire pending uses or dispose unused allocation owners." );
    /// <summary>Attempts a nonblocking reservation with a power-of-two byte alignment.</summary>
    public bool TryAllocate( ulong size, out UploadAllocation? allocation, ulong alignment = ConstantBufferAlignment ) {
        allocation = null;
        if ( !_arena.TryReserve( size, alignment, out ulong offset, out var lifetime ) ) return false;
        allocation = new( lifetime!, Buffer, offset, size ); return true;
    }
    /// <summary>Stops new allocations and releases the buffer after its final slice retires.</summary>
    public void Dispose( ) { _arena.Dispose( ); GC.SuppressFinalize( this ); }
    /// <summary>Releases an abandoned ring owner.</summary>
    ~UploadRing( ) => _arena.Dispose( );
}

/// <summary>A CPU-writable upload slice protected against managed writes while recorded or submitted.</summary>
public sealed class UploadAllocation : GraphicsAllocation {
    internal readonly GpuResource Buffer;
    internal override GraphicsObject Backing => Buffer;
    /// <summary>The byte offset into the backing buffer.</summary>
    public ulong Offset { get; }
    /// <summary>The reserved byte length.</summary>
    public ulong SizeInBytes { get; }
    /// <summary>The GPU virtual address of this slice.</summary>
    public ulong GpuAddress { get { lock ( Lifetime.Gate ) { Lifetime.RequireOwner( ); return checked(Buffer.GpuAddress + Offset); } } }
    internal UploadAllocation( AllocationLifetime lifetime, GpuResource buffer, ulong offset, ulong size ) : base( lifetime ) {
        Buffer = buffer; Offset = offset; SizeInBytes = size;
    }
    /// <summary>Writes within this slice before recording, or after all recorded uses retire.</summary>
    public void Write<T>( ReadOnlySpan<T> source, ulong byteOffset = 0 ) where T : unmanaged {
        lock ( Lifetime.Gate ) {
            Lifetime.RequireWritable( );
            ValidateRange( byteOffset, checked((ulong)source.Length * (ulong)System.Runtime.CompilerServices.Unsafe.SizeOf<T>( )) );
            Buffer.Write( source, checked(Offset + byteOffset) );
        }
    }
    internal void ValidateRange( ulong offset, ulong size ) {
        if ( offset > SizeInBytes || size > SizeInBytes - offset ) throw new ArgumentOutOfRangeException( nameof(size), "The range exceeds the upload slice." );
    }
}

public sealed unsafe partial class GraphicsDevice {
    /// <summary>Creates a bounded descriptor pool. RTV/DSV pools require shaderVisible to be false.</summary>
    public DescriptorPool CreateDescriptorPool( uint capacity, DescriptorKind kind = DescriptorKind.Resource, bool shaderVisible = true ) {
        ArgumentOutOfRangeException.ThrowIfZero( capacity );
        if ( !Enum.IsDefined( kind ) ) throw new ArgumentOutOfRangeException( nameof(kind) );
        if ( shaderVisible && kind is DescriptorKind.RenderTarget or DescriptorKind.DepthStencil )
            throw new ArgumentException( "Render-target and depth-stencil heaps cannot be shader-visible.", nameof(shaderVisible) );
        var type = (D3D12_DESCRIPTOR_HEAP_TYPE)kind;
        D3D12_DESCRIPTOR_HEAP_DESC description = new( ) { Type = type, NumDescriptors = capacity,
            Flags = shaderVisible ? D3D12_DESCRIPTOR_HEAP_FLAGS.D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE : 0 };
        Guid id = typeof(ID3D12DescriptorHeap).GUID;
        Results.Check( Native.Interface.CreateDescriptorHeap( &description, &id, out nint pointer ), "CreateDescriptorHeap(pool)" );
        return new( new DescriptorHeap( ComOwner<ID3D12DescriptorHeap>.Adopt( pointer ), capacity,
            Native.Interface.GetDescriptorHandleIncrementSize( type ) ) { DeviceIdentity = DeviceIdentity }, kind, shaderVisible );
    }
    /// <summary>Creates a bounded upload ring. Callers record appropriate resource states and queue ordering.</summary>
    public UploadRing CreateUploadRing( ulong capacity ) => new( CreateBuffer( capacity, BufferMemory.Upload ) );
    /// <summary>Writes an SRV and retains its resource until the descriptor range becomes reusable.</summary>
    /// <remarks>Managed writes reject ranges referenced by recordings. Native writes through CPU handles must enforce this contract themselves.</remarks>
    public void CreateShaderResourceView( GpuResource resource, DescriptorAllocation allocation, uint index = 0,
        D3D12_SHADER_RESOURCE_VIEW_DESC? description = null ) {
        ArgumentNullException.ThrowIfNull( allocation );
        RequireSameDevice( resource ); RequireSameDevice( allocation.Heap );
        lock ( allocation.Lifetime.Gate ) {
            allocation.Lifetime.RequireWritable( ); allocation.ValidateIndex( index );
            if ( allocation.Kind != DescriptorKind.Resource ) throw new ArgumentException( "An SRV requires a resource descriptor heap.", nameof(allocation) );
            var nativeDescription = description.GetValueOrDefault( );
            var lease = resource.Retain( );
            try { Native.Interface.CreateShaderResourceView( resource.Native.Interface, description.HasValue ? &nativeDescription : null,
                allocation.Heap.CpuHandle( allocation.StartIndex + index ) ); }
            catch { lease.Dispose( ); throw; }
            allocation.Lifetime.ReplaceDependency( index, lease );
        }
    }
}

public sealed unsafe partial class GraphicsCommandList {
    /// <summary>Binds resource and/or sampler heaps and retains the supplied allocation ranges.</summary>
    /// <remarks>Setting heaps replaces the previous heap bindings. Rebind affected descriptor tables afterwards.</remarks>
    public void SetDescriptorHeaps( DescriptorAllocation? resources, DescriptorAllocation? samplers = null ) {
        Recording( );
        if ( Kind == QueueKind.Copy ) throw new InvalidOperationException( "A copy list cannot bind descriptor heaps." );
        if ( resources is null && samplers is null ) throw new ArgumentException( "Supply at least one shader-visible allocation." );
        if ( resources is not null ) ValidateShaderAllocation( resources, DescriptorKind.Resource );
        if ( samplers is not null ) ValidateShaderAllocation( samplers, DescriptorKind.Sampler );
        var heaps = new List<ID3D12DescriptorHeap>( 2 );
        if ( resources is not null ) {
            RetainAllocation( resources ); heaps.Add( resources.Heap.Native.Interface );
        }
        if ( samplers is not null ) {
            RetainAllocation( samplers ); heaps.Add( samplers.Heap.Native.Interface );
        }
        Native.Interface.SetDescriptorHeaps( (uint)heaps.Count, heaps.ToArray( ) );
    }
    /// <summary>Binds a graphics descriptor table within a previously bound heap and retains its allocation.</summary>
    public void SetGraphicsDescriptorTable( uint parameterIndex, DescriptorAllocation allocation, uint index = 0 ) {
        RequireDirectRecording( ); ValidateShaderAllocation( allocation );
        var handle = allocation.GetGpuHandle( index ); RetainAllocation( allocation );
        Native.Interface.SetGraphicsRootDescriptorTable( parameterIndex, handle );
    }
    /// <summary>Binds a compute descriptor table within a previously bound heap and retains its allocation.</summary>
    public void SetComputeDescriptorTable( uint parameterIndex, DescriptorAllocation allocation, uint index = 0 ) {
        Recording( );
        if ( Kind == QueueKind.Copy ) throw new InvalidOperationException( "A copy list cannot bind compute tables." );
        ValidateShaderAllocation( allocation );
        var handle = allocation.GetGpuHandle( index ); RetainAllocation( allocation );
        Native.Interface.SetComputeRootDescriptorTable( parameterIndex, handle );
    }
    private void ValidateShaderAllocation( DescriptorAllocation allocation, DescriptorKind? kind = null ) {
        ArgumentNullException.ThrowIfNull( allocation ); RequireSameDevice( allocation.Heap );
        if ( !allocation.ShaderVisible || (kind.HasValue && allocation.Kind != kind.Value) )
            throw new ArgumentException( "The allocation must belong to a compatible shader-visible descriptor heap.", nameof(allocation) );
    }
    /// <summary>Retains a suballocation's range and backing object through this recording's submission completion.</summary>
    /// <remarks>Call before native commands referencing an allocation. Use separate recordings for simultaneous submissions to multiple queues.</remarks>
    public void RetainAllocation( GraphicsAllocation allocation ) {
        ArgumentNullException.ThrowIfNull( allocation ); Recording( ); RequireSameDevice( allocation.Backing );
        lock ( allocation.Lifetime.Gate ) allocation.Lifetime.RequireOwner( );
        if ( _allocationRetained.ContainsKey( allocation ) ) return;
        var lease = allocation.Lifetime.Acquire( );
        try { Track( allocation.Backing ); _allocationRetained.Add( allocation, lease ); }
        catch { lease.Dispose( ); throw; }
    }
    /// <summary>Copies from an upload slice while retaining its byte range until completion.</summary>
    public void CopyBuffer( UploadAllocation source, GpuResource destination, ulong size, ulong sourceOffset = 0, ulong destinationOffset = 0 ) {
        ArgumentNullException.ThrowIfNull( source ); Recording( );
        source.ValidateRange( sourceOffset, size ); destination.ValidateRange( destinationOffset, size );
        RequireSameDevice( destination ); RetainAllocation( source );
        CopyBuffer( source.Buffer, destination, size, checked(source.Offset + sourceOffset), destinationOffset );
    }
}
