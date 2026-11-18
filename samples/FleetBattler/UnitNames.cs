namespace FleetBattler;

internal sealed partial class FleetSimulation {
    private readonly string?[] _shipNames = new string?[FrontierCapacity];
    private readonly string?[] _bodyNames = new string?[CelestialSystem.BodyCount];
    private readonly Dictionary<int, string> _fleetNames = new( );

    internal string? ShipName( int ship ) => Frontier && Alive( ship ) ? _shipNames[ship] : null;
    internal string? FirstShipName( int squad ) {
        for ( int i = _squads[squad].First; i < _squads[squad].First + _squads[squad].Count; ++i )
            if ( Alive( i ) ) return ShipName( i );
        return null;
    }

    internal string? FleetName( int squad ) => Frontier && ShipCount( squad ) > 0 ? _fleetNames.GetValueOrDefault( FleetId( squad ) ) : null;

    private int _nextShipName;
    internal static string RoleName( HullRole role ) => GameCatalog.Hull( role ).Name;

    private string AssignShipName( HullRole role ) {
        var names = GameCatalog.ShipNames( role );
        int serial = _nextShipName++;
        for ( int i = 0; i < names.Length; ++i ) {
            string candidate = names[(serial + i) % names.Length];
            if ( Array.IndexOf( _shipNames, candidate ) < 0 ) return candidate;
        }
        string stem = names[serial % names.Length];
        string suffix = $" {serial + 1}";
        return stem[..Math.Min( stem.Length, 18 - suffix.Length )] + suffix;
    }

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

    internal bool RenameFleetById( int id, string name ) {
        for ( int squad = 0; squad < SquadCount; ++squad )
            if ( FleetId( squad ) == id && ShipCount( squad ) > 0 ) return RenameFleet( squad, name );
        return false;
    }

    internal bool RenameBody( int body, string name ) {
        if ( !Frontier || (uint)body >= (uint)_bodies.Length || NormalizeName( name ) is not { } label ) return false;
        _bodyNames[body] = label;
        return true;
    }

    internal static string? NormalizeName( string name ) {
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
