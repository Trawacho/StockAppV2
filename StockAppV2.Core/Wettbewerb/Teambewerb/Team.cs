namespace StockApp.Core.Wettbewerb.Teambewerb;


public interface ITeam : IEquatable<ITeam>
{
	public int StartNumber { get; set; }
	public string TeamName { get; set; }
	/// <summary>
	/// TeamName mit Erweiterung bei Strafen oder Status != normal
	/// </summary>
	public string TeamNamePublic { get; }
	public int StrafSpielpunkte { get; set; }

	/// <summary>
	/// normal, entschuldigt, unentschuldigt, usw
	/// </summary>
	public TeamStatus TeamStatus { get; set; }

	/// <summary>
	/// Welche Info zu dem Verein soll angezeigt werden, Kreis, Region, usw...
	/// </summary>
	public TeamInfo TeamInfo { get; set; }

	/// <summary>
	/// the first 25 characters from <see cref="TeamName"/>
	/// </summary>
	public string TeamNameShort { get; }
	public string Nation { get; set; }
	public string Region { get; set; }
	public string Bundesland { get; set; }
	public string Kreis { get; set; }
	public IEnumerable<IPlayer> Players { get; }
	public IReadOnlyCollection<IGame> Games { get; }
	public IEnumerable<int> SpieleMitAnspiel();
	public IEnumerable<int> SpieleAufStartSeite();
	/// <summary>
	/// TRUE, if each game as a Result
	/// </summary>
	/// <returns></returns>
	public bool IsEachGameDone(bool live);

	/// <summary>
	/// Add a game
	/// </summary>
	/// <param name="game"></param>
	void AddGame(IGame game);

	/// <summary>
	/// Delete all Games 
	/// </summary>
	void ClearGames();

	/// <summary>
	/// Occours, when a Game is added or removed
	/// </summary>
	event EventHandler GamesChanged;

	void AddPlayer();
	void AddPlayer(IPlayer player);
	void RemovePlayer(IPlayer selectedPlayer);

	/// <summary>
	/// Occours, when a Player is added or removed
	/// </summary>
	event EventHandler PlayersChanged;


	IOrderedEnumerable<IGrouping<int, IGame>> GetGamesGroupedByRound();
	(int positiv, int negativ) GetSpielPunkte(bool live = false);
	double GetStockNote(bool live = false);
	(int positiv, int negativ) GetStockPunkte(bool live = false);
	int GetStockPunkteDifferenz(bool live = false);
}

public class Team : ITeam
{
	#region Fields

	private readonly List<IGame> _games = new();
	private readonly List<IPlayer> _players = new();
	private string _teamName;

	#endregion

	#region Events

	public event EventHandler PlayersChanged;

	protected virtual void RaisePlayersChanged()
	{
		var handler = PlayersChanged;
		handler?.Invoke(this, EventArgs.Empty);
	}


	public event EventHandler GamesChanged;

	protected virtual void RaiseGamesChanged()
	{
		var handler = GamesChanged;
		handler?.Invoke(this, EventArgs.Empty);
	}

	#endregion


	#region IEquatable implementation

	/// <summary>
	/// If a Team is equal to another Team depends only on the StartNumber
	/// </summary>
	/// <param name="other"></param>
	/// <returns></returns>
	public bool Equals(ITeam other)
	{
		return this.StartNumber == other.StartNumber;
	}

	#endregion

	#region Standard-Properties

	/// <summary>
	/// Startnummer
	/// </summary>
	public int StartNumber { get; set; }

	/// <summary>
	/// Teamname
	/// </summary>
	public string TeamName { get => _teamName; set => _teamName = value; }

	public string TeamNamePublic => TeamStatus != TeamStatus.Normal
									   ? String.Concat(TeamName, " (", TeamStatus.Abbreviation(), ")")
									   : StrafSpielpunkte > 0
										   ? string.Concat(TeamName, " §: ", StrafSpielpunkte)
										   : TeamName;

