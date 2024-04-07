using StockApp.Core.Turnier;
using StockApp.Core.Wettbewerb.Zielbewerb;
using System.Collections.Generic;
using System.Windows.Controls;

namespace StockApp.Prints.ZielResult;
public class ZielResultViewModel : PrintTemplateViewModelBase
{
    private readonly ITurnier _turnier;
    private IZielBewerb ZielBewerb => _turnier.Wettbewerb as IZielBewerb;

    public ZielResultViewModel(ITurnier turnier) : base(turnier)
    {
        _turnier = turnier;
        InitBodyElements();
    }

    private void InitBodyElements()
    {
        BodyElements = new List<Grid>();
        Grid grid = new Grid();
        BodyElements.Add(grid);
    }

    public List<Grid> BodyElements { get; set; }


    public string HeaderString => $"E R G E B N I S";

    public string Endtext => ZielBewerb.EndText;


    public string RefereeName => _turnier.OrgaDaten.Referee.Name;
    public string RefereeClub => _turnier.OrgaDaten.Referee.ClubName;
    public bool HasReferee => !string.IsNullOrWhiteSpace(_turnier.OrgaDaten.Referee.Name);

    public string ComputingOfficerName => _turnier.OrgaDaten.ComputingOfficer.Name;
    public string ComputingOfficerClub => _turnier.OrgaDaten.ComputingOfficer.ClubName;
    public bool HasComputingOfficer => !string.IsNullOrWhiteSpace(_turnier.OrgaDaten.ComputingOfficer.Name);

    public string CompetitionManagerName => _turnier.OrgaDaten.CompetitionManager.Name;
    public string CompetitionManagerClub => _turnier.OrgaDaten.CompetitionManager.ClubName;
    public bool HasCompetitionManager => !string.IsNullOrWhiteSpace(_turnier.OrgaDaten.CompetitionManager.Name);
}
