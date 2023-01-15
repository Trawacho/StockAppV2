using StockApp.Core.Wettbewerb.Zielbewerb;
using StockApp.Lib.ViewModels;
using StockApp.UI.Commands;
using StockApp.UI.Dialogs;
using StockApp.UI.Stores;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;

public class LiveResultsZielViewModel : ViewModelBase, IDialogRequestClose
{
    public event EventHandler<DialogCloseRequestedEventArgs> DialogCloseRequested;
    public event EventHandler<WindowCloseRequestedEventArgs> WindowCloseRequested;

    private readonly ITurnierStore _turnierStore;

    private IZielBewerb ZielBewerb => _turnierStore.Turnier.Wettbewerb as IZielBewerb;


    protected virtual void RaiseCloseRequest(bool? dialogResult)
    {
        var dlgHandler = DialogCloseRequested;
        dlgHandler?.Invoke(this, new DialogCloseRequestedEventArgs(dialogResult));

        var wdwHandler = WindowCloseRequested;
        wdwHandler?.Invoke(this, new WindowCloseRequestedEventArgs());
    }



    public LiveResultsZielViewModel(ITurnierStore turnierStore)
    {
        _turnierStore = turnierStore;
        CloseCommand = new RelayCommand(
            (p) => RaiseCloseRequest(null));

        Ranking = new ObservableCollection<ZielSpielerViewModel>();

        foreach (var spieler in ZielBewerb.Teilnehmerliste)
        {
            spieler.OnlineStatusChanged += Disziplin_ValuesChanged;
            spieler.WertungenChanged += ReAssignEvents;
            foreach (var wertung in spieler.Wertungen)
            {
                foreach (var disziplin in wertung.Disziplinen)
                {
                    disziplin.ValuesChanged += Disziplin_ValuesChanged;
                }
            }
        }
        RefreshRanking();
    }

    private void ReAssignEvents(object sender, EventArgs e)
    {
        foreach (var spieler in ZielBewerb.Teilnehmerliste)
        {
            foreach (var wertung in spieler.Wertungen)
            {
                foreach (var disziplin in wertung.Disziplinen)
                {
                    disziplin.ValuesChanged -= Disziplin_ValuesChanged;
                    disziplin.ValuesChanged += Disziplin_ValuesChanged;
                }
            }
        }
        RefreshRanking();

    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                foreach (var spieler in ZielBewerb.Teilnehmerliste)
                {
                    spieler.WertungenChanged -= ReAssignEvents;
                    spieler.OnlineStatusChanged -= Disziplin_ValuesChanged;
                    foreach (var wertung in spieler.Wertungen)
                    {
                        foreach (var disziplin in wertung.Disziplinen)
                        {
                            disziplin.ValuesChanged -= Disziplin_ValuesChanged;
                        }
                    }
                }
            }
            _disposed = true;
        }
        base.Dispose(disposing);
    }

    private void Disziplin_ValuesChanged(object sender, EventArgs e)
    {
        RefreshRanking();
    }

    private void RefreshRanking()
    {
        Ranking.Clear();
        int rank = 1;
        foreach (var teilnehmer in ZielBewerb.GetTeilnehmerRanked())
        {
            Ranking.Add(new ZielSpielerViewModel(rank++, teilnehmer));
        }
    }

    public string WindowTitle { get; } = $"StockApp Live-Ergebnisse";

    public ObservableCollection<ZielSpielerViewModel> Ranking { get; }


    public ICommand CloseCommand { get; }



    public class ZielSpielerViewModel : ViewModelBase
    {
        private readonly ITeilnehmer _teilnehmer;

        public ZielSpielerViewModel(int rank, ITeilnehmer teilnehmer)
        {
            _teilnehmer = teilnehmer;
            Platzierung = rank;
        }

        public string Name => _teilnehmer.Name;
        public string Verein => _teilnehmer.Vereinsname;
        public string GesamtPunkte => _teilnehmer.GesamtPunkte.ToString();
        public string DetailPunkte => string.Join(Environment.NewLine, _teilnehmer.Wertungen.Where(x => x.GesamtPunkte > 0).Select(x => x.DetailString()));
        public string Nation => _teilnehmer.Nation;
        public int Platzierung { get; }
    }

}

public static class WertungenExtension
{
    public static string DetailString(this IWertung wertung)
    {
        return $"{wertung.GesamtPunkte} >({wertung.PunkteMassenMitte}-{wertung.PunkteSchuesse}-{wertung.PunkteMassenSeitlich}-{wertung.PunkteKombinieren})";
    }
}