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
        using StreamReader reader = new(stream);
        string result = reader.ReadToEnd();
        var jsongameplans = JsonSerializer.Deserialize<IEnumerable<JsonGameplan>>(result);

        foreach (var game in jsongameplans)
        {
            yield return new Gameplan(game.Id, game.Description, game.IsVergleich, game.Teams, game.Courts, game.Plan);
        }
    }


    /// <summary>
    /// Convert the given gameplan to the given temas.
    /// </summary>
    /// <param name="gameplan"></param>
    /// <param name="teams">no virtual teams. without games</param>
    public static void MatchTeamAndGames(IGameplan gameplan, IEnumerable<ITeam> teams, int rounds = 1, bool isStartingChanged = true)
    {
        //Ein liste für normale "Spiele" erzeugen
        var normalGames = new List<IGame>();
        var gameNrOverAll = 1;

        for (int round = 1; round <= rounds; round++)
        {
            foreach (var gamenumber in gameplan.GameplanGamenumbers)
            {
                foreach (var game in gamenumber.Games)
                {
                    var normalGame = Game.Create(
                        teams.FirstOrDefault(t => t.StartNumber == game.A),
                        teams.FirstOrDefault(t => t.StartNumber == game.B),
                        courtNumber: game.Court,
                        gamenumber.Number,
                        roundOfGame: round,
                        gameNumberOverAll: gameNrOverAll,
                        isTeamA_Starting: game.Court % 2 != 0
                        );

                    //Anspiel
                    if (round % 2 == 0 && isStartingChanged) normalGame.IsTeamA_Starting = !normalGame.IsTeamA_Starting;


                    normalGames.Add(normalGame);
                }

                var teamsOnCourt = gamenumber.Games.Select(t => t.A)
                                                   .Union(
                                   gamenumber.Games.Select(t => t.B));

                var teamsOffCourt = Enumerable.Range(1, gameplan.Teams).Except(teamsOnCourt);

                foreach (var offTeam in teamsOffCourt)
                {
                    normalGames.Add(
                        Game.Create(
                        teams.FirstOrDefault(t => t.StartNumber == offTeam),
                        teams.FirstOrDefault(t => t.StartNumber == offTeam),
                        courtNumber: 0, //cause of the 0 it is a breakGame
                        gamenumber.Number,
                        roundOfGame: round,
                        gameNumberOverAll: gameNrOverAll,
                        isTeamA_Starting: false));
                }

                gameNrOverAll++;
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
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("vergleich")]
    public bool IsVergleich { get; set; }

    [JsonPropertyName("teams")]
    public int Teams { get; set; }

    [JsonPropertyName("courts")]
    public int Courts { get; set; }

    [JsonPropertyName("plan")]
    public int[][] Plan { get; set; }
}




