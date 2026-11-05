using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.Graphics.Direct3D12;
using DXSharp.Interop;

namespace DXSharp.Graphics;

/// <summary>An owning GPU resource. Buffer read/write methods copy data within a scoped native mapping.</summary>
public sealed unsafe class GpuResource : GraphicsObject {
    internal readonly ComOwner<ID3D12Resource> Native;
    /// <summary>Whether this buffer was created for unordered access.</summary>
    public bool SupportsUnorderedAccess { get; internal init; }
    /// <summary>The size of a buffer; zero for swap-chain textures.</summary>
    public ulong SizeInBytes { get; }
    /// <summary>The memory placement of this resource.</summary>
    public BufferMemory Memory { get; }
    internal GpuResource( ComOwner<ID3D12Resource> owner, ulong size, BufferMemory memory ) : base( owner ) {
        Native = owner; SizeInBytes = size; Memory = memory;
    }
    internal override ComReference Retain( ) => Native.Retain( );
    /// <summary>The buffer's GPU virtual address. The resource must remain live while the GPU uses it.</summary>
    public ulong GpuAddress => Native.Interface.GetGPUVirtualAddress( );

    /// <summary>Copies data into an upload buffer. Synchronize prior GPU reads before overwriting it.</summary>
    public void Write<T>( ReadOnlySpan<T> source, ulong byteOffset = 0 ) where T : unmanaged {
        if ( Memory != BufferMemory.Upload ) throw new InvalidOperationException( "Write requires an upload buffer." );
        ReadOnlySpan<byte> bytes = MemoryMarshal.AsBytes( source );
        ValidateRange( byteOffset, (ulong)bytes.Length );
        D3D12_RANGE readRange = default;
        void* pointer = null;
        Results.Check( Native.Interface.Map( 0, &readRange, &pointer ), "Resource.Map(write)" );
        try { bytes.CopyTo( new Span<byte>( (byte*)pointer + checked((nint)byteOffset), bytes.Length ) ); }
        finally {
            D3D12_RANGE written = new( ) { Begin = checked((nuint)byteOffset), End = checked((nuint)(byteOffset + (ulong)bytes.Length)) };
            Native.Interface.Unmap( 0, &written );
        }
    }

    /// <summary>Copies data from a readback buffer after its GPU work has completed.</summary>
    public void Read<T>( Span<T> destination, ulong byteOffset = 0 ) where T : unmanaged {
        if ( Memory != BufferMemory.Readback ) throw new InvalidOperationException( "Read requires a readback buffer." );
        Span<byte> bytes = MemoryMarshal.AsBytes( destination );
        ValidateRange( byteOffset, (ulong)bytes.Length );
        D3D12_RANGE readRange = new( ) { Begin = checked((nuint)byteOffset), End = checked((nuint)(byteOffset + (ulong)bytes.Length)) };
        void* pointer = null;
        Results.Check( Native.Interface.Map( 0, &readRange, &pointer ), "Resource.Map(read)" );
        try { new ReadOnlySpan<byte>( (byte*)pointer + checked((nint)byteOffset), bytes.Length ).CopyTo( bytes ); }
        finally { D3D12_RANGE written = default; Native.Interface.Unmap( 0, &written ); }
    }

    internal void ValidateRange( ulong offset, ulong size ) {
        if ( offset > SizeInBytes || size > SizeInBytes - offset ) throw new ArgumentOutOfRangeException( nameof(size), "The buffer range exceeds its allocation." );
    }
}

/// <summary>An owning descriptor allocation.</summary>
public sealed class DescriptorHeap : GraphicsObject {
    internal readonly ComOwner<ID3D12DescriptorHeap> Native;
    internal readonly uint Stride;
    /// <summary>The number of descriptors.</summary>
    public uint Capacity { get; }
    internal DescriptorHeap( ComOwner<ID3D12DescriptorHeap> owner, uint capacity, uint stride ) : base( owner ) {
        Native = owner; Capacity = capacity; Stride = stride;
    }
    internal override ComReference Retain( ) => Native.Retain( );
    internal D3D12_CPU_DESCRIPTOR_HANDLE CpuHandle( uint index ) {
        if ( index >= Capacity ) throw new ArgumentOutOfRangeException( nameof(index) );
        var handle = Native.Interface.GetCPUDescriptorHandleForHeapStart( );
        handle.ptr = checked(handle.ptr + (nuint)index * Stride);
        return handle;
    }
}

