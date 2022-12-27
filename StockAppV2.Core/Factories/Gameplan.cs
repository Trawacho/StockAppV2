namespace StockApp.Core.Factories;

public class Gameplan : IGameplan
{
    public Gameplan(int id, string name, int teams, int courts, int[][] plan)
    {
        _gameplanGamenumbers = new List<IGameplanGameround>();

        ID = id;
        Name = name;
        Teams = teams;
        Courts = courts;

        for (int row = 0; row < plan.GetLength(0); row++)
        {
            _gameplanGamenumbers.Add(new GameplanGamenumber(row + 1, plan[row]));
        }
    }

    public int ID { get; init; }
    public string Name { get; init; }

    public int Courts { get; init; }

    public int Teams { get; init; }

    private readonly List<IGameplanGameround> _gameplanGamenumbers;
    public IEnumerable<IGameplanGameround> GameplanGamenumbers => _gameplanGamenumbers;

    public IEnumerable<( int gamenumber,  IGameplanGame game)> GetAllGames()
    {
        foreach (var r in GameplanGamenumbers)
        {
            foreach (var g in r.Games)
            {
                yield return (r.Number, g);
            }
        }
    }
}



public class GameplanGamenumber : IGameplanGameround
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

    private readonly List<IGameplanGame> _games;
    public IEnumerable<IGameplanGame> Games => _games;
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


public interface IGameplan
{
    int ID { get; }
    string Name { get; }
    int Courts { get; }
    int Teams { get; }
    IEnumerable<IGameplanGameround> GameplanGamenumbers { get; }
    IEnumerable<(int gamenumber, IGameplanGame game)> GetAllGames();
}

public interface IGameplanGameround
{
    int Number { get; }
    IEnumerable<IGameplanGame> Games { get; }
}

public interface IGameplanGame
{
    int A { get; }
    int B { get; }
    int Court { get; }
}
