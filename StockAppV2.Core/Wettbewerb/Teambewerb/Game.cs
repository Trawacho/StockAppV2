using StockApp.Core.Factories;

namespace StockApp.Core.Wettbewerb.Teambewerb;

public interface IGame : IEquatable<IGame>
{
    int RoundOfGame { get; }
    int CourtNumber { get; set; }
    int GameNumber { get; }
    int GameNumberOverAll { get; }
    bool IsTeamA_Starting { get; set; }

    ITeam TeamA { get; set; }
    ITeam TeamB { get; set; }

    ISpielstand Spielstand { get; }

    bool IsPauseGame();
    ITeam GetStartingTeam();
    ITeam GetNotStartingTeam();

    string ToStringExtended();

    event EventHandler SpielstandChanged;
}
public class Game : IGame
{

    #region Fields
    private readonly ISpielstand _spielstand;

    #endregion

    #region IEquatable Implementation

    /// <summary>
    /// True, if RoundOfGame, CourtNumber, GameNumber and IsTeamA_Starting are equal
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(IGame other)
    {
        return RoundOfGame == other.RoundOfGame
             && CourtNumber == other.CourtNumber
             && GameNumber == other.GameNumber
             && IsTeamA_Starting == other.IsTeamA_Starting;
    }

    #endregion


    public event EventHandler SpielstandChanged;
    protected virtual void RaiseSpielstandChanged()
    {
        var handler = SpielstandChanged;
        handler?.Invoke(this, EventArgs.Empty);
    }


    #region Properties

    /// <summary>
    /// SpielRunde (für DoppelRunde oder mehrfachrunden)
    /// </summary>
    public int RoundOfGame { get; set; }

    /// <summary>
    /// Nummer vom laufenden Spiel
    /// </summary>
    public int GameNumber { get; set; }

    /// <summary>
    /// Nummer des Spiels über alle Runden hinweg
    /// </summary>

    public int GameNumberOverAll { get; set; }

    /// <summary>
    /// Nummer der Spielbahn
    /// </summary>
    public int CourtNumber { get; set; }

    /// <summary>
    /// Das Team A hat Anspiel
    /// </summary>
    public bool IsTeamA_Starting { get; set; }

    /// <summary>
    /// Team A - 1
    /// </summary>
    public ITeam TeamA { get; set; }

    /// <summary>
    /// Team B - 2
    /// </summary>
    public ITeam TeamB { get; set; }

    /// <summary>
    /// Stockpunkte in diesem Spiel (auch Live)
    /// </summary>
    public ISpielstand Spielstand => _spielstand;

    #endregion

    #region Constructor

    /// <summary>
    /// Default Constructor
    /// </summary>
    private Game(ISpielstand spielstand)
    {
        _spielstand = spielstand;
        _spielstand.SpielStandChanged += (s, e) => RaiseSpielstandChanged();
    }

    public static IGame Create(ITeam teamA, ITeam teamB, int gameNumber, int roundOfGame, int gameNumberOverAll)
    {
        return new Game(Teambewerb.Spielstand.Create())
        {
            TeamA = teamA,
            TeamB = teamB,
            GameNumber = gameNumber,
            RoundOfGame = roundOfGame,
            GameNumberOverAll = gameNumberOverAll
        };
    }

    public static IGame Create(ITeam teamA, ITeam teamB, int courtNumber, int gameNumber, int roundOfGame, int gameNumberOverAll, bool isTeamA_Starting)
    {
        return new Game(Teambewerb.Spielstand.Create())
        {
            TeamA = teamA,
            TeamB = teamB,
            CourtNumber = courtNumber,
            GameNumber = gameNumber,
            RoundOfGame = roundOfGame,
            GameNumberOverAll = gameNumberOverAll,
            IsTeamA_Starting = isTeamA_Starting,
        };
    }

    public static IGame Create(int gameNumber, int roundOfGame, int gameNumberOverAll)
    {
        return new Game(Teambewerb.Spielstand.Create())
        {
            GameNumber = gameNumber,
            RoundOfGame = roundOfGame,
            GameNumberOverAll = gameNumberOverAll
        };
    }

    public static IGame Create(IFactoryGame factoryGame)
    {
        var game = Create(factoryGame.GameNumber, factoryGame.RoundOfGame, factoryGame.GameNumberOverAll);
        game.CourtNumber = factoryGame.CourtNumber;
        game.IsTeamA_Starting = factoryGame.IsTeamA_Starting;
        return game;
    }

    #endregion

    #region Public Functions


    public override string ToString() => $"Runde: {RoundOfGame} | Spiel: {GameNumber}";

    public string ToStringExtended()
    {
        string s = $"Runde#:{RoundOfGame} G#:{GameNumber}({GameNumberOverAll}) Bahn#:{CourtNumber} -> {TeamA?.StartNumber}";

        if (IsTeamA_Starting) s += "x";

        s += $" : {TeamB?.StartNumber}";

        if (!IsTeamA_Starting) s += "x";

        if (IsPauseGame()) s += " -- Aussetzer";

        return s;
    }

    /// <summary>
    /// Wer ist der Gegner
    /// </summary>
    /// <param name="team"></param>
    /// <returns></returns>
    public ITeam GetOpponent(ITeam team)
    {
        return team == TeamA
                     ? TeamB
                     : team == TeamB
                             ? TeamA
                             : null;
    }

    /// <summary>
    /// Das anspielende Team
    /// </summary>
    public ITeam GetStartingTeam() => IsTeamA_Starting ? TeamA : TeamB;

    /// <summary>
    /// Das NICHT anspielende Team
    /// </summary>
    public ITeam GetNotStartingTeam() => IsTeamA_Starting ? TeamB : TeamA;

    /// <summary>
    /// Dieses Spiel ist ein Aussetzer, wenn TeamA oder TeamB "virtuell" ist, oder die  Spielbahn 0
    /// </summary>
    public bool IsPauseGame() => TeamA.IsVirtual || TeamB.IsVirtual || CourtNumber == 0;

    /// <summary>
    /// normales Spiel, kein Aussetzer
    /// </summary>
    public bool IsNotPauseGame() => !IsPauseGame();

    #endregion
}
