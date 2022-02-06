using StockApp.Core.Wettbewerb.Zielbewerb;
using StockApp.UI.Commands;
using StockApp.UI.Extensions;
using StockApp.UI.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;

public class ZielWertungViewModel : ViewModelBase
{
    private readonly IWertung _wertung;
    private readonly ITeilnehmer _teilnehmer;
    private readonly IZielBewerb _zielBewerb;
    private readonly ITurnierNetworkManager _turnierNetworkManager;
    private ICommand _setWertungOnlineCommand;
    private ICommand _setWertungOfflineCommand;

    public ZielWertungViewModel(IWertung wertung, ITeilnehmer teilnehmer, IZielBewerb zielBewerb, ITurnierNetworkManager turnierNetworkManager)
    {
        _wertung = wertung;
        _teilnehmer = teilnehmer;
        _zielBewerb = zielBewerb;
        _turnierNetworkManager = turnierNetworkManager;
        foreach (var d in _wertung.Disziplinen)
        {
            d.ValuesChanged += DisziplinValueChanged;
            ZielDisziplinViewModels.Add(new DisziplinViewModel(d));
        }

        _teilnehmer.OnlineStatusChanged += OnlineStatusChanged;
    }

    private void DisziplinValueChanged(object sender, EventArgs e)
    {
        RaisePropertyChanged(nameof(GesamtPunkte));
    }

    private void OnlineStatusChanged(object sender, EventArgs e)
    {
        RaisePropertyChanged(nameof(IsOnline));
        RaisePropertyChanged(nameof(IsOffline));
        RaisePropertyChanged(nameof(AktuelleBahn));
        RaisePropertyChanged(nameof(FreieBahnen));
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _teilnehmer.OnlineStatusChanged -= OnlineStatusChanged;

                foreach (var d in _wertung?.Disziplinen)
                {
                    d.ValuesChanged -= DisziplinValueChanged;
                }
                ZielDisziplinViewModels?.DisposeAndClear();
            }
            _disposed = true;
        }
        base.Dispose(disposing);
    }

    public ObservableCollection<DisziplinViewModel> ZielDisziplinViewModels { get; } = new();
    public bool IsOnline => _wertung.IsOnline;
    public bool IsOffline => !IsOnline;

    public int Nummer => _wertung.Nummer;
    public int GesamtPunkte => _wertung.GesamtPunkte;

    public int AktuelleBahn => _teilnehmer.AktuelleBahn;

    public IEnumerable<int> FreieBahnen => _zielBewerb.FreieBahnen;

    public int SelectedBahn { get; set; }

    public ICommand SetWertungOnlineCommand => _setWertungOnlineCommand ??= new RelayCommand(
            (p) =>
            {
                _turnierNetworkManager.AcceptNetworkResult = true;
                _teilnehmer.SetOnline(SelectedBahn, _wertung.Nummer);
            },
            (p) => !IsOnline && SelectedBahn > 0 && _wertung.GesamtPunkte == 0);
    public ICommand SetWertungOfflineCommand => _setWertungOfflineCommand ??= new RelayCommand(
            (p) => _teilnehmer.SetOffline(),
            (p) => IsOnline);

}

