using StockApp.Core.Turnier;
using StockApp.Lib.ViewModels;

namespace StockApp.Prints;

public class PrintTemplateViewModelBase : ViewModelBase
{
    private readonly ITurnier _turnier;

    public PrintTemplateViewModelBase(ITurnier turnier)
    {
        _turnier = turnier;

        HasOperator = !string.IsNullOrWhiteSpace(_turnier.OrgaDaten.Operator);
        HasOrganizer = !string.IsNullOrWhiteSpace(_turnier.OrgaDaten.Organizer);
    }

    public string Title => _turnier.OrgaDaten.TournamentName;
    public bool IsTitle2ndLine => ImageHeaderPath != null;
    public bool IsTitle1stLine => ImageHeaderPath == null;

    public string Durchführer => _turnier.OrgaDaten.Operator;
    public bool HasOperator { get; init; }

    public string Veranstalter => _turnier.OrgaDaten.Organizer;
    public bool HasOrganizer { get; init; }

    public string Ort => _turnier.OrgaDaten.Venue;
    public string Datum => _turnier.OrgaDaten.DateOfTournament.ToString("dddd, dd.MM.yyyy");

    public string ImageTopLeftPath => ImageHeaderPath == null ? _turnier.ContainerTeamBewerbe.CurrentTeamBewerb.ImageTopLeftFilename : null;
    public string ImageTopRightPath => ImageHeaderPath == null ? _turnier.ContainerTeamBewerbe.CurrentTeamBewerb.ImageTopRightFilename : null;
    public string ImageHeaderPath => _turnier.ContainerTeamBewerbe.CurrentTeamBewerb.ImageHeaderFilename;
}
