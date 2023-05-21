using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.ViewModels;

namespace StockApp.UI.ViewModels;

public class TeamBewerbViewModel : ViewModelBase
{
    private readonly ITeamBewerb _teamBewerb;

    public ITeamBewerb TeamBewerb => _teamBewerb;

    public TeamBewerbViewModel()
    {

    }
    public TeamBewerbViewModel(ITeamBewerb teamBewerb)
    {
        _teamBewerb = teamBewerb;
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {

            }
            _disposed = true;
        }
    }

    public string Gruppenname
    {
        get => _teamBewerb.Gruppenname;
        set
        {
            if (_teamBewerb.Gruppenname == value) return;
            _teamBewerb.Gruppenname = value;
            RaisePropertyChanged();
        }
    }

    public int GameGroup
    {
        get => _teamBewerb.SpielGruppe;
        set
        {
            if (_teamBewerb.SpielGruppe == value) return;
            _teamBewerb.SpielGruppe = value;
            RaisePropertyChanged();
        }
    }

    public bool IsSplitGruppe
    {
        get => _teamBewerb.IsSplitGruppe;
        set
        {
            if (_teamBewerb.IsSplitGruppe == value) return;
            _teamBewerb.IsSplitGruppe = value;
            RaisePropertyChanged();
        }
    }

    public int ID => _teamBewerb.ID;
}
