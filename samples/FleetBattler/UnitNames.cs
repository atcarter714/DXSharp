namespace FleetBattler;

internal sealed partial class FleetSimulation {
    private readonly string?[] _shipNames = new string?[FrontierCapacity];
    private readonly string?[] _bodyNames = new string?[CelestialSystem.BodyCount];
    private readonly Dictionary<int, string> _fleetNames = new( );

    internal string? ShipName( int ship ) => Frontier && Alive( ship ) ? _shipNames[ship] : null;
    internal string? FleetName( int squad ) => Frontier && ShipCount( squad ) > 0 ? _fleetNames.GetValueOrDefault( FleetId( squad ) ) : null;

    internal static string RoleName( HullRole role ) => role switch {
        HullRole.Scout => "SCOUT", HullRole.Miner => "CONSTRUCTOR", HullRole.Freighter => "FREIGHTER",
        HullRole.Colony => "COLONY SHIP", _ => "CORVETTE"
    };

    internal bool RenameShip( int ship, string name ) {
        if ( !Frontier || !Alive( ship ) || FactionOf( ship ) != FactionId.Sol || NormalizeName( name ) is not { } label ) return false;
        _shipNames[ship] = label;
        return true;
    }

    internal bool RenameFleet( int squad, string name ) {
        if ( !Frontier || (uint)squad >= (uint)SquadCount || !IsPlayerSquad( squad ) || ShipCount( squad ) == 0 ||
             NormalizeName( name ) is not { } label ) return false;
        _fleetNames[FleetId( squad )] = label;
        return true;
    }

    internal bool RenameBody( int body, string name ) {
        if ( !Frontier || (uint)body >= (uint)_bodies.Length || NormalizeName( name ) is not { } label ) return false;
        _bodyNames[body] = label;
        return true;
    }

    private static string? NormalizeName( string name ) {
        name = name.Trim( ).ToUpperInvariant( );
        if ( name.Length is < 1 or > 18 ) return null;
        foreach ( char ch in name )
            if ( ch is not (>= 'A' and <= 'Z' or >= '0' and <= '9' or ' ' or '-') ) return null;
        return name;
    }

    private void ReleaseFleetName( int squad ) {
        int id = FleetId( squad );
        for ( int i = 0; i < SquadCount; ++i )
            if ( ShipCount( i ) > 0 && FleetId( i ) == id ) return;
        _fleetNames.Remove( id );
    }
}
