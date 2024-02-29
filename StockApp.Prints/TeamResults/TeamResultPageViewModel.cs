using StockApp.Core.Factories;
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

        HasOperator = !string.IsNullOrWhiteSpace(_turnier.OrgaDaten.Operator);
        HasOrganizer = !string.IsNullOrWhiteSpace(_turnier.OrgaDaten.Organizer);
        IsVergleich = GamePlanFactory.LoadAllGameplans().FirstOrDefault(g => g.ID == _teamBewerb.GameplanId)?.IsVergleich ?? false;
        IsBestOf = _teamBewerb.Teams.Count() == 2;
        RankedTeamsTableViewModels = new List<RankedTeamsTableViewModel>();
        foreach (var item in _turnier.ContainerTeamBewerbe.TeamBewerbe)
        {
            if (item.IsSplitGruppe)
            {
                RankedTeamsTableViewModels.Add(new RankedTeamsTableViewModel(item, isLive: false, isSplitGroupOne: true, showStockPunkte: true));
                RankedTeamsTableViewModels.Add(new RankedTeamsTableViewModel(item, isLive: false, isSplitGroupOne: false, showStockPunkte: true));
            }
            else
            {
                RankedTeamsTableViewModels.Add(new RankedTeamsTableViewModel(item, isLive: false, showGroupName: true));
            }
        }
    }

    public ViewModelBase RankedTeamsTableViewModel => new RankedTeamsTableViewModel(_teamBewerb, isLive: false, showGroupName: false);
    public List<RankedTeamsTableViewModel> RankedTeamsTableViewModels { get; init; }
    public ViewModelBase BestOfViewModel => IsBestOf ? new BestOfDetailViewModel(_teamBewerb, isLive: false) : default;
    public ViewModelBase RankedClubViewModel => IsVergleich ? new RankedClubTableViewModel(_teamBewerb, isLive: false) { AsDataGrid = false } : default;


    public string Title => _turnier.OrgaDaten.TournamentName;
    public bool IsTitle2ndLine => ImageHeaderPath != null;
    public bool IsTitle1stLine => ImageHeaderPath == null;

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

    public bool IsVergleich { get; init; }
    public bool IsBestOf { get; init; }
    public bool HasMoreGroups => _turnier.ContainerTeamBewerbe.TeamBewerbe.Count() > 1 || _turnier.ContainerTeamBewerbe.TeamBewerbe.Where(b => b.IsSplitGruppe).Any();
    public string _HeaderString => $"E R G E B N I S";
    public string HeaderString => _turnier.ContainerTeamBewerbe.CurrentTeamBewerb.IsEachGameDone(false) ? _HeaderString : "Zwischenergebnis";

    public string Endtext => _turnier.ContainerTeamBewerbe.CurrentTeamBewerb.Endtext;
    public string Footer
    {
        get
        {
            var footerText = string.Empty;

            if (_turnier.ContainerTeamBewerbe.CurrentTeamBewerb.Teams.Any(t => t.TeamStatus != TeamStatus.Normal))
                footerText = TeamStatusExtension.FooterText();

            if (_turnier.ContainerTeamBewerbe.CurrentTeamBewerb.Teams.Any(t => t.StrafSpielpunkte > 0))
            {
                if (!string.IsNullOrEmpty(footerText))
                    footerText += "; ";

                footerText += "$ = Spielpunktstrafe";
            }

            return footerText;
        }
    }


    public string ImageTopLeftPath => ImageHeaderPath == null ?  _turnier.ContainerTeamBewerbe.CurrentTeamBewerb.ImageTopLeftFilename : null;
    public string ImageTopRightPath => ImageHeaderPath == null ?  _turnier.ContainerTeamBewerbe.CurrentTeamBewerb.ImageTopRightFilename : null;
    public string ImageHeaderPath => _turnier.ContainerTeamBewerbe.CurrentTeamBewerb.ImageHeaderFilename;
}
