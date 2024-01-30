using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockApp.UI.ViewModels;

public class TeamViewModel : ViewModelBase
{

    private readonly ITeam _team;
    private TeamPlayersViewModel _teamPlayersViewModel;
    private readonly List<string> _teamStatis;

    public ITeam Team { get => _team; }
    public string TeamName
    {
        get => _team.TeamName;
        set
        {
            _team.TeamName = value;
            RaisePropertyChanged();
        }
    }
    public string StartNumber
    {
        get => _team.StartNumber.ToString(); set
        {
            if (int.TryParse(value, out int s))
            {
                _team.StartNumber = s;
            }
            RaisePropertyChanged();
        }
    }
    public string Nation
    {
        get => _team.Nation;
        set
        {
            _team.Nation = value;
            RaisePropertyChanged();
        }
    }

    public IEnumerable<string> TeamStatis => _teamStatis;

    public string TeamStatus
    {
        get => _team.TeamStatus.Name();
        set
        {
            _team.TeamStatus = TeamStatusExtension.FromName(value);
            RaisePropertyChanged();
        }
    }

    public int StrafSpielpunkte
    {
        get => _team.StrafSpielpunkte;
        set
        {
            _team.StrafSpielpunkte = value;
            RaisePropertyChanged();
        }
    }

    public TeamPlayersViewModel TeamPlayersViewModel
    {
        get => _teamPlayersViewModel;
        set => SetProperty(ref _teamPlayersViewModel, value);
    }

    public TeamViewModel(ITeam team)
    {
        _team = team;
        TeamPlayersViewModel = new TeamPlayersViewModel(_team);
        _teamStatis = new List<string>();
        foreach (var item in Enum.GetValues(typeof(TeamStatus)).Cast<TeamStatus>()) 
        {
            _teamStatis.Add(item.Name());
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                TeamPlayersViewModel.Dispose();
                _teamPlayersViewModel = null;
            }
            _disposed = true;
        }
    }


}
