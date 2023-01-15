using StockApp.Core.Turnier;
using StockApp.Lib.ViewModels;
using StockApp.UI.Stores;

namespace StockApp.UI.ViewModels;

public class TurnierViewModel : ViewModelBase
{
    private readonly ITurnierStore _turnierStore;

    public TurnierViewModel(ITurnierStore turnierStore)
    {
        this._turnierStore = turnierStore;
        this.EntryFeeViewModel = new EntryFeeViewModel(OrgaDaten.EntryFee);
        this.SchiedsrichterViewModel = new ExecutiveViewModel(OrgaDaten.Referee);
        this.WettbewerbsleiterViewModel = new ExecutiveViewModel(OrgaDaten.CompetitionManager);
        this.RechenbueroViewModel = new ExecutiveViewModel(OrgaDaten.ComputingOfficer);
    }

    public IOrgaDaten OrgaDaten => _turnierStore.Turnier.OrgaDaten;

    public EntryFeeViewModel EntryFeeViewModel { get; }
    public ExecutiveViewModel SchiedsrichterViewModel { get; }
    public ExecutiveViewModel RechenbueroViewModel { get; }
    public ExecutiveViewModel WettbewerbsleiterViewModel { get; }



}

public class TurnierDesignViewModel
{
    public IOrgaDaten OrgaDaten { get; } = new OrgaDaten() { DateOfTournament = System.DateTime.Now };
    public EntryFeeViewModel EntryFeeViewModel { get; } = new EntryFeeViewModel(new Startgebuehr(30.0, "dreissig"));
    public ExecutiveViewModel SchiedsrichterViewModel { get; } = new ExecutiveViewModel(new Schiedsrichter() { ClubName = "ESF Pfeifaweng", Name = "Hans Pfeiffer" });
    public ExecutiveViewModel RechenbueroViewModel { get; } = new ExecutiveViewModel(new Rechenbuero() { ClubName = "SV Rechnerei", Name = "Zahle Maximilian " });
    public ExecutiveViewModel WettbewerbsleiterViewModel { get; } = new ExecutiveViewModel(new Wettbewerbsleiter() { ClubName = "DJK Leiternei", Name = "Wolfgang Leit-Genau" });

}
