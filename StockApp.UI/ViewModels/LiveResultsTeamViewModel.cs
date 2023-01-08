using StockApp.Core.Models;
using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.UI.Commands;
using StockApp.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;

public class LiveResultsTeamViewModel : ViewModelBase, IDialogRequestClose
{
    private readonly ITeamBewerb _teamBewerb;

    public event EventHandler<DialogCloseRequestedEventArgs> DialogCloseRequested;
    public event EventHandler<WindowCloseRequestedEventArgs> WindowCloseRequested;

    protected virtual void RaiseCloseRequest(bool? dialogResult)
    {
        var dlgHandler = DialogCloseRequested;
        dlgHandler?.Invoke(this, new DialogCloseRequestedEventArgs(dialogResult));

        var wdwHandler = WindowCloseRequested;
        wdwHandler?.Invoke(this, new WindowCloseRequestedEventArgs());
    }

    public LiveResultsTeamViewModel(ITeamBewerb teamBewerb)
    {
        _teamBewerb = teamBewerb;

        _teamBewerb.GamesChanged += TeamBewerb_GamesChanged;
        foreach (var game in _teamBewerb.GetAllGames())
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
        if (IsVergleich) RaisePropertyChanged(nameof(RankedClubList));
        if (IsBestOf) RaisePropertyChanged(nameof(KehrenPerGame));
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _teamBewerb.GamesChanged -= TeamBewerb_GamesChanged;
                foreach (var game in _teamBewerb.GetAllGames())
                {
                    game.SpielstandChanged -= TeamBewerb_ResultChanged;
                }
                RaiseCloseRequest(null);
            }
            _disposed = true;
        }
    }

    public string WindowTitle => _teamBewerb?.SpielGruppe > 0
        ? $"Live-Ergebnis - {_teamBewerb.Gruppenname}"
        : "Live-Ergebnis";

    private bool _showStockPunkte;
    public bool ShowStockPunkte
    {
        get => _showStockPunkte;
        set => SetProperty(ref _showStockPunkte, value);
    }

    public bool IERVersion2022 { get => _teamBewerb.IERVersion == IERVersion.v2022; }


    private bool _isLive;
    public bool IsLive
    {
        get => _isLive;
        set => SetProperty(ref _isLive, value);
    }

    private bool _isVergleich;
    public bool IsVergleich
    {
        get => _isVergleich;
        set => SetProperty(ref _isVergleich, value);
    }

    private bool? _isVergleichPossible;
    public bool IsVergleichPossible => _isVergleichPossible ??= Core.Factories.GamePlanFactory.LoadAllGameplans().First(g => g.ID == _teamBewerb.GameplanId)?.IsVergleich ?? false;

    private bool _isBestOf;
    public bool IsBestOf { get => _isBestOf; set => SetProperty(ref _isBestOf, value); }

    public bool IsBestOfPossible => _teamBewerb.Teams.Count() == 2;
    public bool Has8Turns => _teamBewerb.Is8TurnsGame;

    public ICommand CloseCommand { get; }

    public ObservableCollection<RankedTeamModel> RankedTeamList
    {
        get
        {
            var list = new List<RankedTeamModel>();
            int rank = 1;
            foreach (var team in _teamBewerb.GetTeamsRanked(IsLive))
            {
                list.Add(new RankedTeamModel(rank: rank, team: team, live: IsLive, printNameOfPlayer: false));
                rank++;
            }
            return new ObservableCollection<RankedTeamModel>(list.AsReadOnly());
        }
    }

    public ObservableCollection<RankedClubModel> RankedClubList
    {
        get
        {
            var clubA = new RankedClubModel(_teamBewerb.Teams.Take(_teamBewerb.Teams.Count() / 2), IsLive);
            var clubB = new RankedClubModel(_teamBewerb.Teams.Skip(_teamBewerb.Teams.Count() / 2), IsLive);

            var compared = clubA.CompareTo(clubB);
            switch (compared)
            {
                case 0:
                    clubA.Rank = 1;
                    clubB.Rank = 1;
                    break;
                case 1:
                    clubA.Rank = 1;
                    clubB.Rank = 2;
                    break;
                case -1:
                default:
                    clubA.Rank = 2;
                    clubB.Rank = 1;
                    break;
            }

            return new ObservableCollection<RankedClubModel>(new[] { clubA, clubB }.OrderBy(o => o.Rank));
        }
    }

    public IEnumerable<KehrenLivePerGameViewModel> KehrenPerGame
    {
        get
        {
            foreach (var game in _teamBewerb.GetAllGames(false).OrderBy(g => g.GameNumberOverAll))
               yield return new KehrenLivePerGameViewModel(game);
        }
    }
}

public class KehrenLivePerGameViewModel : KehrenBaseViewModel
{
    public KehrenLivePerGameViewModel(IGame game) : base(game)
    {

    }

    public string TeamName1 => _game.IsTeamA_Starting ? $"(A) {_game.TeamA.TeamNameShort}" : _game.TeamA.TeamNameShort;

    public string TeamName2 => _game.IsTeamA_Starting ? _game.TeamB.TeamName : $"(A) {_game.TeamB.TeamNameShort}";

    public override int StockPunkte1 => _game.Spielstand.Punkte_Live_TeamA;

    public override int StockPunkte2 => _game.Spielstand.Punkte_Live_TeamB;

    public override int Spielpunkte1 => _game.Spielstand.GetSpielPunkteTeamA(true);
    public override int Spielpunkte2 => _game.Spielstand.GetSpielPunkteTeamB(true);

    protected override int GetKehre(int kehrenNummer, bool team1)
    {
        return team1 
            ? _game.Spielstand.Kehren_Live.FirstOrDefault(k => k.KehrenNummer == kehrenNummer)?.PunkteTeamA ?? 0
            : _game.Spielstand.Kehren_Live.FirstOrDefault(k => k.KehrenNummer == kehrenNummer)?.PunkteTeamB ?? 0;
    }
}
