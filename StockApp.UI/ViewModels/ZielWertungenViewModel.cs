using StockApp.Comm.NetMqStockTV;
using StockApp.Core.Wettbewerb.Zielbewerb;
using StockApp.Lib.ViewModels;
using StockApp.UI.Commands;
using StockApp.UI.Extensions;
using StockApp.UI.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;

public class ZielWertungenViewModel : ViewModelBase
{
    private readonly ITeilnehmer _teilnehmer;
    private readonly IZielBewerb _zielBewerb;
    private readonly ITurnierNetworkManager _turnierNetworkManager;
    private readonly IStockTVService _stockTVService;
    private WertungViewModel _selectedWertung;
    private ICommand _removeWertungCommand;
    private ViewModelBase _wertungViewModel;
    private ICommand _addWertungCommand;



    public ZielWertungenViewModel(ITeilnehmer teilnehmer, IZielBewerb zielBewerb, ITurnierNetworkManager turnierNetworkManager, IStockTVService stockTVService)
    {
        _teilnehmer = teilnehmer;
        _zielBewerb = zielBewerb;
        _turnierNetworkManager = turnierNetworkManager;
        _stockTVService = stockTVService;
        _teilnehmer.WertungenChanged += WertungenChanged;
        WertungenChanged(this, EventArgs.Empty);
        SelectedWertung = _teilnehmer.HasOnlineWertung ? Wertungen.First(w => w.IsOnline) : Wertungen.First();

    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _teilnehmer.WertungenChanged -= WertungenChanged;
                Wertungen?.DisposeAndClear();
                WertungViewModel?.Dispose();
                SelectedWertung?.Dispose();
            }
            _disposed = true;
        }
        base.Dispose(disposing);
    }

    private void WertungenChanged(object sender, EventArgs e)
    {
        Wertungen?.DisposeAndClear();
        foreach (var w in _teilnehmer.Wertungen)
        {
            Wertungen.Add(new WertungViewModel(w));
        }
    }


    public ObservableCollection<WertungViewModel> Wertungen { get; } = new();

    public ViewModelBase WertungViewModel
    {
        get => _wertungViewModel;
        set
        {
            if (_wertungViewModel == value) return;
            _wertungViewModel?.Dispose();
            _wertungViewModel = value;
            RaisePropertyChanged();
        }
    }

    public WertungViewModel SelectedWertung
    {
        get => _selectedWertung;
        set
        {
            if (_selectedWertung == value) return;
            _selectedWertung?.Dispose();
            _selectedWertung = value;

            WertungViewModel = value != null
                        ? new ZielWertungViewModel(_selectedWertung.Wertung, _teilnehmer, _zielBewerb, _turnierNetworkManager, _stockTVService)
                        : _teilnehmer.HasOnlineWertung
                            ? new ZielWertungViewModel(_teilnehmer.OnlineWertung, _teilnehmer, _zielBewerb, _turnierNetworkManager, _stockTVService)
                            : new ZielWertungViewModel(_teilnehmer.Wertungen.First(), _teilnehmer, _zielBewerb, _turnierNetworkManager, _stockTVService);

        }
    }




    public ICommand AddWertungCommand => _addWertungCommand ??= new RelayCommand(
        (p) => _teilnehmer.AddNewWertung(),
        (p) => _teilnehmer.CanAddWertung());

    public ICommand RemoveWertungCommand => _removeWertungCommand ??= new RelayCommand(
        (p) => _teilnehmer.RemoveWertung(SelectedWertung.Wertung),
        (p) => _teilnehmer.CanRemoveWertung() && SelectedWertung != null && !SelectedWertung.Wertung.IsOnline);
}
