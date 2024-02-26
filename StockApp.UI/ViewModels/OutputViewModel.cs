using StockApp.Lib.ViewModels;
using StockApp.UI.Stores;

namespace StockApp.UI.ViewModels;

public class OutputViewModel : ViewModelBase
{
    private readonly ITurnierStore _turnierStore;

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

    public int _anzahlAufsteiger;
    public int _anzahlAbsteiger;
    public string _endText;
    public object _logoLinks;
    public object _logoRechts;
    public object _briefKopf;
}
