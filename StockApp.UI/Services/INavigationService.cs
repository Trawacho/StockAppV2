using StockApp.UI.ViewModels;

namespace StockApp.UI.Services;
public interface INavigationService<TViewModel> where TViewModel : ViewModelBase
{
    void Navigate();
}