	public int StrafSpielpunkte { get; set; }
	
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public TeamStatus TeamStatus { get; set; }
	
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public TeamInfo TeamInfo { get; set; }

	/// <summary>
	/// First 25 Characters from <see cref="TeamName"/>
	/// </summary>
	public string TeamNameShort => new(TeamName?.Take(25).ToArray() ?? Array.Empty<char>());
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public string Nation { get; set; }
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public string Region { get; set; }
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public string Bundesland { get; set; }
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public string Kreis { get; set; }

	/// <summary>
	/// Liste aller Spieler
	/// </summary>
	public IEnumerable<IPlayer> Players => _players.AsReadOnly();

	/// <summary>
	/// Maximum Number of Players for a Team
	/// </summary>
	public static int MaxNumberOfPlayers { get; } = 6;

	/// <summary>
	/// Minimum Number of Players for a Team
	/// </summary>
	public static int MinNumberOfPlayer { get; } = 0;

	/// <summary>
	/// Liste aller Spiele 
	/// </summary>
	public IReadOnlyCollection<IGame> Games => _games.AsReadOnly();

	/// <summary>
	/// Alle Spielnummern, bei denen die Mannschaft auf der "grünen" bzw. Start-Seite steht.
	/// Das Property wird für den NetworkService und die WertungskarteTV benötigt.
	/// </summary>
	public IEnumerable<int> SpieleAufStartSeite()
	{
		/* 
         * Im NetworkService werden pro Bahn die Ergebnisse übertragen. Durch das Property
         * --> IsDirectionOfCourtsFromRightToLeft <-- im Tournament wird die Richtung der Bahnnummer eingestellt
         * So kann erkannt werden ob die "steigenedeMannschaft" links oder rechts auf der Bahn steht. Die Ergebnisse                 
         * im NetworkService können dann zugeordnet werden
         * Bsp: Bahn1 befindet sich rechts, die weiteren Bahnen links davon
         * Mannschaft1 befindet sich im 1. Spiel somit auf der rechten Seite des Spielfelds
         * Die nächsten Spiele bis zur letzten Bahn ganz links sind somit "steigende Spiele", die Mannschaft1 befindet
         * sich bei diesen Spielen immer rechts. 
         */

		// Ergebnisliste mit SpielNummern (GameNumberOverAll) 
		var result = new List<int>();

		// Nach SpielNummer sortierte Liste
		var sortedGames = Games.OrderBy(g => g.GameNumberOverAll).ToList();

		for (int i = 0; i < sortedGames.Count; i++)
		{
			if (sortedGames[i].TeamA == this && !sortedGames[i].IsPauseGame())
				result.Add(sortedGames[i].GameNumberOverAll);
		}
		return result;
	}

	public IEnumerable<int> SpieleMitAnspiel()
	{
		var result = new List<int>();
		var sortedGames = Games.OrderBy(g => g.GameNumberOverAll).ToList();

		for (int i = 0; i < sortedGames.Count; i++)
		{
			if (sortedGames[i].GetStartingTeam() == this)
			{
				result.Add(sortedGames[i].GameNumberOverAll);
			}
		}

		return result;
	}

	public bool IsEachGameDone(bool live = false) =>
		Games.Where(g => !g.IsPauseGame() && g.GetOpponent(this).TeamStatus == TeamStatus.Normal)
			 .All(g => g.IsGameDone(live));



	#endregion

	#region Result Functions

