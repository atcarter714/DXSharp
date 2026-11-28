using System.Numerics;

namespace FleetBattler;

internal sealed partial class FleetSimulation {
    private const float PirateControlRadius = 26;
    internal int PiracyCycles { get; private set; }
    internal int PirateOreLost { get; private set; }
    internal int PirateEnergyLost { get; private set; }
    internal int LastPiracyBody { get; private set; }
    internal int PirateControlledBody { get; private set; } = -1;
    internal int BlockadedSites => BitOperations.PopCount( _pirateControl );
    internal int BlockadeOrePerCycle { get; private set; }
    internal int BlockadeEnergyPerCycle { get; private set; }
    private ulong _pirateControl;
    internal bool IsPirateControlled( int body ) => (_pirateControl & (1ul << body)) != 0;
    private static ReadOnlySpan<PlanetFacility> RaidTargets =>
        [PlanetFacility.Skimmer, PlanetFacility.Mine, PlanetFacility.Depot, PlanetFacility.Lab, PlanetFacility.Yard, PlanetFacility.Solar,
            PlanetFacility.Habitat, PlanetFacility.Soletta];

    private void AdvancePiracy( ) {
        if ( !_battle ) return;
        if ( FactionCount( FactionId.Raiders ) != 0 && Tick % EconomyInterval != 0 ) return;
        PirateControlledBody = -1;
        _pirateControl = 0;
        BlockadeOrePerCycle = BlockadeEnergyPerCycle = 0;
        if ( FactionCount( FactionId.Raiders ) == 0 ) return;
        for ( int body = 1; body < _bodies.Length; ++body ) {
            if ( RaidTarget( body ) is not { } facility ) continue;
            int income = ExtractionIncome( body, Tick );
            var position = SitePositionAt( body, facility, Tick, _bodies[body].Center );
            int pirates = 0, escorts = 0, attacker = -1;
            for ( int ship = 0; ship < Count; ++ship ) {
                if ( !Alive( ship ) || Vector2.DistanceSquared( _positions[ship], position ) > PirateControlRadius * PirateControlRadius ) continue;
                if ( FactionOf( ship ) == FactionId.Raiders ) { ++pirates; attacker = ship; }
                else if ( FactionOf( ship ) == FactionId.Sol && IsWarship( Role( SquadronOf( ship ) ) ) ) escorts += NavalWeight( Role( SquadronOf( ship ) ) );
            }
            if ( pirates <= escorts ) continue;
            PirateControlledBody = body;
            _pirateControl |= 1ul << body;
            if ( FrontierProgression ) {
                DamageSite( body, facility, 8 + 3 * pirates );
                Emit( _positions[attacker], position, (int)FactionId.Raiders, burst: false, EffectStyle.Thermal );
                LastEngagementTick = Tick;
            }
            int levy = Math.Max( 1, income / 2 );
            BlockadeOrePerCycle += levy;
            BlockadeEnergyPerCycle += pirates;
            int ore = Math.Min( Ore, levy );
            int energy = Math.Min( Energy, pirates );
            if ( ore == 0 && energy == 0 && !FrontierProgression ) continue;
            Ore -= ore;
            Energy -= energy;
            LastPiracyBody = body;
            PirateOreLost += ore;
            PirateEnergyLost += energy;
            ++PiracyCycles;
        }
    }

    private PlanetFacility? RaidTarget( int body ) {
        if ( FrontierProgression && !PirateRaidBody( body ) ) return null;
        PlanetFacility? disabled = null;
        foreach ( var facility in RaidTargets ) {
            if ( SiteRemaining( body, facility ) != 0 || (!FrontierProgression && facility is not (PlanetFacility.Mine or PlanetFacility.Skimmer)) ) continue;
            if ( SiteIntegrity( body, facility ) > 0 ) return facility;
            disabled ??= facility;
        }
        return disabled;
    }

    private bool PirateRaidBody( int body ) => !Solar || body != Homeworld && SolarCatalog.Get( body ).Parent != Homeworld;

    private void PlotPirateRaid( int squad ) {
        ref var raider = ref _squads[squad];
        var center = Center( squad );
        int strength = ShipCount( squad );
        int escorts = LocalEscortStrength( center, 42, out var threat );
        int targetBody = -1;
        PlanetFacility targetFacility = default;
        float score = float.NegativeInfinity;
        if ( escorts < strength ) {
            for ( int body = 1; body < _bodies.Length; ++body ) {
                if ( RaidTarget( body ) is not { } facility ) continue;
                int income = ExtractionIncome( body, Tick );
                var site = SitePositionAt( body, facility, Tick, _bodies[body].Center );
                int guard = LocalEscortStrength( site, 36, out _ );
                if ( guard >= strength ) continue;
                float value = income * 4 - guard * 12 - Vector2.Distance( center, site ) * 0.08f;
                if ( value > score ) { targetBody = body; targetFacility = facility; score = value; }
            }
        }
        if ( targetBody >= 0 ) {
            raider.Stance = FleetStance.Normal;
            var orbit = Facility( targetFacility ).Layer == SiteLayer.Orbit ? FacilityOrbit( targetBody, targetFacility ) : default;
            // Hold a body-relative orbit beside the facility. Replot only when the target changes.
            orbit.Y += .27f;
            if ( raider.Order.Body != targetBody + 1 || raider.Order.Enemy != 0 ||
                 (orbit.X > 0 && (raider.Order.Radius != orbit.X || raider.Order.Phase != orbit.Y)) ) {
                var unit = new FleetSelection( squad );
                SetOrder( unit, PlotTransfer( unit, targetBody, false, fixedBerth: orbit.X > 0 ? orbit : null ) );
            }
            return;
        }

        raider.Stance = FleetStance.Passive;
        var away = center - (escorts > 0 ? threat : _bodies[0].Center);
        var direction = away.LengthSquared( ) < 0.01f ? Vector2.UnitX : Vector2.Normalize( away );
        // An undefended escape vector stays on the chart and avoids diving through the patrol.
        var target = Vector2.Clamp( center + direction * 95, new( -NavigationRadius + 4 ), new( NavigationRadius - 4 ) );
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

}
