using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.ViewModels;
using StockApp.UI.Stores;

namespace StockApp.UI.ViewModels;

public class OutputViewModel : ViewModelBase
{
    private readonly ITurnierStore _turnierStore;
    private ITeamBewerb TeamBewerb => _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb;

    int _anzahlAufsteiger;
    int _anzahlAbsteiger;
    object _logoLinks;
    object _logoRechts;
    object _briefKopf;

    public OutputViewModel(ITurnierStore turnierStore)
    {
        _turnierStore = turnierStore;
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                ;
            }
            _disposed = true;
        }
    }

    public string NumberOfTeamsWithNamedPlayerOnResult
    {
        get => TeamBewerb.NumberOfTeamsWithNamedPlayerOnResult.ToString();
        set
        {
            var val = StringToIntConverter(value);
            if (TeamBewerb.NumberOfTeamsWithNamedPlayerOnResult != val)
            {
                TeamBewerb.NumberOfTeamsWithNamedPlayerOnResult = val;
                RaisePropertyChanged();
            }
        }
    }

    public string AnzahlAufsteiger
    {
        get => _anzahlAufsteiger.ToString();
        set => SetProperty(ref _anzahlAufsteiger, StringToIntConverter(value));
    }

    public string AnzahlAbsteiger
    {
        get => _anzahlAbsteiger.ToString();
        set => SetProperty(ref _anzahlAbsteiger, StringToIntConverter(value));
    }

    private int StringToIntConverter(string value) => int.TryParse(value, out int result) ? result : 0;

    public string EndText
    {
        get => _turnierStore.Turnier.OrgaDaten.Endtext;
        set
        {
            _turnierStore.Turnier.OrgaDaten.Endtext = value;
            RaisePropertyChanged();
        }
    }
}
