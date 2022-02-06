using StockApp.Core.Wettbewerb;
using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Core.Wettbewerb.Zielbewerb;
using StockApp.UI.Stores;

namespace StockApp.UI.ViewModels;
public class WettbewerbsartViewModel : ViewModelBase
{
    private readonly ITurnierStore _turnierStore;

    public WettbewerbsartViewModel(ITurnierStore turnierStore)
    {
        _turnierStore = turnierStore;
    }

    public bool IsTeamChecked
    {
        get => _turnierStore.Turnier.Wettbewerb is ITeamBewerb;
        set
        {
            _turnierStore.Turnier.SetBewerb(Wettbewerbsart.Team);
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(IsZielChecked));
        }
    }

    public bool IsZielChecked
    {
        get => _turnierStore.Turnier.Wettbewerb is IZielBewerb;
        set
        {
            _turnierStore.Turnier.SetBewerb(Wettbewerbsart.Ziel);
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(IsTeamChecked));
        }
    }
}
