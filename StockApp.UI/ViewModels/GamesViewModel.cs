using StockApp.Core.Factories;
using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.ViewModels;
using StockApp.UI.Commands;
using StockApp.UI.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;

public class GamesViewModel : ViewModelBase
{
    private readonly ITurnierStore _turnierStore;
    private ICommand _createGamesCommand;
    private bool _isCreatingGames;
    private ITeamBewerb _currentTeamBewerb;
    private ViewModelBase _gamesPrintsViewModel;
    private IEnumerable<IGameplan> _gameplans;

    private ITeamBewerb CurrentTeamBewerb
    {
        get => _currentTeamBewerb;
        set
        {
            if (_currentTeamBewerb != null)
                _currentTeamBewerb.GamesChanged -= TeamBewerb_GamesChanged;

            _pendingGameplanId = null;

            SetProperty(ref _currentTeamBewerb, value);
            if (value != null)
                GamesPrintsViewModel = new GamesPrintsViewModel(CurrentTeamBewerb, _turnierStore);

            if (value != null)
                _currentTeamBewerb.GamesChanged += TeamBewerb_GamesChanged;
        }
    }

    public ViewModelBase GamesPrintsViewModel
    {
        get => _gamesPrintsViewModel;
        private set
        {
            _gamesPrintsViewModel?.Dispose();
            SetProperty(ref _gamesPrintsViewModel, value);
        }
    }
    public bool IsCreatingGames { get => _isCreatingGames; set => SetProperty(ref _isCreatingGames, value); }

    public IEnumerable<IGameplan> Gameplans
    {
        get => _gameplans;
        private set
        {
            SetProperty(ref _gameplans, value);
        }
    }

    #region TeamBewerb - Depended Properties

    public int CountOfGames => CurrentTeamBewerb.GetCountOfGames();
    public int CountOfGamesPerCourt => CurrentTeamBewerb.GetCountOfGamesPerCourt();
    public bool HasMoreGameRounds => SpielRunden > 1;
    public bool HasGames => CountOfGames > 0;
    public bool HasNoGames => !HasGames;

    /// <summary>
    /// Kleinster Wert, auf den <see cref="SpielRunden"/> gesetzt werden darf, ohne bereits gespielte Runden zu gefährden.
    /// </summary>
    public int MinSpielRunden => Math.Max(1, CurrentTeamBewerb.GetHighestPlayedRound());

    private int? _pendingGameplanId;

    /// <summary>
    /// UI-seitige Auswahl des Spielplans. Wird erst bei tatsächlicher Generierung (<see cref="CreateGamesCommand"/>)
    /// in <see cref="ITeamBewerb.GameplanId"/> übernommen, damit erkennbar bleibt, ob sich die Auswahl seit der
    /// letzten Generierung geändert hat.
    /// </summary>
    public int SelectedGameplanId
    {
        get => _pendingGameplanId
            ?? (Gameplans.Any(g => g.ID == CurrentTeamBewerb.GameplanId) ? CurrentTeamBewerb.GameplanId : 0);
        set
        {
            if (SelectedGameplanId == value)
                return;

            _pendingGameplanId = value;
            RaisePropertyChanged();
        }
    }

    public bool HasChangeStart
    {
        get => CurrentTeamBewerb.StartingTeamChange;
        set
        {
            if (CurrentTeamBewerb.StartingTeamChange == value)
                return;

            CurrentTeamBewerb.StartingTeamChange = value;
            RaisePropertyChanged(nameof(HasChangeStart));
        }
    }

    public bool Has8Turns
    {
        get => CurrentTeamBewerb.Is8TurnsGame;
        set
        {
            if (CurrentTeamBewerb.Is8TurnsGame == value) return;
            CurrentTeamBewerb.Is8TurnsGame = value;
            RaisePropertyChanged();
        }
    }

    public int SpielRunden
    {
        get => CurrentTeamBewerb.NumberOfGameRounds;
        set
        {
            if (CurrentTeamBewerb.NumberOfGameRounds == value) return;
            CurrentTeamBewerb.NumberOfGameRounds = value;
            RaisePropertyChanged(nameof(SpielRunden));
            RaisePropertyChanged(nameof(HasMoreGameRounds));
        }
    }