/// <summary>A render-target view associated with its descriptor heap and resource.</summary>
public readonly struct RenderTargetView {
    internal readonly DescriptorHeap Heap;
    internal readonly GpuResource Resource;
    internal readonly uint Index;
    internal RenderTargetView( DescriptorHeap heap, GpuResource resource, uint index ) { Heap = heap; Resource = resource; Index = index; }
    internal D3D12_CPU_DESCRIPTOR_HANDLE Handle => (Heap ?? throw new InvalidOperationException( "The view is uninitialized." )).CpuHandle( Index );
}

public sealed unsafe partial class GraphicsDevice {
    /// <summary>Allocates an explicit buffer. The caller manages resource-state transitions.</summary>
    public GpuResource CreateBuffer( ulong size, BufferMemory memory = BufferMemory.Default,
        ResourceState initialState = ResourceState.Common, bool unorderedAccess = false ) {
        if ( size == 0 || size > (ulong)nint.MaxValue ) throw new ArgumentOutOfRangeException( nameof(size) );
        if ( !Enum.IsDefined( memory ) ) throw new ArgumentOutOfRangeException( nameof(memory) );
        if ( memory == BufferMemory.Upload ) initialState = ResourceState.GenericRead;
        if ( memory == BufferMemory.Readback ) initialState = ResourceState.CopyDestination;
        if ( unorderedAccess && memory != BufferMemory.Default ) throw new ArgumentException( "UAV buffers require default memory." );
        D3D12_HEAP_PROPERTIES heap = new( ) { Type = (D3D12_HEAP_TYPE)memory, CreationNodeMask = 1, VisibleNodeMask = 1 };
        D3D12_RESOURCE_DESC description = new( ) {
            Dimension = D3D12_RESOURCE_DIMENSION.D3D12_RESOURCE_DIMENSION_BUFFER, Width = size, Height = 1,
            DepthOrArraySize = 1, MipLevels = 1, SampleDesc = new( ) { Count = 1 },
            Layout = D3D12_TEXTURE_LAYOUT.D3D12_TEXTURE_LAYOUT_ROW_MAJOR,
            Flags = unorderedAccess ? D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_ALLOW_UNORDERED_ACCESS : 0
        };
        Guid id = typeof(ID3D12Resource).GUID;
        nint pointer = 0;
        Results.Check( Native.Interface.CreateCommittedResource( &heap, 0, &description, (D3D12_RESOURCE_STATES)initialState, null, &id, (void**)&pointer ), "CreateCommittedResource" );
        return new( ComOwner<ID3D12Resource>.Adopt( pointer ), size, memory ) { DeviceIdentity = DeviceIdentity, SupportsUnorderedAccess = unorderedAccess };
    }
    /// <summary>Creates a non-shader-visible render-target descriptor heap.</summary>
    public DescriptorHeap CreateRenderTargetHeap( uint capacity ) {
        if ( capacity == 0 ) throw new ArgumentOutOfRangeException( nameof(capacity) );
        D3D12_DESCRIPTOR_HEAP_DESC description = new( ) { Type = D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_RTV, NumDescriptors = capacity };
        Guid id = typeof(ID3D12DescriptorHeap).GUID;
        Results.Check( Native.Interface.CreateDescriptorHeap( &description, &id, out nint pointer ), "CreateDescriptorHeap" );
        return new( ComOwner<ID3D12DescriptorHeap>.Adopt( pointer ), capacity,
            Native.Interface.GetDescriptorHandleIncrementSize( D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_RTV ) ) { DeviceIdentity = DeviceIdentity };
    }
    /// <summary>Writes an RTV and returns a view carrying its lifetime dependencies.</summary>
    public RenderTargetView CreateRenderTargetView( GpuResource resource, DescriptorHeap heap, uint index ) {
        RequireSameDevice( resource ); RequireSameDevice( heap );
        Native.Interface.CreateRenderTargetView( resource.Native.Interface, null, heap.CpuHandle( index ) );
        GC.KeepAlive( resource );
        return new( heap, resource, index );
    }
}
