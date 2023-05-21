using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.Models;
using StockApp.Lib.ViewModels;
using StockApp.UI.Commands;
using StockApp.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        if (IsVergleich) RaisePropertyChanged(nameof(RankedClubTableViewModel));
        if (IsBestOf) RaisePropertyChanged(nameof(BestOfDetailsViewModel));
        if (IsSplitGruppe)
        {
            RaisePropertyChanged(nameof(RankedTeamsTableSplitGroupOneViewModel));
            RaisePropertyChanged(nameof(RankedTeamsTableSplitGroupTwoViewModel));
        }
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

    public string WindowTitle => _teamBewerb?.SpielGruppe > 0 ? $"Live-Ergebnis - {_teamBewerb.Gruppenname}" : "Live-Ergebnis";
    public bool IERVersion2022 => _teamBewerb.IERVersion == IERVersion.v2022;
    public bool IsBestOfPossible => _teamBewerb.Teams.Count() == 2;
    public bool Has8Turns => _teamBewerb.Is8TurnsGame;
    public bool IsSplitGruppe => _teamBewerb.IsSplitGruppe;

    public ViewModelBase BestOfDetailsViewModel => IsBestOf ? new BestOfDetailViewModel(_teamBewerb, isLive: true) : default;
    public ViewModelBase RankedClubTableViewModel => IsVergleich ? new RankedClubTableViewModel(_teamBewerb, isLive: true) { AsDataGrid = true } : default;

    public ViewModelBase RankedTeamsTableSplitGroupOneViewModel => IsSplitGruppe
        ? new RankedTeamsTableViewModel(_teamBewerb, isLive: IsLive, isSplitGroupOne: true, showStockPunkte: ShowStockPunkte)
        : default;
    public ViewModelBase RankedTeamsTableSplitGroupTwoViewModel => IsSplitGruppe
        ? new RankedTeamsTableViewModel(_teamBewerb, isLive: IsLive, isSplitGroupOne: false, showStockPunkte: ShowStockPunkte)
        : default;


    private bool _showStockPunkte;
    public bool ShowStockPunkte
    {
        get => _showStockPunkte;
        set => SetProperty(
            ref _showStockPunkte,
            value,
            () =>
            {
                RaisePropertyChanged(nameof(RankedTeamsTableSplitGroupOneViewModel));
                RaisePropertyChanged(nameof(RankedTeamsTableSplitGroupTwoViewModel));
            });
    }


    private bool _isLive;
    public bool IsLive { get => _isLive; set => SetProperty(ref _isLive, value); }


    private bool _isVergleich;
    public bool IsVergleich { get => _isVergleich; set => SetProperty(ref _isVergleich, value, () => RaisePropertyChanged(nameof(RankedClubTableViewModel))); }


    private bool? _isVergleichPossible;
    public bool IsVergleichPossible => _isVergleichPossible ??= Core.Factories.GamePlanFactory.LoadAllGameplans().First(g => g.ID == _teamBewerb.GameplanId)?.IsVergleich ?? false;


    private bool _isBestOf;
    public bool IsBestOf { get => _isBestOf; set => SetProperty(ref _isBestOf, value, () => RaisePropertyChanged(nameof(BestOfDetailsViewModel))); }


    public ICommand CloseCommand { get; init; }

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

}
