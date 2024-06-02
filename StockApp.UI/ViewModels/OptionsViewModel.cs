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
    private ITeamBewerb _teamBewerb;

    private ITeamBewerb TeamBewerb
    {
        get => _teamBewerb; set
        {
            SetProperty(ref _teamBewerb, value);
        }
    }

    private readonly string _imageFileFilter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;*.bmp";

    public OptionsViewModel(ITurnierStore turnierStore)
    {
        _turnierStore = turnierStore;
        TeamBewerb = _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb;

        _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerbChanged += ContainerTeamBewerbe_CurrentTeamBewerbChanged;
    }

    private void ContainerTeamBewerbe_CurrentTeamBewerbChanged(object sender, System.EventArgs e)
    {
        TeamBewerb = _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb;
        RaisePropertyChanged(nameof(NumberOfTeamsWithNamedPlayerOnResult));
        RaisePropertyChanged(nameof(AnzahlAufsteiger));
        RaisePropertyChanged(nameof(AnzahlAbsteiger));
        RaisePropertyChanged(nameof(EndText));
        RaisePropertyChanged(nameof(TeamNameWithStartnumber));
        RaisePropertyChanged(nameof(ImageLinksObenPath));
        RaisePropertyChanged(nameof(ImageRechtsObenPath));
        RaisePropertyChanged(nameof(ImageHeaderPath));
        RaisePropertyChanged(nameof(RowSpace));
        RaisePropertyChanged(nameof(FontSize));
        RaisePropertyChanged(nameof(PageBreakSplitGroup));
        RaisePropertyChanged(nameof(IsSplitGruppe));
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerbChanged -= ContainerTeamBewerbe_CurrentTeamBewerbChanged; ;
            }
            _disposed = true;
        }
    }

    #region Properties

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

    public string EndText
    {
        get => TeamBewerb.Endtext;
        set
        {
            _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.Endtext = value;
            RaisePropertyChanged();
        }
    }

    public bool TeamNameWithStartnumber
    {
        get => TeamBewerb.TeamNameWithStartnumber;
        set
        {
            _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.TeamNameWithStartnumber = value;
            RaisePropertyChanged();
        }
    }

    public string ImageLinksObenPath
    {
        get => TeamBewerb.ImageTopLeftFilename;
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
        get => TeamBewerb.ImageTopRightFilename;
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
        get => TeamBewerb.ImageHeaderFilename;
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
        get => TeamBewerb.RowSpace;
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
        get => TeamBewerb.FontSize;
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
        get => TeamBewerb.PageBreakSplitGroup;
        set
        {
            if (TeamBewerb.PageBreakSplitGroup != value)
            {
                TeamBewerb.PageBreakSplitGroup = value;
                RaisePropertyChanged();
            }
        }
    }
   
    public bool IsSplitGruppe => TeamBewerb.IsSplitGruppe;

    #endregion

    #region Commands

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

    #endregion


    private static int StringToIntConverter(string value) => int.TryParse(value, out int result) ? result : 0;
}
