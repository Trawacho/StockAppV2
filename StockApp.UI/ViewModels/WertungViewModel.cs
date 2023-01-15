using StockApp.Core.Wettbewerb.Zielbewerb;
using StockApp.Lib.ViewModels;
using System;

namespace StockApp.UI.ViewModels;

public class WertungViewModel : ViewModelBase
{
    private readonly IWertung _wertung;

    public IWertung Wertung => _wertung;

    public WertungViewModel(IWertung wertung)
    {
        _wertung = wertung;
        _wertung.OnlineStatusChanged += OnlineStatusChanged;
        foreach (var d in _wertung.Disziplinen)
        {
            d.ValuesChanged += ValuesChanged;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _wertung.OnlineStatusChanged -= OnlineStatusChanged;

                foreach (var d in _wertung.Disziplinen)
                {
                    d.ValuesChanged -= ValuesChanged;
                }
            }
            _disposed = true;
        }
        base.Dispose(disposing);
    }

    private void OnlineStatusChanged(object sender, EventArgs e)
    {
        RaisePropertyChanged(nameof(IsOnline));
    }

    private void ValuesChanged(object sender, EventArgs e)
    {
        RaisePropertyChanged(nameof(GesamtPunkte));
        RaisePropertyChanged(nameof(PunkteMassenMitte));
        RaisePropertyChanged(nameof(PunkteMassenSeitlich));
        RaisePropertyChanged(nameof(PunkteSchuesse));
        RaisePropertyChanged(nameof(PunkteKombinieren));
    }

    public int Nummer => _wertung.Nummer;
    public int PunkteMassenMitte => _wertung.PunkteMassenMitte;
    public int PunkteMassenSeitlich => _wertung.PunkteMassenSeitlich;
    public int PunkteSchuesse => _wertung.PunkteSchuesse;
    public int PunkteKombinieren => _wertung.PunkteKombinieren;
    public int GesamtPunkte => _wertung.GesamtPunkte;
    public bool IsOnline => _wertung.IsOnline;

}
