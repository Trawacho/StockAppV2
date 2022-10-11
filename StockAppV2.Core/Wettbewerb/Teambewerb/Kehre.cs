using StockApp.Comm.NetMqStockTV;

namespace StockApp.Core.Wettbewerb.Teambewerb;

public interface IKehre
{
    int KehrenNummer { get; }
    int PunkteTeamA { get; }
    int PunkteTeamB { get; }
    /// <summary>
    /// Set values if not default
    /// </summary>
    /// <param name="teamA"></param>
    /// <param name="teamB"></param>
    IKehre SetKehre(int teamA = int.MinValue, int teamB = int.MinValue);

}

public class Kehre : IKehre
{
    private Kehre(int kehrenNummer)
    {
        KehrenNummer = kehrenNummer;
    }

    public static IKehre Create(int kehrenNummer, int punkteTeamA, int punkteTeamB)
    {
        return new Kehre(kehrenNummer)
        {
            PunkteTeamA = punkteTeamA,
            PunkteTeamB = punkteTeamB
        };
    }

    public int KehrenNummer { get; init; }
    public int PunkteTeamA { get; private set; }
    public int PunkteTeamB { get; private set; }
   

    /// <summary>
    /// Set values if not default
    /// </summary>
    /// <param name="teamA"></param>
    /// <param name="teamB"></param>
    public IKehre SetKehre(int teamA = int.MinValue, int teamB = int.MinValue)
    {
        if (teamA != int.MinValue)
            PunkteTeamA = teamA;

        if (teamB != int.MinValue)
            PunkteTeamB = teamB;

        return this;
    }

    public static IKehre Convert(IStockTVTurn turn, bool spielrichtungRechtsNachLinks)
    {
        if (spielrichtungRechtsNachLinks)
            return new Kehre(turn.TurnNumber) { PunkteTeamA = turn.PointsA, PunkteTeamB = turn.PointsB };
        else
            return new Kehre(turn.TurnNumber) { PunkteTeamA = turn.PointsB, PunkteTeamB = turn.PointsA };

    }
}
