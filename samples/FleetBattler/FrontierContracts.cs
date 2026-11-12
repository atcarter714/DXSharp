using System.Numerics;

namespace FleetBattler;

internal enum MerchantDecision { Pending, Trade, Research }

internal sealed partial class FleetSimulation {
    internal MerchantDecision MerchantAgreement { get; private set; }
    internal int MerchantShipments { get; private set; }
    internal bool HasMerchantOffer => Established && _surveyedProspects > 0;

    internal static ResourceCost ContractCost( MerchantDecision decision ) => decision switch {
        MerchantDecision.Trade => new( 0, 12 ),
        MerchantDecision.Research => new( 15, 0 ),
        _ => throw new ArgumentOutOfRangeException( nameof(decision) )
    };

    internal bool CanAcceptContract( MerchantDecision decision ) => HasMerchantOffer && MerchantAgreement == MerchantDecision.Pending &&
        decision is MerchantDecision.Trade or MerchantDecision.Research && CanAfford( ContractCost( decision ) );

    internal bool AcceptContract( MerchantDecision decision ) {
        if ( !CanAcceptContract( decision ) ) return false;
        Spend( ContractCost( decision ) );
        MerchantAgreement = decision;
        if ( decision == MerchantDecision.Trade ) {
            Refund( new( 25, 0 ) );
            var arrival = _bodies[Homeworld].Center + new Vector2( 19, 6 );
            Park( AddSquad( 1, (int)FactionId.Merchants, arrival, HullRole.Freighter ), Homeworld );
        } else Refund( new( 0, 0, 18 ) );
        return true;
    }

    internal Disposition Relationship( FactionId from, FactionId to ) =>
        MerchantAgreement == MerchantDecision.Trade &&
        (from == FactionId.Sol && to == FactionId.Merchants || from == FactionId.Merchants && to == FactionId.Sol)
            ? Disposition.Allied : Factions.Attitude( from, to );

    private void AdvanceContracts( ) {
        if ( MerchantAgreement != MerchantDecision.Trade || (Tick + 1) % (IncomeInterval * 5) != 0 ||
             Energy < 5 || Ore > 987 ) return;
        Energy -= 5;
        Ore += 12;
        ++MerchantShipments;
    }
}
