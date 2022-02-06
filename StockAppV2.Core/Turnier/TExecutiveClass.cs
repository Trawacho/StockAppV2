namespace StockApp.Core.Turnier;

public interface IExecutive
{
    string Name { get; set; }
    string ClubName { get; set; }
}

public abstract class TExecutiveClass : IExecutive
{
    public string Name { get; set; }

    public string ClubName { get; set; }
}
