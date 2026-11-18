using System.Numerics;

namespace FleetBattler;

internal enum FactionId { Sol, Raiders, Merchants, Rival, Compact, Consortium, Count }
internal enum Disposition { Allied, Neutral, Hostile }

internal static class Factions {
    internal static string Name( FactionId id ) => id switch {
        FactionId.Sol => "SOL AUTHORITY", FactionId.Raiders => "AMBER RAIDERS",
        FactionId.Merchants => "FREE MERCHANTS", FactionId.Rival => "RIVAL COMMAND",
        FactionId.Compact => "CERES COMPACT", _ => "SURVEY CONSORTIUM"
    };

    internal static Disposition Attitude( FactionId from, FactionId toward ) => from == toward ? Disposition.Allied
        : from == FactionId.Raiders || toward == FactionId.Raiders ||
          from is FactionId.Sol && toward is FactionId.Rival || from is FactionId.Rival && toward is FactionId.Sol
            ? Disposition.Hostile : Disposition.Neutral;

    internal static Vector4 ShipTint( FactionId id ) => id switch {
        FactionId.Sol => new( 0.66f, 1.55f, 2.25f, 1.04f ),
        FactionId.Merchants => new( 0.82f, 1.42f, 0.97f, 1 ),
        FactionId.Compact => new( 1.35f, 1.22f, 0.68f, 1 ),
        FactionId.Consortium => new( 1.24f, 1.0f, 1.64f, 1 ),
        _ => new( 2.9f, 0.89f, 0.37f, 1.11f )
    };
}
