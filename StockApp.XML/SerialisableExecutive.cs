using StockApp.Core.Turnier;

namespace StockApp.XML;

public class SerialisableExecutive : IExecutive
{
    public SerialisableExecutive()
    {

    }
    public SerialisableExecutive(IExecutive executive)
    {
        Name = executive.Name; ;
        ClubName = executive.ClubName;
    }
    public void ToNormal(IExecutive normal)
    {
        normal.ClubName = ClubName;
        normal.Name = Name;
    }

    public string Name { get; set; }
    public string ClubName { get; set; }

}