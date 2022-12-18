using StockApp.Core.Wettbewerb.Teambewerb;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StockApp.Core.Factories;

public static class GamePlanFactory
{
    public static IEnumerable<IGameplan> LoadAllGameplans()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "StockApp.Core.Factories.gpf.json";

        using Stream stream = assembly.GetManifestResourceStream(resourceName);
        using StreamReader reader = new StreamReader(stream);
        string result = reader.ReadToEnd();
        var jsongameplans= JsonSerializer.Deserialize<IEnumerable<JsonGameplan>>(result);
    
        foreach (var game in jsongameplans)
        {
            yield return new Gameplan(game.Description, game.Teams, game.Courts, game.Plan);
        }
    }

    

    public static void MatchTeamAndGames(IGameplan factoryGames, IEnumerable<Wettbewerb.Teambewerb.ITeam> teams)
    {
        //Entferne alle Spiele von allen Teams
        foreach (var t in teams)
            t.ClearGames();

        //Ein liste für normale "Spiele" erzeugen
        var normalGames = new List<IGame>();

        foreach (var round in factoryGames.GameplanRounds)
        {
            foreach (var gamenumber in round.GameplanGamenumbers)
            {
                foreach (var game in gamenumber.Games)
                {
                    var normalGame = Wettbewerb.Teambewerb.Game.Create(
                        teams.FirstOrDefault(t => t.StartNumber == game.A),
                        teams.FirstOrDefault(t => t.StartNumber == game.B),
                        courtNumber: game.Court,
                        gamenumber.Number,
                        roundOfGame: round.Round,
                        gameNumberOverAll: gamenumber.Number * round.Round,
                        game.Court % 2 != 0);
                    normalGames.Add(normalGame);
                }
            }
        }

        // den Mannschaften die Spiele zuweisen
        foreach (var team in teams)
        {
            var teamGames = normalGames.Where(g => g.TeamA.StartNumber == team.StartNumber || g.TeamB?.StartNumber == team.StartNumber);
            foreach (var game in teamGames)
                team.AddGame(game);
        }
    }
}



public class JsonGameplan
{
    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("teams")]
    public int Teams { get; set; }

    [JsonPropertyName("courts")]
    public int Courts { get; set; }

    [JsonPropertyName("plan")]
    public int[][] Plan { get; set; }
}

public interface IGameplan
{
    string Name { get; }
    int Courts { get; }
    int Teams { get; }
    IEnumerable<IGameplanRound> GameplanRounds { get; }
}

public class Gameplan : IGameplan
{
    public Gameplan(string name, int teams, int courts, int[][] plan, int numberOfRoundsToCreate = 1)
    {
        _gameplanRounds = new List<IGameplanRound>();
        Name = name;
        Teams = teams;
        Courts = courts;

        for (int i = 0; i < numberOfRoundsToCreate; i++)
        {
            _gameplanRounds.Add(new GameplanRound(i + 1, plan));
        }
    }

    public string Name { get; }

    public int Courts { get; }

    public int Teams { get; }

    private List<IGameplanRound> _gameplanRounds;
    public IEnumerable<IGameplanRound> GameplanRounds => _gameplanRounds;
}

public interface IGameplanRound
{
    int Round { get; }
    IEnumerable<IGameplanGamenumber> GameplanGamenumbers { get; }
}

public class GameplanRound : IGameplanRound
{
    public GameplanRound(int round, int[][] plan)
    {
        _gameplanGamenumbers = new List<IGameplanGamenumber>();

        Round = round;
        for (int row = 0; row < plan.GetLength(0); row++)
        {
            _gameplanGamenumbers.Add(new GameplanGamenumber(row + 1, plan[row]));
        }
    }

    public int Round { get; }

    private List<IGameplanGamenumber> _gameplanGamenumbers;
    public IEnumerable<IGameplanGamenumber> GameplanGamenumbers => _gameplanGamenumbers;
}

public interface IGameplanGamenumber
{
    int Number { get; }
    IEnumerable<IGameplanGame> Games { get; }
}

public class GameplanGamenumber : IGameplanGamenumber
{
    public GameplanGamenumber(int number, int[] games)
    {
        _games = new List<IGameplanGame>();
        Number = number;
        int court = 1;
        for (int i = 0; i < games.Length; i += 2)
        {
            _games.Add(new GameplanGame(games[i], games[i + 1], court));
            court++;
        }
    }

    public int Number { get; }

    private List<IGameplanGame> _games;
    public IEnumerable<IGameplanGame> Games => _games;
}

public interface IGameplanGame
{
    int A { get; }
    int B { get; }
    int Court { get; }
}

public class GameplanGame : IGameplanGame
{
    public GameplanGame(int a, int b, int court)
    {
        A = a;
        B = b;
        Court = court;
    }

    public int A { get; }

    public int B { get; }

    public int Court { get; }
}
