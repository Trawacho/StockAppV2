using StockApp.Core.Turnier;
using StockApp.Core.Wettbewerb.Teambewerb;
using System.Collections.Generic;

namespace StockApp.Prints.TeamResults;

public class TeamResultPageViewModel
{
    private readonly ITurnier _turnier;
    private readonly ITeamBewerb _teamBewerb;

    public TeamResultPageViewModel(ITurnier turnier)
    {
        _turnier = turnier;
        _teamBewerb = turnier.Wettbewerb as ITeamBewerb;
        int rank = 1;
        foreach (var t in _teamBewerb.GetTeamsRanked())
        {
            RankedTeams.Add(new RankedTeamViewModel(rank, t, rank <= _teamBewerb.NumberOfTeamsWithNamedPlayerOnResult));
            rank++;
        }

    }

    public string Title => _turnier.OrgaDaten.TournamentName;
    public string Durchführer => _turnier.OrgaDaten.Operator;
    public bool HasOperator => !string.IsNullOrWhiteSpace(_turnier.OrgaDaten.Operator);

    public string Veranstalter => _turnier.OrgaDaten.Organizer;
    public bool HasOrganizer => !string.IsNullOrWhiteSpace(_turnier.OrgaDaten.Organizer);

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
    public bool HasCompetitionManger => !string.IsNullOrWhiteSpace(_turnier.OrgaDaten.CompetitionManager.Name);


    public IList<RankedTeamViewModel> RankedTeams { get; } = new List<RankedTeamViewModel>();


}

