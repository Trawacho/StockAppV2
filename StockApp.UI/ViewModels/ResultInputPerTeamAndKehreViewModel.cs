using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace StockApp.UI.ViewModels;

internal class ResultInputPerTeamAndKehreViewModel : ViewModelBase
{
    private readonly bool _is8TurnsGame;
    private ITeam _selectedTeam;

    public ResultInputPerTeamAndKehreViewModel(IEnumerable<ITeam> teams, bool is8TurnsGame)
    {
        _is8TurnsGame = is8TurnsGame;

        foreach (var team in teams.Where(t => !t.IsVirtual))
        {
            Teams.Add(team);
        }

        SelectedTeam = Teams?.FirstOrDefault();
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                SelectedTeam = null;
                Teams?.Clear();
                KehrenPerTeam.DisposeAndClear();
            }
            _disposed = true;
        }
    }

    public ITeam SelectedTeam
    {
        get => _selectedTeam;
        set => SetProperty(
            ref _selectedTeam,
            value,
            () =>
            {
                SetPointsPerTeam();
            });
    }

    public ObservableCollection<ITeam> Teams { get; } = new();

    public ObservableCollection<KehrePerTeamAndGameViewModel> KehrenPerTeam { get; } = new();

    public bool Has8Turns => _is8TurnsGame;

    private void SetPointsPerTeam()
    {
        KehrenPerTeam.Clear();

        if (SelectedTeam == null) return;

        foreach (var game in SelectedTeam?.Games?.OrderBy(g => g.GameNumberOverAll))
        {
            KehrenPerTeam.Add(new KehrePerTeamAndGameViewModel(game, SelectedTeam));
        }
    }
}
public class KehrePerTeamAndGameViewModel : KehrenBaseViewModel
{
    private readonly ITeam _team;

    public KehrePerTeamAndGameViewModel(IGame game, ITeam team) : base(game)
    {
        _team = team;
        _game.SpielstandChanged += Game_SpielstandChanged;
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _game.SpielstandChanged -= Game_SpielstandChanged;
            }
            _disposed = true;
        }
    }

    private void Game_SpielstandChanged(object sender, EventArgs e)
    {
        RaisePropertyChanged(nameof(StockPunkte1));
        RaisePropertyChanged(nameof(StockPunkte2));
    }

    public bool IsBreakGame => _game.IsPauseGame();
    public int GameNumber => _game.GameNumberOverAll;
    public string Gegner
    {
        get
        {
            if (IsBreakGame)
                return "Setzt aus";

            return _team.StartNumber == _game.TeamA.StartNumber
                ? _game.TeamB.TeamName
                : _game.TeamA.TeamName;
        }
    }

    public override int StockPunkte1 => _team.StartNumber == _game.TeamA.StartNumber ? _game.Spielstand.Punkte_Master_TeamA : _game.Spielstand.Punkte_Master_TeamB;
   
    public override int StockPunkte2 => _team.StartNumber != _game.TeamA.StartNumber ? _game.Spielstand.Punkte_Master_TeamA : _game.Spielstand.Punkte_Master_TeamB;
    
    protected override void SetKehre(int kehrenNummer, int value, bool team1, string propName1, [CallerMemberName] string propName2 = null)
    {
        if (team1)
        {
            if (_team.StartNumber == _game.TeamA.StartNumber)
                _game.Spielstand.SetMasterKehre(kehrenNummer, teamA: value, teamB: value != 0 ? 0 : int.MinValue);
            else
                _game.Spielstand.SetMasterKehre(kehrenNummer, teamA: value != 0 ? 0 : int.MinValue, teamB: value);
        }
        else
        {
            if (_team.StartNumber != _game.TeamA.StartNumber)
                _game.Spielstand.SetMasterKehre(kehrenNummer, teamA: value, teamB: value != 0 ? 0 : int.MinValue);
            else
                _game.Spielstand.SetMasterKehre(kehrenNummer, teamA: value != 0 ? 0 : int.MinValue, teamB: value);
        }

        RaisePropertyChanged(propName1);
        RaisePropertyChanged(propName2);
        RaisePropertyChanged(nameof(StockPunkte1));
        RaisePropertyChanged(nameof(StockPunkte2));
    }

    protected override int GetKehre(int kehrenNummer, bool team1)
    {
        if (team1)
            return _team.StartNumber == _game.TeamA.StartNumber
                ? _game.Spielstand.Kehren_Master.FirstOrDefault(k => k.KehrenNummer == kehrenNummer)?.PunkteTeamA ?? 0
                : _game.Spielstand.Kehren_Master.FirstOrDefault(k => k.KehrenNummer == kehrenNummer)?.PunkteTeamB ?? 0;
        else
            return _team.StartNumber != _game.TeamA.StartNumber
            ? _game.Spielstand.Kehren_Master.FirstOrDefault(k => k.KehrenNummer == kehrenNummer)?.PunkteTeamA ?? 0
            : _game.Spielstand.Kehren_Master.FirstOrDefault(k => k.KehrenNummer == kehrenNummer)?.PunkteTeamB ?? 0;
    }
}