	public (int positiv, int negativ) GetSpielPunkte(bool live = false)
	{
		int pos = Games.Where(g => g.TeamA == this && g.TeamB.TeamStatus == TeamStatus.Normal).Sum(s => s.Spielstand.GetSpielPunkteTeamA(live)) +
				  Games.Where(g => g.TeamB == this && g.TeamA.TeamStatus == TeamStatus.Normal).Sum(s => s.Spielstand.GetSpielPunkteTeamB(live));

		int neg = Games.Where(g => g.TeamA != this && g.TeamA.TeamStatus == TeamStatus.Normal).Sum(s => s.Spielstand.GetSpielPunkteTeamA(live)) +
				  Games.Where(g => g.TeamB != this && g.TeamB.TeamStatus == TeamStatus.Normal).Sum(s => s.Spielstand.GetSpielPunkteTeamB(live));

		return TeamStatus == TeamStatus.Normal
			? (pos - StrafSpielpunkte, neg)
			: (0, 0);
	}

	public (int positiv, int negativ) GetStockPunkte(bool live = false)
	{
		int pos = Games.Where(g => g.TeamA == this && g.TeamB.TeamStatus == TeamStatus.Normal).Sum(s => s.Spielstand.GetStockPunkteTeamA(live)) +
				  Games.Where(g => g.TeamB == this && g.TeamA.TeamStatus == TeamStatus.Normal).Sum(s => s.Spielstand.GetStockPunkteTeamB(live));

		int neg = Games.Where(g => g.TeamA != this && g.TeamA.TeamStatus == TeamStatus.Normal).Sum(s => s.Spielstand.GetStockPunkteTeamA(live)) +
				  Games.Where(g => g.TeamB != this && g.TeamB.TeamStatus == TeamStatus.Normal).Sum(s => s.Spielstand.GetStockPunkteTeamB(live));

		return TeamStatus == TeamStatus.Normal ? (pos, neg) : (0, 0);
	}

	public double GetStockNote(bool live = false)
	{
		var (positiv, negativ) = GetStockPunkte(live);
		return negativ == 0
				? positiv
				: Math.Round((double)positiv / negativ, 3);
	}


	public int GetStockPunkteDifferenz(bool live = false)
	{
		var (positiv, negativ) = GetStockPunkte(live);
		return positiv - negativ;
	}


	#endregion

	#region Constructor

	/// <summary>
	/// Constructor 
	/// </summary>
	/// <param name="TeamName"></param>
	private Team(string TeamName)
	{
		StartNumber = 0;
		this.TeamName = TeamName;
		TeamStatus = TeamStatus.Normal;
		StrafSpielpunkte = 0;
	}

	public static ITeam Create(string teamName) => new Team(teamName);



	#endregion

	#region Functions

	/// <summary>
	/// Deletes alle Games
	/// </summary>
	public void ClearGames()
	{
		_games.Clear();
		RaiseGamesChanged();
	}

	/// <summary>
	/// Ein Spiel anhängen
	/// </summary>
	/// <param name="game"></param>
	public void AddGame(IGame game)
	{
		_games.Add(game);
		RaiseGamesChanged();
	}

	/// <summary>
	/// Alle Spiele gruppiert nach Runde
	/// </summary>
	/// <returns></returns>
	public IOrderedEnumerable<IGrouping<int, IGame>> GetGamesGroupedByRound()
	{
		return from game in Games.OrderBy(r => r.RoundOfGame)
								 .ThenBy(g => g.GameNumber)
			   group game by game.RoundOfGame into grGames
			   orderby grGames.Key
			   select grGames;
	}

	/// <summary>
	/// Einen Spieler der Mannschaft entfernen
	/// </summary>
	/// <param name="selectedPlayer"></param>
	public void RemovePlayer(IPlayer selectedPlayer)
	{
		_players.Remove(selectedPlayer);
		RaisePlayersChanged();
	}


	/// <summary>
	/// Einen Spieler der Mannschaft anfügen
	/// </summary>
	public void AddPlayer() => AddPlayer(Player.Create());

	public void AddPlayer(IPlayer player)
	{
		_players.Add(player);
		RaisePlayersChanged();
	}

	/// <summary>
	/// Startnummer und Mannschaftsname
	/// </summary>
	/// <returns></returns>
	public override string ToString() => $"{StartNumber}. {TeamName}";

	#endregion

}

