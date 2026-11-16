using System.Numerics;

namespace FleetBattler;

internal sealed partial class FleetSimulation {
    private const float PirateControlRadius = 26;
    internal int PiracyCycles { get; private set; }
    internal int PirateOreLost { get; private set; }
    internal int PirateEnergyLost { get; private set; }
    internal int LastPiracyBody { get; private set; }
    internal int PirateControlledBody { get; private set; } = -1;

    private void AdvancePiracy( ) {
        if ( !_battle || Tick % IncomeInterval != 0 ) return;
        PirateControlledBody = -1;
        if ( FactionCount( FactionId.Raiders ) == 0 ) return;
        for ( int body = 1; body < _bodies.Length; ++body ) {
            int income = ExtractionIncome( body, Tick );
            if ( income == 0 ) continue;
            var position = ExtractionPosition( body );
            int pirates = 0, escorts = 0;
            for ( int ship = 0; ship < Count; ++ship ) {
                if ( !Alive( ship ) || Vector2.DistanceSquared( _positions[ship], position ) > PirateControlRadius * PirateControlRadius ) continue;
                if ( FactionOf( ship ) == FactionId.Raiders ) ++pirates;
                else if ( FactionOf( ship ) == FactionId.Sol && IsWarship( Role( SquadronOf( ship ) ) ) ) escorts += NavalWeight( Role( SquadronOf( ship ) ) );
            }
            if ( pirates <= escorts ) continue;
            PirateControlledBody = body;
            int ore = Math.Min( Ore, Math.Max( 1, income / 2 ) );
            int energy = Math.Min( Energy, pirates );
            if ( ore == 0 && energy == 0 ) continue;
            Ore -= ore;
            Energy -= energy;
            LastPiracyBody = body;
            PirateOreLost += ore;
            PirateEnergyLost += energy;
            ++PiracyCycles;
        }
    }

    private void PlotPirateRaid( int squad ) {
        ref var raider = ref _squads[squad];
        var center = Center( squad );
        int strength = ShipCount( squad );
        int escorts = LocalEscortStrength( center, 42, out var threat );
        var target = Vector2.Zero;
        float score = float.NegativeInfinity;
        if ( escorts < strength ) {
            for ( int body = 1; body < _bodies.Length; ++body ) {
                int income = ExtractionIncome( body, Tick );
                if ( income == 0 ) continue;
                var site = ExtractionPosition( body );
                int guard = LocalEscortStrength( site, 36, out _ );
                if ( guard >= strength ) continue;
                float value = income * 4 - guard * 12 - Vector2.Distance( center, site ) * 0.08f;
                if ( value > score ) { target = site; score = value; }
            }
        }
        if ( !float.IsNegativeInfinity( score ) ) {
            raider.Stance = FleetStance.Normal;
            if ( raider.Order.Enemy != 0 || Vector2.DistanceSquared( raider.Order.Target, target ) > 64 ) Move( squad, target );
            return;
        }

        raider.Stance = FleetStance.Passive;
        var away = center - (escorts > 0 ? threat : _bodies[0].Center);
        var direction = away.LengthSquared( ) < 0.01f ? Vector2.UnitX : Vector2.Normalize( away );
        // An undefended escape vector stays on the chart and avoids diving through the patrol.
        target = Vector2.Clamp( center + direction * 95, new( -NavigationRadius + 4 ), new( NavigationRadius - 4 ) );
        if ( Vector2.DistanceSquared( center, target ) < 144 && escorts == 0 ) {
            for ( int ship = raider.First; ship < raider.First + raider.Count; ++ship ) {
                if ( !Alive( ship ) ) continue;
                _hull[ship] = 0;
                --raider.Alive;
                --_factionCount[(int)FactionId.Raiders];
            }
        } else if ( raider.Order.Enemy != 0 || Vector2.DistanceSquared( raider.Order.Target, target ) > 144 )
            Move( squad, target );
    }

    internal int LocalEscortStrength( Vector2 position, float radius, out Vector2 nearest ) {
        nearest = default;
        float closest = float.PositiveInfinity;
        int weight = 0;
        for ( int ship = 0; ship < Count; ++ship ) {
            if ( !Alive( ship ) || FactionOf( ship ) != FactionId.Sol ) continue;
            int strength = NavalWeight( Role( SquadronOf( ship ) ) );
            if ( strength == 0 ) continue;
            float distance = Vector2.DistanceSquared( position, _positions[ship] );
            if ( distance > radius * radius ) continue;
            weight += strength;
            if ( distance < closest ) { closest = distance; nearest = _positions[ship]; }
        }
        return weight;
    }

    private Vector2 ExtractionPosition( int body ) => SitePositionAt( body,
        Remaining( _skimmers[body] ) == 0 ? PlanetFacility.Skimmer : PlanetFacility.Mine, Tick, _bodies[body].Center );
}
