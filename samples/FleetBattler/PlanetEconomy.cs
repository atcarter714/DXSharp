using System.Numerics;

namespace FleetBattler;

internal enum PlanetFacility { Mine, Lab }

internal readonly record struct PlanetStatus( int Survey, int MineRemaining, int LabRemaining ) {
    internal bool Surveyed => Survey >= FleetSimulation.SurveyDuration;
}

internal sealed partial class FleetSimulation {
    internal const int SurveyDuration = 120, MineCost = 35, LabCost = 45, UpgradeCost = 30;
    internal const int FacilityDuration = 180;
    private int _surveyRequests;
    private readonly int[] _survey = new int[CelestialSystem.BodyCount];
    private readonly int[] _mines = new int[CelestialSystem.BodyCount];
    private readonly int[] _labs = new int[CelestialSystem.BodyCount];

    internal int Science { get; private set; }
    internal bool WeaponUpgrade { get; private set; }

    internal PlanetStatus Planet( int body ) =>
        new( _survey[body], Remaining( _mines[body] ), Remaining( _labs[body] ) );

    private int Remaining( int deadline ) => deadline == 0 ? -1 : Math.Max( 0, deadline - Tick );

    internal void BeginSurvey( int body ) {
        if ( body > 0 && body < CelestialSystem.BodyCount ) _surveyRequests |= 1 << body;
    }

    internal bool BuildFacility( int body, PlanetFacility facility ) {
        if ( body <= 0 || body >= CelestialSystem.BodyCount || !Planet( body ).Surveyed || Lost ) return false;
        var projects = facility == PlanetFacility.Mine ? _mines : _labs;
        int cost = facility == PlanetFacility.Mine ? MineCost : LabCost;
        if ( projects[body] != 0 || Ore < cost ) return false;
        Ore -= cost;
        projects[body] = Tick + FacilityDuration;
        return true;
    }

    internal bool ResearchWeapons( ) {
        if ( Science < UpgradeCost || WeaponUpgrade || Lost ) return false;
        Science -= UpgradeCost;
        WeaponUpgrade = true;
        return true;
    }

    private void AdvancePlanets( ) {
        for ( int body = 1; body < _bodies.Length; ++body ) {
            if ( (_surveyRequests & (1 << body)) != 0 && _survey[body] < SurveyDuration ) {
                bool present = false;
                float range = OrbitRadius( body, 1 ) + 8;
                for ( int i = 0; i < Count && !present; ++i ) {
                    if ( !Alive( i ) || SideOf( i ) != 0 ) continue;
                    present = Vector2.DistanceSquared( _positions[i], _bodies[body].Center ) <= range * range;
                }
                if ( !present ) _survey[body] = 0;
                else if ( ++_survey[body] == SurveyDuration ) Ore = Math.Min( 999, Ore + 40 );
            }
            if ( (Tick + 1) % IncomeInterval != 0 ) continue;
            if ( _mines[body] > 0 && _mines[body] <= Tick + 1 ) Ore = Math.Min( 999, Ore + 8 );
            if ( _labs[body] > 0 && _labs[body] <= Tick + 1 ) Science = Math.Min( 999, Science + 6 );
        }
    }
}
