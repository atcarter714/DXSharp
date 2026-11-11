using System.Numerics;
using System.Runtime.InteropServices;

namespace FleetBattler;

[StructLayout( LayoutKind.Sequential )]
internal readonly record struct ShipInstance( Vector2 Center, Vector2 Heading, Vector4 Tint );

internal sealed class FleetSimulation {
    internal const int MaxShips = 10_000;
    internal const double StepSeconds = 1.0 / 60;
    private const float Pitch = 3.6f;
    private const float MaxSpeed = 36;
    private const float ApproachGain = 1.8f;
    private const float Response = 0.09f;
    private readonly Vector2[] _positions;
    private readonly Vector2[] _velocities;
    private readonly Vector2[] _headings;
    private readonly Vector2[] _offsets;
    private readonly Vector2[] _targets = new Vector2[2];
    private readonly int _split;
    private readonly float _targetLimit;

    internal FleetSimulation( int ships, uint seed ) {
        if ( ships is < 2 or > MaxShips ) throw new ArgumentOutOfRangeException( nameof(ships) );
        _positions = new Vector2[ships];
        _velocities = new Vector2[ships];
        _headings = new Vector2[ships];
        _offsets = new Vector2[ships];
        _split = (ships + 1) / 2;
        int side = (int)MathF.Ceiling( MathF.Sqrt( _split ) );
        BoardRadius = side * Pitch + 14;
        _targetLimit = BoardRadius - side * Pitch * 0.5f - 3;
        _targets[0] = new( -0.51f * BoardRadius, 0.24f * BoardRadius );
        _targets[1] = -_targets[0];
        for ( int i = 0; i < ships; ++i ) {
            int fleet = i < _split ? 0 : 1;
            int slot = fleet == 0 ? i : i - _split;
            var cell = new Vector2( slot % side, slot / side ) - new Vector2( (side - 1) * 0.5f );
            var jitter = new Vector2( NextUnit( ref seed ), NextUnit( ref seed ) ) - new Vector2( 0.5f );
            _offsets[i] = cell * Pitch + jitter * 0.42f;
            _positions[i] = _targets[fleet] + _offsets[i];
            _headings[i] = fleet == 0 ? Vector2.UnitX : -Vector2.UnitX;
        }
    }

    internal float BoardRadius { get; }
    internal int Count => _positions.Length;
    internal int Tick { get; private set; }
    internal ReadOnlySpan<Vector2> Positions => _positions;

    internal int FindFleet( Vector2 point ) {
        int closest = 0;
        float best = float.PositiveInfinity;
        for ( int i = 0; i < _positions.Length; ++i ) {
            float distance = Vector2.DistanceSquared( point, _positions[i] );
            if ( distance >= best ) continue;
            best = distance;
            closest = i;
        }
        return closest < _split ? 0 : 1;
    }

    internal void Move( int fleet, Vector2 destination ) {
        if ( fleet is < 0 or > 1 ) throw new ArgumentOutOfRangeException( nameof(fleet) );
        if ( !float.IsFinite( destination.X ) || !float.IsFinite( destination.Y ) )
            throw new ArgumentOutOfRangeException( nameof(destination) );
        _targets[fleet] = Vector2.Clamp( destination, new( -_targetLimit ), new( _targetLimit ) );
    }

    internal void Step( ) {
        for ( int i = 0; i < _positions.Length; ++i ) {
            var error = _targets[i < _split ? 0 : 1] + _offsets[i] - _positions[i];
            var desired = error * ApproachGain;
            float speedSquared = desired.LengthSquared( );
            if ( speedSquared > MaxSpeed * MaxSpeed )
                desired *= MaxSpeed / MathF.Sqrt( speedSquared );
            var velocity = Vector2.Lerp( _velocities[i], desired, Response );
            if ( error.LengthSquared( ) < 0.0001f && velocity.LengthSquared( ) < 0.0001f ) {
                _positions[i] += error;
                _velocities[i] = Vector2.Zero;
                continue;
            }
            _velocities[i] = velocity;
            _positions[i] += velocity * (float)StepSeconds;
            if ( velocity.LengthSquared( ) > 0.25f )
                _headings[i] = Vector2.Normalize( velocity );
        }
        ++Tick;
    }

    internal void FillInstances( Span<ShipInstance> destination, int selected ) {
        if ( destination.Length != _positions.Length ) throw new ArgumentException( "Instance span must match the fleet size.", nameof(destination) );
        var cyan = new Vector4( 0.62f, 1.45f, 2.15f, selected == 0 ? 1.2f : 0.87f );
        var orange = new Vector4( 2.55f, 0.79f, 0.32f, selected == 1 ? 1.2f : 0.87f );
        for ( int i = 0; i < destination.Length; ++i )
            destination[i] = new( _positions[i], _headings[i], i < _split ? cyan : orange );
    }

    internal ulong StateHash( ) {
        ulong hash = 14695981039346656037;
        Append( (uint)Tick );
        Append( (uint)Count );
        foreach ( var target in _targets ) AppendVector( target );
        for ( int i = 0; i < _positions.Length; ++i ) {
            AppendVector( _offsets[i] );
            AppendVector( _positions[i] );
            AppendVector( _velocities[i] );
            AppendVector( _headings[i] );
        }
        return hash;

        void AppendVector( Vector2 value ) {
            Append( BitConverter.SingleToUInt32Bits( value.X ) );
            Append( BitConverter.SingleToUInt32Bits( value.Y ) );
        }

        void Append( uint bits ) {
            for ( int i = 0; i < 32; i += 8 )
                hash = unchecked((hash ^ (byte)(bits >> i)) * 1099511628211);
        }
    }

    private static float NextUnit( ref uint state ) {
        // Integer mixing keeps seeded placement independent of System.Random.
        state = unchecked(state + 0x9E3779B9u);
        uint bits = state;
        bits = unchecked((bits ^ (bits >> 16)) * 0x85EBCA6Bu);
        bits = unchecked((bits ^ (bits >> 13)) * 0xC2B2AE35u);
        return ((bits ^ (bits >> 16)) >> 8) * (1f / 16_777_216);
    }
}
