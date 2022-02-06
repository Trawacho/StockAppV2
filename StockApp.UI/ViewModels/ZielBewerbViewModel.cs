using StockApp.Core.Wettbewerb.Zielbewerb;
using StockApp.UI.Commands;
using StockApp.UI.Extensions;
using StockApp.UI.Services;
using StockApp.UI.Stores;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;

public class ZielBewerbViewModel : ViewModelBase
{
    private readonly ITurnierStore _turnierStore;
    private readonly ITurnierNetworkManager _turnierNetworkManager;
    private readonly IZielBewerb _zielBewerb;
    private ICommand _addPlayerCommand;
    private ICommand _removePlayerCommand;
    private TeilnehmerViewModel _selectedTeilnehmer;
    private ViewModelBase _wertungenViewModel;

    public ZielBewerbViewModel(ITurnierStore turnierStore, ITurnierNetworkManager turnierNetworkManager)
    {
        _turnierStore = turnierStore;
        _turnierNetworkManager = turnierNetworkManager;
        _zielBewerb = _turnierStore.Turnier?.Wettbewerb as IZielBewerb;
        _zielBewerb.TeilnehmerCollectionChanged += TeilnehmerCollectionChanged;
        TeilnehmerCollectionChanged(this, EventArgs.Empty);
        SelectedTeilnehmer = Teilnehmerliste?.First();
    }


    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _zielBewerb.TeilnehmerCollectionChanged -= TeilnehmerCollectionChanged;
                _selectedTeilnehmer?.Dispose();
                Teilnehmerliste?.DisposeAndClear();
                WertungenViewModel?.Dispose();
            }
        }
        base.Dispose(disposing);
    }

    private void TeilnehmerCollectionChanged(object sender, EventArgs e)
    {
        Teilnehmerliste.DisposeAndClear();
        foreach (var tln in _zielBewerb.Teilnehmerliste)
        {
            Teilnehmerliste.Add(new TeilnehmerViewModel(tln));
        }
    }

    public TeilnehmerViewModel SelectedTeilnehmer
    {
        get => _selectedTeilnehmer;
        set
        {
            if (_selectedTeilnehmer == value) return;
            _selectedTeilnehmer?.Dispose();
            _selectedTeilnehmer = value;

            if (value != null)
                WertungenViewModel = value != null ? new ZielWertungenViewModel(_selectedTeilnehmer.Teilnehmer, _zielBewerb, _turnierNetworkManager)
                    : new ZielWertungenViewModel(Teilnehmerliste.First().Teilnehmer, _zielBewerb, _turnierNetworkManager);

            RaisePropertyChanged();
        }
    }

    public ObservableCollection<TeilnehmerViewModel> Teilnehmerliste { get; } = new();

    public ViewModelBase WertungenViewModel
    {
        get => _wertungenViewModel;
        set
        {
            if (_wertungenViewModel == value) return;
            _wertungenViewModel?.Dispose();
            _wertungenViewModel = value;
            RaisePropertyChanged();
        }
    }


    public ICommand AddPlayerCommand => _addPlayerCommand ??= new RelayCommand(
        (p) => _zielBewerb.AddNewTeilnehmer(),
        (p) => _zielBewerb.CanAddTeilnehmer());

    public ICommand RemovePlayerCommand => _removePlayerCommand ??= new RelayCommand(
        (p) => _zielBewerb.RemoveTeilnehmer(SelectedTeilnehmer.Teilnehmer),
        (p) => _zielBewerb.CanRemoveTeilnehmer() && SelectedTeilnehmer != null);



}

public class TeilnehmerViewModel : ViewModelBase
{
    private readonly ITeilnehmer _teilnehmer;
    public ITeilnehmer Teilnehmer => _teilnehmer;
    public TeilnehmerViewModel(ITeilnehmer teilnehmer)
    {
        _teilnehmer = teilnehmer;
        _teilnehmer.OnlineStatusChanged += OnlineStatusChanged;
        _teilnehmer.StartNumberChanged += StartNumberChanged;
    }


    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _teilnehmer.OnlineStatusChanged -= OnlineStatusChanged;
                _teilnehmer.StartNumberChanged -= StartNumberChanged;
            }
            _disposed = true;
        }
        base.Dispose(disposing);
    }

    private void OnlineStatusChanged(object sender, EventArgs e)
    {
        RaisePropertyChanged(nameof(Name));
    }

    private void StartNumberChanged(object sender, EventArgs e)
    {
        RaisePropertyChanged(nameof(Startnummer));

    }


    public string Name => _teilnehmer.Name;

    public int Startnummer => _teilnehmer.Startnummer;

    public string Nachname
    {
        get => _teilnehmer.LastName;
        set
        {
            _teilnehmer.LastName = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(Name));
        }
    }

    public string FirstName
    {
        get => _teilnehmer.FirstName;
        set
        {
            _teilnehmer.FirstName = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(Name));
        }
    }

    public string Verein
    {
        get => _teilnehmer.Vereinsname;
        set
        {
            _teilnehmer.Vereinsname = value;
            RaisePropertyChanged();
        }
    }

    public string Nation
    {
        get => _teilnehmer.Nation;
        set
        {
            _teilnehmer.Nation = value;
            RaisePropertyChanged();
        }
    }

    public string Passnummer
    {
        get => _teilnehmer.LicenseNumber;
        set
        {
            _teilnehmer.LicenseNumber = value;
            RaisePropertyChanged();
        }
    }
}
