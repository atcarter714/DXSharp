using System.Numerics;
using System.Runtime.InteropServices;

namespace FleetBattler;

[StructLayout( LayoutKind.Sequential )]
internal readonly record struct ShipInstance( Vector2 Center, Vector2 Heading, Vector4 Tint );

internal sealed class FleetSimulation {
    internal const int MaxShips = 10_000;
    internal const int DefaultShips = 72;
    internal const int WingSize = 6, WingCost = 60, WingGoal = 2;
    internal const int CaptureDuration = 90, IncomeInterval = 60, IncomeAmount = 15;
    internal const double StepSeconds = 1.0 / 60;
    private const int SquadsPerFaction = 6;
    private const float Pitch = 3.9f, MaxSpeed = 36, ApproachGain = 1.8f, Response = 0.09f;
    private readonly Vector2[] _positions, _velocities, _headings, _offsets;
    private readonly Squad[] _squads = new Squad[2 * SquadsPerFaction + WingGoal];
    private readonly bool _rivalPatrol;
    private readonly float _targetLimit;
    private int _amberFirst, _incomeTicks;
    private uint _seed;

    private struct Squad {
        internal int First, Count, Faction;
        internal Vector2 Home, Target, Center;
    }

    internal FleetSimulation( int ships, uint seed, bool rivalPatrol = false ) {
        if ( ships is < 2 or > MaxShips ) throw new ArgumentOutOfRangeException( nameof(ships) );
        _seed = seed;
        _rivalPatrol = rivalPatrol;
        int capacity = ships + WingGoal * WingSize;
        _positions = new Vector2[capacity];
        _velocities = new Vector2[capacity];
        _headings = new Vector2[capacity];
        _offsets = new Vector2[capacity];
        int cyan = (ships + 1) / 2;
        int largestSquad = (cyan + SquadsPerFaction - 1) / SquadsPerFaction;
        float formationRadius = MathF.Ceiling( MathF.Sqrt( largestSquad * 1.6f ) ) * Pitch;
        BoardRadius = MathF.Max( 78, formationRadius * 2.85f + 22 );
        _targetLimit = BoardRadius - formationRadius * 0.6f;
        Site = new( BoardRadius * 0.14f, -BoardRadius * 0.07f );
        SiteRadius = MathF.Max( 8, BoardRadius * 0.093f );
        AddFleet( cyan, 0 );
        _amberFirst = SquadCount;
        AddFleet( ships - cyan, 1 );
    }

    internal float BoardRadius { get; }
    internal Vector2 Site { get; }
    internal float SiteRadius { get; }
    internal Vector2 Shipyard => new( -0.59f * BoardRadius, 0.62f * BoardRadius );
    internal int Count { get; private set; }
    internal int Capacity => _positions.Length;
    internal int SquadCount { get; private set; }
    internal int Tick { get; private set; }
    internal int Owner { get; private set; } = -1;
    internal int Capturing { get; private set; } = -1;
    internal int CaptureTicks { get; private set; }
    internal bool Contested { get; private set; }
    internal int Ore { get; private set; }
    internal int WingsBuilt { get; private set; }
    internal ReadOnlySpan<Vector2> Positions => _positions.AsSpan( 0, Count );

    internal Vector2 Center( int squad ) => _squads[squad].Center;
    internal Vector2 Target( int squad ) => _squads[squad].Target;
    internal int ShipCount( int squad ) => _squads[squad].Count;

    internal int FindSquad( Vector2 point ) {
        int closest = 0;
        float best = float.PositiveInfinity;
        for ( int i = 0; i < SquadCount; ++i ) {
            if ( _squads[i].Faction != 0 ) continue;
            float distance = Vector2.DistanceSquared( point, _squads[i].Center );
            if ( distance >= best ) continue;
            best = distance;
            closest = i;
        }
        return closest;
    }

    internal void Move( int squad, Vector2 destination ) {
        if ( (uint)squad >= (uint)SquadCount ) throw new ArgumentOutOfRangeException( nameof(squad) );
        if ( !float.IsFinite( destination.X ) || !float.IsFinite( destination.Y ) )
            throw new ArgumentOutOfRangeException( nameof(destination) );
        _squads[squad].Target = Vector2.Clamp( destination, new( -_targetLimit ), new( _targetLimit ) );
    }

    internal int BuildWing( ) {
        if ( Ore < WingCost || WingsBuilt == WingGoal ) return -1;
        int index = SquadCount;
        var arrival = Shipyard + new Vector2( WingsBuilt * 13, 0 );
        AddSquad( WingSize, 0, arrival );
        Ore -= WingCost;
        ++WingsBuilt;
        return index;
    }

    internal void Step( ) {
        if ( _rivalPatrol && Tick >= 480 && (Tick - 480) % 600 == 0 ) {
            bool incoming = ((Tick - 480) / 600 & 1) == 0;
            Move( _amberFirst, incoming ? Site : _squads[_amberFirst].Home );
        }

        int cyan = 0, amber = 0;
        for ( int i = 0; i < SquadCount; ++i ) {
            ref var squad = ref _squads[i];
            var sum = Vector2.Zero;
            int end = squad.First + squad.Count;
            for ( int ship = squad.First; ship < end; ++ship ) {
                var error = squad.Target + _offsets[ship] - _positions[ship];
                var desired = error * ApproachGain;
                float speedSquared = desired.LengthSquared( );
                if ( speedSquared > MaxSpeed * MaxSpeed ) desired *= MaxSpeed / MathF.Sqrt( speedSquared );
                var velocity = Vector2.Lerp( _velocities[ship], desired, Response );
                if ( error.LengthSquared( ) < 0.0001f && velocity.LengthSquared( ) < 0.0001f ) {
                    _positions[ship] += error;
                    _velocities[ship] = Vector2.Zero;
                } else {
                    _velocities[ship] = velocity;
                    _positions[ship] += velocity * (float)StepSeconds;
                    if ( velocity.LengthSquared( ) > 0.25f ) _headings[ship] = Vector2.Normalize( velocity );
                }
                sum += _positions[ship];
            }
            squad.Center = sum / squad.Count;
            if ( Vector2.DistanceSquared( squad.Center, Site ) <= SiteRadius * SiteRadius ) {
                if ( squad.Faction == 0 ) ++cyan;
                else ++amber;
            }
        }
        AdvanceSite( cyan, amber );
        ++Tick;
    }

