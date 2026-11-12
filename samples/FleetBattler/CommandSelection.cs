using System.Numerics;

namespace FleetBattler;

internal enum SelectionMode { Replace, Add, Toggle }

internal sealed class CommandSelection {
    private readonly bool[] _ships;

    private readonly int[] _generations;

    internal CommandSelection( int capacity ) {
        _ships = new bool[capacity];
        _generations = new int[capacity];
    }

    internal ReadOnlySpan<bool> Ships => _ships;
    internal int Count { get; private set; }
    internal int Primary { get; private set; } = -1;
    internal FleetSelection Focus( FleetSimulation fleet ) =>
        Primary >= 0 ? new( fleet.SquadronOf( Primary ), Count == 1 ? Primary : -1 ) : new( 0 );

    internal FleetSelection? SurveyUnit( FleetSimulation fleet ) {
        if ( Primary < 0 ) return null;
        int squad = fleet.SquadronOf( Primary );
        if ( Count == 1 ) return new( squad, Primary );
        if ( Count != fleet.ShipCount( squad ) ) return null;
        for ( int ship = Primary + 1; ship < fleet.Count; ++ship )
            if ( _ships[ship] && fleet.SquadronOf( ship ) != squad ) return null;
        return new( squad );
    }

    internal void Clear( ) {
        Array.Clear( _ships );
        Count = 0;
        Primary = -1;
    }

    internal void Prune( FleetSimulation fleet ) {
        Count = 0;
        Primary = -1;
        for ( int i = 0; i < fleet.Count; ++i ) {
            if ( !_ships[i] ) continue;
            if ( !fleet.Alive( i ) || _generations[i] != fleet.ShipGeneration( i ) ) {
                _ships[i] = false;
                continue;
            }
            if ( Primary < 0 ) Primary = i;
            ++Count;
        }
    }

    internal void Click( FleetSimulation fleet, int ship, bool squad, SelectionMode mode ) {
        if ( squad && ship >= 0 ) { Squadron( fleet, fleet.SquadronOf( ship ), mode ); return; }
        if ( mode == SelectionMode.Replace ) Clear( );
        if ( ship >= 0 && fleet.Alive( ship ) && fleet.SideOf( ship ) == 0 ) Include( fleet, ship, mode );
        Prune( fleet );
    }

    internal void Squadron( FleetSimulation fleet, int squad, SelectionMode mode ) {
        if ( mode == SelectionMode.Replace ) Clear( );
        if ( (uint)squad < (uint)fleet.SquadCount ) {
            int id = fleet.FleetId( squad );
            for ( int i = 0; i < fleet.Count; ++i )
                if ( fleet.Alive( i ) && fleet.SideOf( i ) == 0 && fleet.FleetId( fleet.SquadronOf( i ) ) == id ) Include( fleet, i, mode );
        }
        Prune( fleet );
    }

    internal void All( FleetSimulation fleet ) {
        Clear( );
        for ( int i = 0; i < fleet.Count; ++i )
            if ( fleet.Alive( i ) && fleet.SideOf( i ) == 0 ) Include( fleet, i, SelectionMode.Add );
        Prune( fleet );
    }

    internal void Box( FleetSimulation fleet, Vector4 rectangle, Matrix4x4 viewProjection, Vector2 dimensions, SelectionMode mode, float interpolation = 1 ) {
        if ( mode == SelectionMode.Replace ) Clear( );
        for ( int i = 0; i < fleet.Count; ++i ) {
            if ( !fleet.Alive( i ) || fleet.SideOf( i ) != 0 ) continue;
            var pixel = Project( fleet.VisualPosition( i, interpolation ), viewProjection, dimensions );
            if ( pixel.X >= rectangle.X && pixel.X <= rectangle.Z && pixel.Y >= rectangle.Y && pixel.Y <= rectangle.W )
                Include( fleet, i, mode );
        }
        Prune( fleet );
    }

    internal static int Hit( FleetSimulation fleet, Vector2 pixel, Matrix4x4 viewProjection, Vector2 dimensions, int faction, float interpolation = 1 ) {
        float best = MathF.Pow( MathF.Max( 11, dimensions.Y * 0.013f ), 2 );
        int result = -1;
        for ( int i = 0; i < fleet.Count; ++i ) {
            if ( !fleet.Alive( i ) ) continue;
            int side = fleet.SideOf( i );
            if ( faction == -2 ? !fleet.IsHostile( (int)FactionId.Sol, side ) : faction >= 0 && side != faction ) continue;
            float distance = Vector2.DistanceSquared( pixel, Project( fleet.VisualPosition( i, interpolation ), viewProjection, dimensions ) );
            if ( distance >= best ) continue;
            best = distance;
            result = i;
        }
        return result;
    }

    internal bool Issue( FleetSimulation fleet, Vector2 destination, int orbit = -1, int hostile = -1 ) {
        if ( Count == 0 || fleet.Lost ) return false;
        bool issued = false;
        Span<int> selectedPerSquad = stackalloc int[fleet.SquadCount];
        selectedPerSquad.Clear( );
        Vector2 centroid = default;
        for ( int i = 0; i < fleet.Count; ++i ) {
            if ( !_ships[i] || !fleet.Alive( i ) ) continue;
            ++selectedPerSquad[fleet.SquadronOf( i )];
            centroid += fleet.Positions[i];
        }
        centroid /= Count;
        for ( int squad = 0; squad < selectedPerSquad.Length; ++squad ) {
            if ( selectedPerSquad[squad] == 0 || selectedPerSquad[squad] != fleet.ShipCount( squad ) ) continue;
            Order( new( squad ), fleet.Center( squad ) );
            selectedPerSquad[squad] = 0;
        }
        for ( int ship = 0; ship < fleet.Count; ++ship ) {
            if ( !_ships[ship] || !fleet.Alive( ship ) ) continue;
            int squad = fleet.SquadronOf( ship );
            if ( selectedPerSquad[squad] > 0 ) Order( new( squad, ship ), fleet.Positions[ship] );
        }

        return issued;

        void Order( FleetSelection selection, Vector2 center ) {
            if ( hostile >= 0 ) { issued |= fleet.Attack( selection, hostile ); }
            else if ( orbit >= 0 ) issued |= fleet.Orbit( selection, orbit );
            else {
                var offset = center - centroid;
                float length = offset.Length( );
                if ( length > 16 ) offset *= 16 / length;
                issued |= fleet.Move( selection, destination + offset );
            }
        }
    }

    private void Include( FleetSimulation fleet, int ship, SelectionMode mode ) {
        _ships[ship] = mode != SelectionMode.Toggle || !_ships[ship] || _generations[ship] != fleet.ShipGeneration( ship );
        _generations[ship] = fleet.ShipGeneration( ship );
    }

    internal static Vector2 Project( Vector2 world, Matrix4x4 matrix, Vector2 dimensions ) {
        var clip = Vector4.Transform( new Vector4( world.X, 0.1f, world.Y, 1 ), matrix );
        return (new Vector2( clip.X, -clip.Y ) / clip.W + Vector2.One) * dimensions * 0.5f;
    }
}
