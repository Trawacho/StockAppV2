using StockApp.Core.Turnier;
using StockApp.Lib.ViewModels;

namespace StockApp.Prints;

public class PrintTemplateViewModelBase : ViewModelBase
{
    private readonly ITurnier _turnier;

    public PrintTemplateViewModelBase(ITurnier turnier)
    {
        _turnier = turnier;
    }

    public string Title => _turnier.OrgaDaten.TournamentName;


    public string Durchführer => _turnier.OrgaDaten.Operator;
    public bool HasOperator => !string.IsNullOrWhiteSpace(_turnier.OrgaDaten.Operator);

    public string Veranstalter => _turnier.OrgaDaten.Organizer;
    public bool HasOrganizer => !string.IsNullOrWhiteSpace(_turnier.OrgaDaten.Organizer);

    public string Ort => _turnier.OrgaDaten.Venue;
    public string Datum => _turnier.OrgaDaten.DateOfTournament.ToString("dddd, dd.MM.yyyy");

}