namespace StockApp.Core.Wettbewerb.Teambewerb;

/// <summary>
/// Evaluates whether §610 of the rulebook applies and adjusts game results accordingly.
/// §610: Tournament can be aborted early if more than 50% of planned games were played.
/// All teams count equally many games (excluding pauses).
/// For teams that played more games than the minimum, the last game is excluded.
/// </summary>
public static class Paragraph610Evaluator
{
    /// <summary>
    /// Checks if §610 applies: more than 50% of planned games were played (excluding pauses).
    /// </summary>
    public static bool IsApplicable(ITeamBewerb teamBewerb)
    {
        var allGames = teamBewerb.GetAllGames(withBreaks: false).ToList();
        var playedGames = allGames.Where(g => g.IsGameDone(false)).ToList();

        if (allGames.Count == 0)
            return false;

        return playedGames.Count > allGames.Count / 2.0;
    }

    /// <summary>
    /// Returns a dictionary of teams to their games that should be counted under §610.
    /// Games are adjusted so all teams have the same count (minimum across all teams).
    /// If a team has more games than the minimum, its last game is excluded.
    /// </summary>
    public static IReadOnlyDictionary<ITeam, IList<IGame>> GetAdjustedGames(ITeamBewerb teamBewerb)
    {
        var result = new Dictionary<ITeam, IList<IGame>>();

        // Get all played games (without pauses) for each team
        var teamGamesMap = new Dictionary<ITeam, List<IGame>>();
        foreach (var team in teamBewerb.Teams)
        {
            var playedGamesForTeam = team.Games
                .Where(g => !g.IsPauseGame() && g.IsGameDone(false))
                .OrderBy(g => g.GameNumberOverAll)
                .ToList();

            teamGamesMap[team] = playedGamesForTeam;
        }

        // Find minimum number of games across all teams
        int minGames = teamGamesMap.Values.Min(games => games.Count);

        // Adjust: keep only minGames for each team, excluding the last if needed
        foreach (var team in teamBewerb.Teams)
        {
            var games = teamGamesMap[team];
            var adjustedGames = games.Take(minGames).ToList();
            result[team] = adjustedGames;
        }

        return result;
    }
}
