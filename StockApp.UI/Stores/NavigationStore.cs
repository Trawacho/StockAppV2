using StockApp.Lib.ViewModels;
using System;

namespace StockApp.UI.Stores;

public class NavigationStore : INavigationStore
{
    public event EventHandler CurrentViewModelChanged;

    private ViewModelBase _currentViewModel;
    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            _currentViewModel?.Dispose();
            _currentViewModel = value;
            OnCurrentViewModelChanged();
        }
    }

    private void OnCurrentViewModelChanged()
    {
        CurrentViewModelChanged?.Invoke(this, EventArgs.Empty);
    }
}

public interface INavigationStore
{
    event EventHandler CurrentViewModelChanged;
    ViewModelBase CurrentViewModel { get; set; }
}



