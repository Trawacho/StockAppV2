using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using StockApp.Core.Wettbewerb.Zielbewerb;
using StockApp.Lib.Extensions;
using StockApp.Lib.ViewModels;
using StockApp.Prints.Receipts;
using StockApp.Prints.Teamresult;
using StockApp.Prints.ZielResult;
using StockApp.UI.Commands;
using StockApp.UI.Components;
using StockApp.UI.Settings;
using StockApp.UI.Stores;
using System.ServiceModel;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;

public class ZielBewerbDruckViewModel : ViewModelBase
{
    private readonly ITurnierStore _turnierStore;
    private IZielBewerb ZielBewerb => _turnierStore.Turnier.Wettbewerb as IZielBewerb;
    private readonly string _imageFileFilter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;*.bmp";

    public ZielBewerbDruckViewModel(ITurnierStore turnierStore)
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


    private ICommand _druckResultCommand;
    public ICommand DruckResultCommand => _druckResultCommand ??= new AsyncRelayCommand(
        async (p) =>
        {
            var _printPreview = new PrintPreview(await ZielResultFactory.Create(_turnierStore.Turnier));
            PreferencesManager.GeneralAppSettings.WindowPlaceManager.Register(_printPreview, "ZielResult");
            _printPreview.ShowDialog();
        },
        (p) => { return true; });

    private ICommand _druckQuittungenCommand;
    public ICommand DruckQuittungenCommand => _druckQuittungenCommand ??= new RelayCommand(
         async (p) =>
         {
             var _printPreview = new PrintPreview(await ReceiptsFactory.Create(_turnierStore.Turnier));
             PreferencesManager.GeneralAppSettings.WindowPlaceManager.Register(_printPreview, "Receipt");
             _printPreview.ShowDialog();
         },
        (p) => { return true; });

    public ICommand ImageHeaderSelectCommand => new RelayCommand(
        (p) =>
        {
            var ofd = new OpenFileDialog() { Filter = _imageFileFilter, Title = "Bild auswählen..." };
            if (ofd.ShowDialog() == true)
                ImageHeaderPath = ofd.FileName;
        },
        (p) => true);

    public ICommand ImageHeaderResetCommand => new RelayCommand(
        (p) => ImageHeaderPath = null,
        (p) => true);

    public ICommand ImageRechtsObenSelectCommand => new RelayCommand(
       (p) =>
       {
           var ofd = new OpenFileDialog() { Filter = _imageFileFilter, Title = "Bild auswählen..." };
           if (ofd.ShowDialog() == true)
               ImageRechtsObenPath = ofd.FileName;
       },
       (p) => true);

    public ICommand ImageRechtsObenResetCommand => new RelayCommand(
        (p) => ImageRechtsObenPath = null,
        (p) => true);

    public ICommand ImageLinksObenSelectCommand => new RelayCommand(
        (p) =>
        {
            var ofd = new OpenFileDialog() { Filter = _imageFileFilter, Title = "Bild auswählen..." };
            if (ofd.ShowDialog() == true)
                ImageLinksObenPath = ofd.FileName;
        },
        (p) => true);

    public ICommand ImageLinksObenResetCommand => new RelayCommand(
        (p) => ImageLinksObenPath = null,
        (p) => true);

    public int FontSize
    {
        get => ZielBewerb.FontSize;
        set
        {
            if (ZielBewerb.FontSize != value)
            {
                ZielBewerb.FontSize = value.InRange(12, 24);
                RaisePropertyChanged();
            }
        }
    }

    public int RowSpace
    {
        get => ZielBewerb.RowSpace;
        set
        {
            if (ZielBewerb.RowSpace != value)
            {
                ZielBewerb.RowSpace = value.InRange(0, 99);
                RaisePropertyChanged();
            }
        }
    }

    public bool HasTeamname
    {
        get => ZielBewerb.HasTeamname;
        set
        {
            if (ZielBewerb.HasTeamname != value)
            {
                ZielBewerb.HasTeamname = value;
                RaisePropertyChanged();
            }
        }
    }

    public bool HasNation
    {
        get => ZielBewerb.HasNation;
        set
        {
            if (ZielBewerb.HasNation != value)
            {
                ZielBewerb.HasNation = value;
                RaisePropertyChanged();
            }
        }
    }

    public string ImageHeaderPath
    {
        get => ZielBewerb.ImageHeaderFileName;
        set
        {
            if (ZielBewerb.ImageHeaderFileName != value)
            {
                ZielBewerb.ImageHeaderFileName = value;
                RaisePropertyChanged();
            }
        }
    }

    public string ImageRechtsObenPath
    {
        get => ZielBewerb.ImageTopRightFileName;
        set
        {
            if (ZielBewerb.ImageTopRightFileName != value)
            {
                ZielBewerb.ImageTopRightFileName = value;
                RaisePropertyChanged();
            }
        }
    }

    public string ImageLinksObenPath
    {
        get => ZielBewerb.ImageTopLeftFileName;
        set
        {
            if (ZielBewerb.ImageTopLeftFileName != value)
            {
                ZielBewerb.ImageTopLeftFileName = value;
                RaisePropertyChanged();
            }
        }
    }

    public string EndText
    {
        get => ZielBewerb.EndText;
        set
        {
            if (ZielBewerb.EndText != value)
            {
                ZielBewerb.EndText = value;
                RaisePropertyChanged();
            }
        }
    }
}
