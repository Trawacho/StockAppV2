using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.UI.Commands;
using StockApp.UI.Dialogs;
using StockApp.UI.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;

public class LiveResultsTeamViewModel : ViewModelBase, IDialogRequestClose
{
    private ITeamBewerb TeamBewerb => _turnierStore.Turnier.Wettbewerb as ITeamBewerb;
    private readonly ITurnierStore _turnierStore;

    public event EventHandler<DialogCloseRequestedEventArgs> DialogCloseRequested;
    public event EventHandler<WindowCloseRequestedEventArgs> WindowCloseRequested;

    protected virtual void RaiseCloseRequest(bool? dialogResult)
    {
        var dlgHandler = DialogCloseRequested;
        dlgHandler?.Invoke(this, new DialogCloseRequestedEventArgs(dialogResult));

        var wdwHandler = WindowCloseRequested;
        wdwHandler?.Invoke(this, new WindowCloseRequestedEventArgs());
    }

    public LiveResultsTeamViewModel(ITurnierStore turnierStore)
    {
        _turnierStore = turnierStore;
        TeamBewerb.GamesChanged += TeamBewerb_GamesChanged;
        foreach (var game in TeamBewerb.GetAllGames())
        {
            game.SpielstandChanged += TeamBewerb_ResultChanged;
        }

        CloseCommand = new RelayCommand(
            (p) => RaiseCloseRequest(null),
            (p) => true);

        ShowStockPunkte = true;
        IsLive = true;
    }

    /// <summary>
    /// only for Design Instance
    /// </summary>
    public LiveResultsTeamViewModel()
    {

    }

    private void TeamBewerb_GamesChanged(object sender, EventArgs e)
    {
        Dispose();
    }

    private void TeamBewerb_ResultChanged(object sender, EventArgs e)
    {
        RaisePropertyChanged(nameof(RankedTeamList));
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                TeamBewerb.GamesChanged -= TeamBewerb_GamesChanged;
                foreach (var game in TeamBewerb.GetAllGames())
                {
                    game.SpielstandChanged -= TeamBewerb_ResultChanged;
                }
                RaiseCloseRequest(null);
            }
            _disposed = true;
        }
    }

    public string WindowTitle => TeamBewerb?.SpielGruppe > 0 ? $"Live-Ergebnis der Spielgruppe {Core.Converter.GameGroupStringConverter.Convert(TeamBewerb?.SpielGruppe ?? 0)}" : "Live-Ergebnis";

    private bool _showStockPunkte;
    public bool ShowStockPunkte
    {
        get => _showStockPunkte;
        set => SetProperty(ref _showStockPunkte, value);
    }

    public bool IERVersion2022 { get => TeamBewerb.IERVersion == IERVersion.v2022; }


    private bool _isLive;
    public bool IsLive
    {
        get => _isLive;
        set => SetProperty(ref _isLive, value);
    }

    public ICommand CloseCommand { get; }

    public ObservableCollection<RankedTeamViewModel> RankedTeamList
    {
        get
        {
            var list = new List<RankedTeamViewModel>();
            int rank = 1;
            foreach (var team in TeamBewerb.GetTeamsRanked(IsLive))
            {
                list.Add(new RankedTeamViewModel(rank, team, IsLive));
                rank++;
            }
            return new ObservableCollection<RankedTeamViewModel>(list.AsReadOnly());
        }
    }


    public class RankedTeamViewModel
    {
        private readonly int _rank;
        private readonly ITeam _team;
        private readonly bool _live;

        public int Rank => _rank;
        public string TeamName => _team.TeamName;
        public string SpielPunkte => $"{_team.GetSpielPunkte(_live).positiv }:{_team.GetSpielPunkte(_live).negativ}";
        public string StockPunkte => $"{_team.GetStockPunkte(_live).positiv}:{_team.GetStockPunkte(_live).negativ}";
        public string StockNote => _team.GetStockNote(_live).ToString("F3");
        public string StockPunkteDifferenz => $"{_team.GetStockPunkteDifferenz(_live)}";

        public RankedTeamViewModel(int rank, ITeam team, bool live)
        {
            _rank = rank;
            _team = team;
            _live = live;
        }
    }

}


