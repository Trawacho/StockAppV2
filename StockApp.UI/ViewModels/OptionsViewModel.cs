using Microsoft.Win32;
using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.Extensions;
using StockApp.Lib.ViewModels;
using StockApp.UI.Commands;
using StockApp.UI.Stores;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;

public class OptionsViewModel : ViewModelBase
{
    private readonly ITurnierStore _turnierStore;
    private ITeamBewerb TeamBewerb => _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb;
    private readonly string _imageFileFilter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;*.bmp";

    public OptionsViewModel(ITurnierStore turnierStore)
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

    private static int StringToIntConverter(string value) => int.TryParse(value, out int result) ? result : 0;

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

    public string ImageLinksObenPath
    {
        get => _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.ImageTopLeftFilename;
        set
        {
            if (_turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.ImageTopLeftFilename != value)
            {
                _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.ImageTopLeftFilename = value;
                RaisePropertyChanged();
            }
        }
    }

    public string ImageRechtsObenPath
    {
        get => _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.ImageTopRightFilename;
        set
        {
            if (_turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.ImageTopRightFilename != value)
            {
                _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.ImageTopRightFilename = value;
                RaisePropertyChanged();
            }
        }
    }

    public string ImageHeaderPath
    {
        get => _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.ImageHeaderFilename;
        set
        {
            if (_turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.ImageHeaderFilename != value)
            {
                _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.ImageHeaderFilename = value;
                RaisePropertyChanged();
            }
        }
    }

    public int RowSpace
    {
        get => _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.RowSpace;
        set
        {
            if (_turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.RowSpace != value)
            {
                _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.RowSpace = value.InRange(0, 99);
                RaisePropertyChanged();
            }
        }
    }

    public int FontSize
    {
        get => _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.FontSize;
        set
        {
            if (_turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.FontSize != value)
            {
                _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.FontSize = value.InRange(12, 24);
                RaisePropertyChanged();
            }
        }
    }

    public bool PageBreakSplitGroup
    {
        get => _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.PageBreakSplitGroup;
        set
        {
            if(_turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.PageBreakSplitGroup != value)
            {
                _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.PageBreakSplitGroup = value;
                RaisePropertyChanged();
            }
        }
    }
    public bool IsSplitGruppe => _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.IsSplitGruppe;



    public ICommand ImageLinksObenResetCommand => new RelayCommand(
        (p) => ImageLinksObenPath = null,
        (p) => true);

    public ICommand ImageLinksObenSelectCommand => new RelayCommand(
        (p) =>
        {
            var ofd = new OpenFileDialog() { Filter = _imageFileFilter, Title = "Bild auswählen..." };
            if (ofd.ShowDialog() == true)
                ImageLinksObenPath = ofd.FileName;
        },
        (p) => true);


    public ICommand ImageRechtsObenResetCommand => new RelayCommand(
        (p) => ImageRechtsObenPath = null,
        (p) => true);

    public ICommand ImageRechtsObenSelectCommand => new RelayCommand(
        (p) =>
        {
            var ofd = new OpenFileDialog() { Filter = _imageFileFilter, Title = "Bild auswählen..." };
            if (ofd.ShowDialog() == true)
                ImageRechtsObenPath = ofd.FileName;
        },
        (p) => true);

    public ICommand ImageHeaderResetCommand => new RelayCommand(
        (p) => ImageHeaderPath = null,
        (p) => true);

    public ICommand ImageHeaderSelectCommand => new RelayCommand(
        (p) =>
        {
            var ofd = new OpenFileDialog() { Filter = _imageFileFilter, Title = "Bild auswählen..." };
            if (ofd.ShowDialog() == true)
                ImageHeaderPath = ofd.FileName;
        },
        (p) => true);

}
