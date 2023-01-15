using StockApp.Core.Wettbewerb.Zielbewerb;
using System;

namespace StockApp.UI.ViewModels;
public class DisziplinViewModel : ViewModelBase
{
    private readonly IDisziplin _disziplin;

    public DisziplinViewModel(IDisziplin disziplin)
    {
        _disziplin = disziplin;
        _disziplin.ValuesChanged += ValueChanged;
    }
    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _disziplin.ValuesChanged -= ValueChanged;
            }
            _disposed = true;
        }
        base.Dispose(disposing);
    }

    private void ValueChanged(object sender, EventArgs e)
    {
        RaisePropertyChanged(nameof(Summe));
        RaisePropertyChanged(nameof(Versuch1));
        RaisePropertyChanged(nameof(Versuch2));
        RaisePropertyChanged(nameof(Versuch3));
        RaisePropertyChanged(nameof(Versuch4));
        RaisePropertyChanged(nameof(Versuch5));
        RaisePropertyChanged(nameof(Versuch6));
    }

    public int Summe => _disziplin.Summe;
    public string Name => _disziplin.Name;
    public int Versuch1 { get => _disziplin.Versuch1; set => _disziplin.Versuch1 = value; }
    public int Versuch2 { get => _disziplin.Versuch2; set => _disziplin.Versuch2 = value; }
    public int Versuch3 { get => _disziplin.Versuch3; set => _disziplin.Versuch3 = value; }
    public int Versuch4 { get => _disziplin.Versuch4; set => _disziplin.Versuch4 = value; }
    public int Versuch5 { get => _disziplin.Versuch5; set => _disziplin.Versuch5 = value; }
    public int Versuch6 { get => _disziplin.Versuch6; set => _disziplin.Versuch6 = value; }
}