    private void AdvanceSite( int cyan, int amber ) {
        Contested = cyan != 0 && amber != 0;
        int dominant = cyan == amber ? -1 : cyan > amber ? 0 : 1;
        if ( dominant != -1 && dominant != Owner ) {
            if ( Capturing != dominant ) {
                CaptureTicks = 0;
                Capturing = dominant;
            }
            if ( ++CaptureTicks == CaptureDuration ) {
                Owner = dominant;
                Capturing = -1;
                CaptureTicks = 0;
                _incomeTicks = 0;
            }
        } else {
            Capturing = -1;
            CaptureTicks = 0;
        }

        if ( Owner != 0 || dominant != 0 || WingsBuilt == WingGoal ) {
            _incomeTicks = 0;
            return;
        }
        if ( ++_incomeTicks == IncomeInterval ) {
            Ore = Math.Min( Ore + IncomeAmount, WingCost * WingGoal );
            _incomeTicks = 0;
        }
    }

    internal void FillInstances( Span<ShipInstance> destination, int selected ) {
        if ( destination.Length != Count ) throw new ArgumentException( "Instance span must match the active ship count.", nameof(destination) );
        for ( int i = 0; i < SquadCount; ++i ) {
            ref readonly var squad = ref _squads[i];
            var tint = squad.Faction == 0 ? new Vector4( 0.66f, 1.55f, 2.25f, i == selected ? 1.55f : 1.04f )
                : new Vector4( 2.9f, 0.89f, 0.37f, 1.11f );
            int end = squad.First + squad.Count;
            for ( int ship = squad.First; ship < end; ++ship )
                destination[ship] = new( _positions[ship], _headings[ship], tint );
        }
    }

    internal ulong StateHash( ) {
        ulong hash = 14695981039346656037;
        Append( (uint)Tick ); Append( (uint)Count ); Append( (uint)SquadCount ); Append( _seed );
        Append( (uint)Owner ); Append( (uint)Capturing ); Append( (uint)CaptureTicks ); Append( Contested ? 1u : 0u );
        Append( (uint)Ore ); Append( (uint)WingsBuilt ); Append( (uint)_incomeTicks ); Append( _rivalPatrol ? 1u : 0u );
        for ( int i = 0; i < SquadCount; ++i ) {
            ref readonly var squad = ref _squads[i];
            Append( (uint)squad.Count ); Append( (uint)squad.Faction );
            AppendVector( squad.Home ); AppendVector( squad.Target ); AppendVector( squad.Center );
        }
        for ( int i = 0; i < Count; ++i ) {
            AppendVector( _offsets[i] ); AppendVector( _positions[i] );
            AppendVector( _velocities[i] ); AppendVector( _headings[i] );
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

    private void AddFleet( int ships, int faction ) {
        int squads = Math.Min( ships, SquadsPerFaction );
        for ( int i = 0; i < squads; ++i ) {
            float angle = -0.93f + 1.86f * (i + 0.5f) / squads;
            float radius = BoardRadius * (i % 2 == 0 ? 0.56f : 0.77f);
            var home = new Vector2( MathF.Cos( angle ) * (faction == 0 ? -1 : 1), MathF.Sin( angle ) ) * radius;
            AddSquad( ships / squads + (i < ships % squads ? 1 : 0), faction, home );
        }
    }

    private void AddSquad( int count, int faction, Vector2 center ) {
        var heading = faction == 0 ? Vector2.UnitX : -Vector2.UnitX;
        var right = new Vector2( -heading.Y, heading.X );
        int columns = (int)MathF.Ceiling( MathF.Sqrt( count * 1.6f ) );
        var mean = Vector2.Zero;
        for ( int i = 0; i < count; ++i ) {
            int row = count <= 10 ? (int)((MathF.Sqrt( 8 * i + 1 ) - 1) * 0.5f) : i / columns;
            float column = count <= 10 ? i - row * (row + 2) * 0.5f : i % columns + (row & 1) * 0.5f;
            var jitter = (new Vector2( NextUnit( ), NextUnit( ) ) - new Vector2( 0.5f )) * 0.28f;
            var local = new Vector2( column, row * 0.91f ) * Pitch + jitter;
            _offsets[Count + i] = right * local.X - heading * local.Y;
            mean += _offsets[Count + i];
        }
        mean /= count;
        for ( int i = Count; i < Count + count; ++i ) {
            _offsets[i] -= mean;
            _positions[i] = center + _offsets[i];
            _headings[i] = heading;
        }
        _squads[SquadCount++] = new( ) { First = Count, Count = count, Faction = faction, Home = center, Target = center, Center = center };
        Count += count;
    }

    private float NextUnit( ) {
        _seed = unchecked(_seed + 0x9E3779B9u);
        uint bits = _seed;
        bits = unchecked((bits ^ (bits >> 16)) * 0x85EBCA6Bu);
        bits = unchecked((bits ^ (bits >> 13)) * 0xC2B2AE35u);
        return ((bits ^ (bits >> 16)) >> 8) * (1f / 16_777_216);
    }
}
