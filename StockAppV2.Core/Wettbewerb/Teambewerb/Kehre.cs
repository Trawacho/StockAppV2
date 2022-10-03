using StockApp.Comm.NetMqStockTV;

namespace StockApp.Core.Wettbewerb.Teambewerb;

public interface IKehre
{
    int KehrenNummer { get; set; }
    int PunkteTeamA { get; set; }
    int PunkteTeamB { get; set; }
}

public class Kehre : IKehre
{
    private Kehre()
    {

    }

    public static IKehre Create(int kehrenNummer, int punkteTeamA, int punkteTeamB)
    {
        return new Kehre()
        {
            KehrenNummer = kehrenNummer,
            PunkteTeamA = punkteTeamA,
            PunkteTeamB = punkteTeamB
        };
    }


    public int KehrenNummer { get; set; }
    public int PunkteTeamA { get; set; }
    public int PunkteTeamB { get; set; }
    public static IKehre Convert(IStockTVTurn turn, bool spielrichtungRechtsNachLinks)
    {
        if (spielrichtungRechtsNachLinks)
            return new Kehre() { KehrenNummer = turn.TurnNumber, PunkteTeamA = turn.PointsA, PunkteTeamB = turn.PointsB };
        else
            return new Kehre() { KehrenNummer = turn.TurnNumber, PunkteTeamA = turn.PointsB, PunkteTeamB = turn.PointsA };

    }
}
