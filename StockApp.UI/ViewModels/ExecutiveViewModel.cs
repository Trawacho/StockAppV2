using StockApp.Core.Turnier;
using StockApp.Lib.ViewModels;

namespace StockApp.UI.ViewModels;

public class ExecutiveViewModel : ViewModelBase, IExecutiveViewModel
{
    private readonly IExecutive _executive;

    public string Name
    {
        get => _executive.Name;
        set
        {
            _executive.Name = value;
            RaisePropertyChanged();
        }
    }

    public string ClubName
    {
        get => _executive.ClubName;
        set
        {
            _executive.ClubName = value;
            RaisePropertyChanged();
        }
    }

    public ExecutiveViewModel(IExecutive executive)
    {
        _executive = executive;
    }
}

public class ExecutiveDesignViewModel : IExecutiveViewModel
{
    public string Name { get; set; }
    public string ClubName { get; set; }
}

public interface IExecutiveViewModel
{
    string Name { get; set; }
    string ClubName { get; set; }
}
