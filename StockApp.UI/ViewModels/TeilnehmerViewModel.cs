using StockApp.Core.Wettbewerb.Zielbewerb;
using StockApp.Lib.ViewModels;
using StockApp.UI.com;
using StockApp.UI.Commands;
using StockApp.UI.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;

public class TeilnehmerViewModel : ViewModelBase
{
    private readonly ITeilnehmer _teilnehmer;
    public ITeilnehmer Teilnehmer => _teilnehmer;
    private readonly IEnumerable<IVerein> _vereine;
    public TeilnehmerViewModel(ITeilnehmer teilnehmer, ITurnierStore store)
    {
        _teilnehmer = teilnehmer;
        _teilnehmer.OnlineStatusChanged += OnlineStatusChanged;
        _teilnehmer.StartNumberChanged += StartNumberChanged;
        _vereine = store.TemplateVereine;
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

    public IEnumerable<string> TemplateVereine => _vereine.Select(x => x.Name);

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

    public string Spielklasse
    {
        get => _teilnehmer.Spielklasse;
        set
        {
            if (string.IsNullOrWhiteSpace(value.TrimEnd()))
            {
                _teilnehmer.Spielklasse = null;
            }
            else
            {
                _teilnehmer.Spielklasse = value.TrimEnd();
            }


            RaisePropertyChanged();
        }
    }

    public ICommand VereinSelectedEnterCommand => new RelayCommand(
        (p) =>
        {
            var x = _vereine?.FirstOrDefault(v => v.Name == Verein);
            if (x != null)
                Nation = x.Land + "/" + x.Region + "/" + x.Bundesland + "/" + x.Kreis;
        },
        (p) => true);
}
