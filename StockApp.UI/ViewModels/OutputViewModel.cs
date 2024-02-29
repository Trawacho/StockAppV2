using Microsoft.Win32;
using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.ViewModels;
using StockApp.UI.Commands;
using StockApp.UI.Stores;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;

public class OutputViewModel : ViewModelBase
{
    private readonly ITurnierStore _turnierStore;
    private ITeamBewerb TeamBewerb => _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb;

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
        get => TeamBewerb.AnzahlAufsteiger.ToString();
        set
        {
            var val = StringToIntConverter(value);
            if (TeamBewerb.AnzahlAufsteiger != val)
            {
                TeamBewerb.AnzahlAufsteiger = val;
                RaisePropertyChanged();
            }
        }
    }

    public string AnzahlAbsteiger
    {
        get => TeamBewerb.AnzahlAbsteiger.ToString();
        set
        {
            var val = StringToIntConverter(value);
            if (TeamBewerb.AnzahlAbsteiger != val)
            {
                TeamBewerb.AnzahlAbsteiger = val;
                RaisePropertyChanged();
            }
        }
    }

    private int StringToIntConverter(string value) => int.TryParse(value, out int result) ? result : 0;

    public string EndText
    {
        get => _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.Endtext;
        set
        {
            _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.Endtext = value;
            RaisePropertyChanged();
        }
    }

    public bool TeamNameWithStartnumber
    {
        get => _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.TeamNameWithStartnumber;
        set
        {
            _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.TeamNameWithStartnumber = value;
            RaisePropertyChanged();
        }
    }

    public string ImageLinksObenPath => _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.ImageTopLeftFilename;
    public ICommand ImageLinksObenResetCommand => new RelayCommand(
        (p) =>
        {
            _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.ImageTopLeftFilename = null;
            RaisePropertyChanged(nameof(ImageLinksObenPath));
        },
        (p) => true);

    public ICommand ImageLinksObenSelectCommand => new RelayCommand(
        (p) =>
        {
            var ofd = new OpenFileDialog();
            
            ofd.ShowDialog();
            _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.ImageTopLeftFilename = ofd.FileName;
            RaisePropertyChanged(nameof(ImageLinksObenPath));
        },
        (p) => true);
}
