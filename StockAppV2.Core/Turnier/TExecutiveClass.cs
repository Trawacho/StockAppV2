namespace StockApp.Core.Turnier;

public interface IExecutive
{
    string Name { get; set; }
    string ClubName { get; set; }
}

public abstract class TExecutiveClass : IExecutive
{
    private string _name;
    private string _clubName;

    public string Name { get => _name; set => _name = value.Trim(); }

    public string ClubName { get => _clubName; set => _clubName = value.Trim(); }
}
