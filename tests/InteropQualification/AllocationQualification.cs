using DXSharp.Graphics;

namespace InteropQualification;

internal static class AllocationQualification {
    private sealed class Backing : IDisposable {
        internal int Disposals;
        public void Dispose( ) => ++Disposals;
    }
    private static void Check( bool condition, string message ) {
        if ( !condition ) throw new InvalidOperationException( $"Allocation qualification: {message}" );
    }
    private static void Invalid<T>( Action action ) where T : Exception {
        try { action( ); } catch ( T ) { return; }
        throw new InvalidOperationException( $"Expected {typeof(T).Name}." );
    }
    internal static void Run( ) {
        var backing = new Backing( );
        using ( var arena = new AllocationArena( 256, backing ) ) {
            Check( arena.TryReserve( 256, 256, out ulong offset, out var owner ) && offset == 0, "full aligned reservation" );
            using var queueA = owner!.Acquire( );
            var queueB = owner.Acquire( );
            lock ( owner.Gate ) Invalid<InvalidOperationException>( owner.RequireWritable );
            owner.Dispose( ); owner.Dispose( );
            Check( arena.Available == 0 && !arena.TryReserve( 1, 1, out _, out _ ), "owner disposal cannot reclaim a recorded range" );
            queueA.Dispose( );
            Check( arena.Available == 0, "first queue cannot reclaim another queue's range" );
            arena.Dispose( );
            Check( backing.Disposals == 0, "pool disposal preserves live allocation backing" );
            queueB.Dispose( ); queueB.Dispose( );
            Check( backing.Disposals == 1 && arena.Available == 256, "final retirement releases range and backing once" );
            Invalid<ObjectDisposedException>( ( ) => arena.TryReserve( 1, 1, out _, out _ ) );
            Invalid<ObjectDisposedException>( ( ) => owner.Acquire( ) );
        }
        Check( backing.Disposals == 1, "repeat pool disposal" );

        using ( var arena = new AllocationArena( 512, new Backing( ) ) ) {
            Invalid<ArgumentOutOfRangeException>( ( ) => arena.TryReserve( 0, 1, out _, out _ ) );
            Invalid<ArgumentOutOfRangeException>( ( ) => arena.TryReserve( 513, 1, out _, out _ ) );
            Invalid<ArgumentOutOfRangeException>( ( ) => arena.TryReserve( 1, 3, out _, out _ ) );
            Check( arena.TryReserve( 17, 1, out _, out var first ), "unaligned-sized slice" ); first!.Dispose( );
            Check( arena.TryReserve( 512, 512, out ulong offset, out var whole ) && offset == 0, "wrap across a cursor in a fully free buffer" ); whole!.Dispose( );
            Check( arena.TryReserve( 4, 256, out offset, out var live ) && offset % 256 == 0, "constant-buffer alignment" );
            using ( live!.Acquire( ) ) { }
            lock ( live.Gate ) live.RequireWritable( );
            live.Dispose( );
        }

        // Compare allocation success and every occupied byte against an independent bitmap.
        const int capacity = 4096;
        using ( var arena = new AllocationArena( capacity, new Backing( ) ) ) {
            var used = new bool[capacity];
            var live = new List<(int Offset, int Size, AllocationLifetime Owner)>( );
            var random = new Random( 714 );
            for ( int step = 0; step < 20_000; ++step ) {
                if ( live.Count > 0 && random.Next( 3 ) == 0 ) {
                    int index = random.Next( live.Count ); var item = live[index]; live.RemoveAt( index );
                    item.Owner.Dispose( ); Array.Fill( used, false, item.Offset, item.Size );
                } else {
                    int size = random.Next( 1, 385 ), alignment = 1 << random.Next( 0, 10 );
                    bool possible = false;
                    for ( int candidate = 0; candidate <= capacity - size; candidate += alignment )
                        if ( used.AsSpan( candidate, size ).IndexOf( true ) < 0 ) { possible = true; break; }
                    bool success = arena.TryReserve( (ulong)size, (ulong)alignment, out ulong offset, out var owner );
                    Check( success == possible, "reservation result agrees with bitmap" );
                    if ( success ) {
                        Check( offset % (ulong)alignment == 0 && used.AsSpan( (int)offset, size ).IndexOf( true ) < 0, "no overlap and correct alignment" );
                        Array.Fill( used, true, (int)offset, size ); live.Add( ((int)offset, size, owner!) );
                    }
                }
                Check( arena.Available == (ulong)used.Count( value => !value ), "available-byte accounting" );
            }
            foreach ( var item in live ) item.Owner.Dispose( );
            Check( arena.Available == capacity, "all bytes reclaimed" );
            Check( arena.TryReserve( capacity, 512, out _, out var whole ), "coalesces after out-of-order retirement" );
            whole!.Dispose( );
        }
        Console.WriteLine( "PASS: allocation lifetime, independent queue leases, alignment, exhaustion, wraparound, and 20,000 bitmap-checked operations." );
    }
}