    #endregion

    #region Confirm-Overlay (Spielplan generieren)

    private bool _isConfirmModalOpen;
    private string _confirmMessage;
    private string _confirmYesText;
    private string _confirmNoText;
    private Action _pendingConfirmYes;
    private Action _pendingConfirmNo;
    private ICommand _confirmYesCommand;
    private ICommand _confirmNoCommand;
    private ICommand _confirmCancelCommand;

    public bool IsConfirmModalOpen { get => _isConfirmModalOpen; private set => SetProperty(ref _isConfirmModalOpen, value); }
    public string ConfirmMessage { get => _confirmMessage; private set => SetProperty(ref _confirmMessage, value); }
    public string ConfirmYesText { get => _confirmYesText; private set => SetProperty(ref _confirmYesText, value); }
    public string ConfirmNoText { get => _confirmNoText; private set => SetProperty(ref _confirmNoText, value); }

    /// <summary>
    /// TRUE, wenn im Confirm-Overlay ein "Nein"-Button angezeigt werden soll (dreigeteilte Abfrage).
    /// FALSE = nur Ja/Abbrechen (zweigeteilte Abfrage).
    /// </summary>
    public bool HasConfirmNoOption => !string.IsNullOrEmpty(ConfirmNoText);

    private void OpenConfirmModal(string message, string yesText, Action onYes, string noText = null, Action onNo = null)
    {
        ConfirmMessage = message;
        ConfirmYesText = yesText;
        ConfirmNoText = noText;
        RaisePropertyChanged(nameof(HasConfirmNoOption));
        _pendingConfirmYes = onYes;
        _pendingConfirmNo = onNo;
        IsConfirmModalOpen = true;
    }

    private void CloseConfirmModal()
    {
        IsConfirmModalOpen = false;
        _pendingConfirmYes = null;
        _pendingConfirmNo = null;
    }

    public ICommand ConfirmYesCommand => _confirmYesCommand ??= new RelayCommand(
        (p) =>
        {
            var action = _pendingConfirmYes;
            CloseConfirmModal();
            action?.Invoke();
        },
        (p) => true);

    public ICommand ConfirmNoCommand => _confirmNoCommand ??= new RelayCommand(
        (p) =>
        {
            var action = _pendingConfirmNo;
            CloseConfirmModal();
            action?.Invoke();
        },
        (p) => true);

    public ICommand ConfirmCancelCommand => _confirmCancelCommand ??= new RelayCommand(
        (p) =>
        {
            CloseConfirmModal();
            IsCreatingGames = false;
        },
        (p) => true);

    #endregion

    public ICommand CreateGamesCommand => _createGamesCommand ??= new RelayCommand
        ((p) =>
        {
            IsCreatingGames = true;

            var teamBewerb = CurrentTeamBewerb;
            var gameplan = Gameplans.FirstOrDefault(g => g.ID == SelectedGameplanId);
            int currentMaxRound = teamBewerb.Games.Any() ? teamBewerb.Games.Max(g => g.RoundOfGame) : 0;
            bool gameplanChanged = teamBewerb.GameplanId != SelectedGameplanId;

            void FinishGeneration()
            {
                teamBewerb.GameplanId = SelectedGameplanId;
                _pendingGameplanId = null;
                IsCreatingGames = false;
            }

            void AppendRounds()
            {
                int nextGameNumberOverAll = teamBewerb.Games.Max(g => g.GameNumberOverAll) + 1;
                GamePlanFactory.MatchTeamAndGames(gameplan, teamBewerb.Teams, SpielRunden, HasChangeStart,
                    startRound: currentMaxRound + 1, startGameNumberOverAll: nextGameNumberOverAll);
                FinishGeneration();
            }

            void OverwriteAll()
            {
                //Entferne alle Spiele von allen Teams
                foreach (var t in teamBewerb.Teams)
                    t.ClearGames();

                teamBewerb.IsSplitGruppe = gameplan?.IsSplit ?? false;

                GamePlanFactory.MatchTeamAndGames(gameplan, teamBewerb.Teams, SpielRunden, HasChangeStart);
                FinishGeneration();
            }

            if (teamBewerb.Games.Any())
            {
                if (!gameplanChanged && SpielRunden > currentMaxRound)
                {
                    OpenConfirmModal(
                        "Es sind bereits Spiele/Ergebnisse vorhanden.",
                        yesText: "Runde anhängen", onYes: AppendRounds,
                        noText: "Ergebnisse überschreiben", onNo: OverwriteAll);
                }
                else
                {
                    OpenConfirmModal(
                        "Dies löscht alle bisherigen Spiele und Ergebnisse und erstellt den Spielplan neu. Fortfahren?",
                        yesText: "Ja", onYes: OverwriteAll);
                }
                return;
            }

            OverwriteAll();
        },
        (p) => !IsCreatingGames && SelectedGameplanId != 0
        );

