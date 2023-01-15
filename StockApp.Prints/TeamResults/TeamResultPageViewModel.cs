﻿using StockApp.Core.Factories;
using StockApp.Core.Models;
using StockApp.Core.Turnier;
using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace StockApp.Prints.TeamResults;

public class TeamResultPageViewModel
{
    private readonly ITurnier _turnier;
    private readonly ITeamBewerb _teamBewerb;

    /// <summary>
    /// only for Design-Time
    /// </summary>
    public TeamResultPageViewModel()
    {
        HasOperator = true;
        HasOrganizer = true;
        IsBestOf = false;
        IsVergleich = false;
    }

    public TeamResultPageViewModel(ITurnier turnier)
    {
        _turnier = turnier;
        _teamBewerb = ((IContainerTeamBewerbe)turnier.Wettbewerb).CurrentTeamBewerb;
        int rank = 1;
        foreach (var t in _teamBewerb.GetTeamsRanked())
        {
            RankedTeams.Add(new RankedTeamModel(rank: rank, team: t, printNameOfPlayer: rank <= _teamBewerb.NumberOfTeamsWithNamedPlayerOnResult, live: false));
            rank++;
        }

        HasOperator = !string.IsNullOrWhiteSpace(_turnier.OrgaDaten.Operator);
        HasOrganizer = !string.IsNullOrWhiteSpace(_turnier.OrgaDaten.Organizer);
        IsVergleich = GamePlanFactory.LoadAllGameplans().First(g => g.ID == _teamBewerb.GameplanId)?.IsVergleich ?? false;
        IsBestOf = _teamBewerb.Teams.Count() == 2;
    }

    public ViewModelBase BestOfViewModel => IsBestOf ? new BestOfDetailViewModel(_teamBewerb, isLive:false) : default;



    public string Title => _turnier.OrgaDaten.TournamentName;
    public string Durchführer => _turnier.OrgaDaten.Operator;
    public bool HasOperator { get; init; }

    public string Veranstalter => _turnier.OrgaDaten.Organizer;
    public bool HasOrganizer { get; init; }

    public string Ort => _turnier.OrgaDaten.Venue;
    public string Datum => _turnier.OrgaDaten.DateOfTournament.ToString("dddd, dd.MM.yyyy");


    public string RefereeName => _turnier.OrgaDaten.Referee.Name;
    public string RefereeClub => _turnier.OrgaDaten.Referee.ClubName;
    public bool HasReferee => !string.IsNullOrWhiteSpace(_turnier.OrgaDaten.Referee.Name);

    public string ComputingOfficerName => _turnier.OrgaDaten.ComputingOfficer.Name;
    public string ComputingOfficerClub => _turnier.OrgaDaten.ComputingOfficer.ClubName;
    public bool HasComputingOfficer => !string.IsNullOrWhiteSpace(_turnier.OrgaDaten.ComputingOfficer.Name);

    public string CompetitionManagerName => _turnier.OrgaDaten.CompetitionManager.Name;
    public string CompetitionManagerClub => _turnier.OrgaDaten.CompetitionManager.ClubName;
    public bool HasCompetitionManager => !string.IsNullOrWhiteSpace(_turnier.OrgaDaten.CompetitionManager.Name);

    public IList<RankedTeamModel> RankedTeams { get; } = new List<RankedTeamModel>();

    public bool IsVergleich { get; init; }
    public IList<RankedClubModel> RankedClubs
    {
        get
        {
            var clubA = new RankedClubModel(_teamBewerb.Teams.Take(_teamBewerb.Teams.Count() / 2));
            var clubB = new RankedClubModel(_teamBewerb.Teams.Skip(_teamBewerb.Teams.Count() / 2));
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

            return new[] { clubA, clubB }.OrderBy(o => o.Rank).ToList();
        }
    }

    public bool IERVersion2022 => _teamBewerb?.IERVersion == IERVersion.v2022;

    public bool IsBestOf { get; init; }

    public bool Is8TurnsGame => _teamBewerb.Is8TurnsGame;

    public bool HasMoreGroups => _turnier.ContainerTeamBewerbe.TeamBewerbe.Count() > 1;
    public string HeaderString =>
            !HasMoreGroups
                ? $"E R G E B N I S"
                : $"E R G E B N I S  -  {_teamBewerb.Gruppenname}";
}
