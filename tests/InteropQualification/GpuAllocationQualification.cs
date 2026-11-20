using DXSharp;
using DXSharp.Graphics;
using DXSharp.Interop;
using Windows.Win32.Graphics.Direct3D12;

namespace InteropQualification;

internal static unsafe class GpuAllocationQualification {
    private static void Check( bool condition, string message ) {
        if ( !condition ) throw new InvalidOperationException( $"GPU allocations: {message}" );
    }
    private static void Invalid<T>( Action action ) where T : Exception {
        try { action( ); } catch ( T ) { return; }
        throw new InvalidOperationException( $"Expected {typeof(T).Name}." );
    }
    internal static void Run( GraphicsDevice device ) {
        using var descriptors = device.CreateDescriptorPool( 1 );
        using var ring = device.CreateUploadRing( 256 );
        using var range = descriptors.Allocate( );
        using var slice = ring.Allocate( 256 );
        slice.Write<uint>( [714, 1234, 0x12345678, 0xabcdef01] );
        Check( slice.Offset == 0 && slice.GpuAddress % 256 == 0, "upload address alignment" );
        Invalid<ArgumentOutOfRangeException>( ( ) => slice.Write<uint>( [1], 256 ) );
        Invalid<ArgumentOutOfRangeException>( ( ) => range.GetCpuHandle( 1 ) );
        Check( !descriptors.TryAllocate( 1, out _ ) && !ring.TryAllocate( 256, out _ ), "explicit bounded exhaustion" );

        using var queueA = device.CreateCommandQueue( );
        using var queueB = device.CreateCommandQueue( );
        using var allocatorA = device.CreateCommandAllocator( );
        using var allocatorB = device.CreateCommandAllocator( );
        using var commandsA = device.CreateCommandList( allocatorA );
        using var commandsB = device.CreateCommandList( allocatorB );
        using var readbackA = device.CreateBuffer( 16, BufferMemory.Readback );
        using var readbackB = device.CreateBuffer( 16, BufferMemory.Readback );
        Guid iid = typeof(ID3D12Fence).GUID;
        Results.Check( device.Native.Interface.CreateFence( 0, 0, &iid, out nint pointer ), "CreateFence(allocation gates)" );
        using var gate = ComOwner<ID3D12Fence>.Adopt( pointer );

        commandsA.CopyBuffer( slice, readbackA, 16 );
        commandsB.CopyBuffer( slice, readbackB, 16 );
        commandsA.RetainAllocation( range ); commandsA.RetainAllocation( range );
        commandsB.RetainAllocation( range );
        Invalid<InvalidOperationException>( ( ) => slice.Write<uint>( [0] ) );
        commandsA.Close( ); commandsB.Close( );

        // Hold both queues at independently releasable points, so the CPU observes
        // pending ranges deterministically even on fast WARP/NativeAOT runners.
        Results.Check( queueA.Native.Interface.Wait( gate.Interface, 1 ), "QueueA.Wait(allocation gate)" );
        Results.Check( queueB.Native.Interface.Wait( gate.Interface, 2 ), "QueueB.Wait(allocation gate)" );
        try {
            ulong a = queueA.Submit( commandsA ), b = queueB.Submit( commandsB );
            slice.Dispose( ); range.Dispose( );
            Check( ring.AvailableBytes == 0 && descriptors.AvailableCount == 0, "early owner disposal preserves submitted ranges" );
            Invalid<ObjectDisposedException>( ( ) => slice.Write<uint>( [0] ) );
            Invalid<TimeoutException>( ( ) => queueB.Wait( b, TimeSpan.Zero ) );
            Check( !ring.TryAllocate( 256, out _ ), "timed-out waits retain slices" );
            Results.Check( gate.Interface.Signal( 1 ), "Signal(first allocation gate)" );
            queueA.Wait( a );
            Check( ring.AvailableBytes == 0 && descriptors.AvailableCount == 0, "first queue retirement cannot reclaim second queue's use" );
            Results.Check( gate.Interface.Signal( 2 ), "Signal(second allocation gate)" );
            queueB.Wait( b );
            Check( ring.AvailableBytes == 256 && descriptors.AvailableCount == 1, "last consuming queue releases ranges" );
            Span<uint> values = stackalloc uint[4];
            readbackA.Read<uint>( values ); Check( values.SequenceEqual( [714u, 1234u, 0x12345678u, 0xabcdef01u] ), "first queue readback" );
            readbackB.Read<uint>( values ); Check( values.SequenceEqual( [714u, 1234u, 0x12345678u, 0xabcdef01u] ), "second queue readback" );
            Invalid<InvalidOperationException>( ( ) => queueA.Submit( commandsA ) );
        } finally { Results.Check( gate.Interface.Signal( 2 ), "Release allocation gates during cleanup" ); }

        // Exercise wrap/reuse with a real copy and readback on each iteration.
        Span<uint> wrappedValue = stackalloc uint[1];
        for ( uint iteration = 0; iteration < 12; ++iteration ) {
            allocatorA.Reset( ); commandsA.Reset( allocatorA );
            using var reused = ring.Allocate( 256 );
            reused.Write<uint>( [iteration + 700] );
            commandsA.CopyBuffer( reused, readbackA, 4 ); commandsA.Close( );
            reused.Dispose( ); queueA.Wait( queueA.Submit( commandsA ) );
            readbackA.Read<uint>( wrappedValue );
            Check( wrappedValue[0] == iteration + 700 && ring.AvailableBytes == 256, "wrapped upload data and retirement" );
        }

        allocatorA.Reset( ); commandsA.Reset( allocatorA );
        using ( var abandoned = ring.Allocate( 256 ) ) {
            commandsA.CopyBuffer( abandoned, readbackA, 4 ); commandsA.Close( ); abandoned.Dispose( );
            Check( ring.AvailableBytes == 0, "unsubmitted recording reserves its slice" );
            commandsA.Reset( allocatorA );
            Check( ring.AvailableBytes == 256, "reset discards an unsubmitted allocation lease" );
            commandsA.Close( );
        }
        using ( var active = descriptors.Allocate( ) ) {
            descriptors.Dispose( );
            _ = active.GetCpuHandle( );
            Invalid<ObjectDisposedException>( ( ) => descriptors.Allocate( ) );
        }
        using ( var active = ring.Allocate( 256 ) ) {
            ring.Dispose( ); active.Write<uint>( [1] );
            Invalid<ObjectDisposedException>( ( ) => ring.Allocate( 1 ) );
        }
        Console.WriteLine( "PASS: two blocked WARP queues, shared descriptor/upload leases, early disposal, timeout, reset, wraparound and numerical readback." );
    }
}
