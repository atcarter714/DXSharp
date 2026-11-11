using System.Numerics;
using System.Runtime.InteropServices;
using FleetBattler;

namespace InteropQualification;

internal static class FleetQualification {
    internal static void Run( ) {
        var first = Replay( 37, 0 );
        var second = Replay( 37, 0 );
        Require( first == second, "the same seed and orders reproduce all checkpoints" );
        Require( first != Replay( 38, 0 ), "a different seed changes the state" );
        var changed = Replay( 37, 1 );
        Require( first.BeforeOrder == changed.BeforeOrder && first.AfterOrder != changed.AfterOrder,
            "a changed order first diverges after its tick" );

        var fleet = new FleetSimulation( 129, 37 );
        var previous = fleet.Positions.ToArray( );
        var instances = new ShipInstance[fleet.Count];
        int split = (fleet.Count + 1) / 2;
        Require( Marshal.SizeOf<ShipInstance>( ) == FleetDraw.InstanceStride, "instance stream ABI" );
        Require( fleet.FindFleet( previous[0] ) == 0 && fleet.FindFleet( previous[^1] ) == 1, "fleet selection" );
        fleet.Move( 0, new( 8, -20 ) );
        for ( int i = 0; i < 360; ++i ) fleet.Step( );
        fleet.FillInstances( instances, 0 );
        var moved = fleet.Positions[0] - previous[0];
        Require( moved.X > 20 && moved.Y < -15, "move order reaches the other side of the board" );
        for ( int i = 0; i < fleet.Count; ++i ) {
            var instance = instances[i];
            Require( instance.Center == fleet.Positions[i] && MathF.Abs( instance.Heading.LengthSquared( ) - 1 ) < 1e-5f,
                "instance data preserves position and a normalized heading" );
            if ( i < split ) {
                Require( Vector2.DistanceSquared( fleet.Positions[i] - previous[i], moved ) < 1e-5f,
                    "the ordered fleet preserves formation spacing" );
                Require( instance.Tint.Z > instance.Tint.X && instance.Tint.W > 1, "selected fleet tint" );
            } else {
                Require( fleet.Positions[i] == previous[i] && instance.Tint.X > instance.Tint.Z && instance.Tint.W < 1,
                    "the other fleet stays in place with its own tint" );
            }
        }

        long start = GC.GetAllocatedBytesForCurrentThread( );
        for ( int i = 0; i < 120; ++i ) {
            fleet.Step( );
            fleet.FillInstances( instances, 1 );
        }
        Require( GC.GetAllocatedBytesForCurrentThread( ) == start, "steady-state stepping and packing allocate zero bytes" );
        Console.WriteLine( $"PASS: fleet replay, orders, formation spacing, instance ABI, and zero-allocation updates ({first.AfterOrder:X16})." );
    }

    private static (ulong BeforeOrder, ulong AfterOrder) Replay( uint seed, float orderDelta ) {
        var fleet = new FleetSimulation( 1025, seed );
        fleet.Move( 1, new( 11, 20 ) );
        for ( int i = 0; i < 180; ++i ) fleet.Step( );
        ulong checkpoint = fleet.StateHash( );
        fleet.Move( 0, new( 15 + orderDelta, -35 ) );
        for ( int i = 0; i < 480; ++i ) fleet.Step( );
        Require( fleet.Tick == 660, "fixed tick count" );
        return (checkpoint, fleet.StateHash( ));
    }

    private static void Require( bool condition, string name ) {
        if ( !condition ) throw new InvalidOperationException( $"Fleet qualification failed: {name}." );
    }
}
