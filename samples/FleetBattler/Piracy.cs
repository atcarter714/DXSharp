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

    private Vector2 ExtractionPosition( int body ) => SitePositionAt( body,
        Remaining( _skimmers[body] ) == 0 ? PlanetFacility.Skimmer : PlanetFacility.Mine, Tick, _bodies[body].Center );
}