    #region Constructor

    public GamesViewModel(ITurnierStore turnierStore)
    {
        _turnierStore = turnierStore;
        _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerbChanged += CurrentTeamBewerbChangend;

        CurrentTeamBewerb = _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb;

		//TODO: die Anzahl der verfügbaren Mannschaften, ohne die Entschuldigten. Sollte auch beim Erstellen der Spiele berücksichtigt werden
		// damit die entschuldigten Mannschaften nicht am Ende stehen müssen. 
		// Funktion noch nicht ausreichend getestet!! ... was ist bei Splitgruppen!!!
		int teamsCount = CurrentTeamBewerb.Teams.Where(t => t.TeamStatus != TeamStatus.Entschuldigt).Count();
        Gameplans = _turnierStore.Turnier.ContainerTeamBewerbe.TeamBewerbe.Count() == 1
                          ? Gameplans = _turnierStore.Turnier.ContainerTeamBewerbe.Gameplans.Where(t => t.Teams == teamsCount)
                          : Gameplans = _turnierStore.Turnier.ContainerTeamBewerbe.Gameplans.Where(t => t.Teams == teamsCount && !t.IsSplit);
    }

    private void CurrentTeamBewerbChangend(object sender, EventArgs e)
    {
        CurrentTeamBewerb = _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb;

		//TODO: die Anzahl der verfügbaren Mannschaften, ohne die Entschuldigten. Sollte auch beim Erstellen der Spiele berücksichtigt werden
		// damit die entschuldigten Mannschaften nicht am Ende stehen müssen
		// Funktion noch nicht ausreichend getestet!! ... was ist bei Splitgruppen!!!
		int teamsCount = CurrentTeamBewerb.Teams.Where(t => t.TeamStatus != TeamStatus.Entschuldigt).Count();
		Gameplans = _turnierStore.Turnier.ContainerTeamBewerbe.TeamBewerbe.Count() == 1
                         ? Gameplans = _turnierStore.Turnier.ContainerTeamBewerbe.Gameplans.Where(t => t.Teams == teamsCount)
                         : Gameplans = _turnierStore.Turnier.ContainerTeamBewerbe.Gameplans.Where(t => t.Teams == teamsCount && !t.IsSplit);

        TeamBewerb_GamesChanged(sender, e);
    }

    private void TeamBewerb_GamesChanged(object sender, System.EventArgs e)
    {
        RaisePropertyChanged(nameof(HasNoGames));
        RaisePropertyChanged(nameof(CountOfGames));
        RaisePropertyChanged(nameof(CountOfGamesPerCourt));
        RaisePropertyChanged(nameof(SpielRunden));
        RaisePropertyChanged(nameof(HasMoreGameRounds));
        RaisePropertyChanged(nameof(HasGames));
        RaisePropertyChanged(nameof(SelectedGameplanId));
        RaisePropertyChanged(nameof(HasChangeStart));
        RaisePropertyChanged(nameof(Has8Turns));
        RaisePropertyChanged(nameof(MinSpielRunden));
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerbChanged -= CurrentTeamBewerbChangend;
                CurrentTeamBewerb.GamesChanged -= TeamBewerb_GamesChanged;
                GamesPrintsViewModel?.Dispose();
                GamesPrintsViewModel = null;
            }
            _disposed = true;
        }
    }

    #endregion
}
