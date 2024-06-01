using StockApp.Lib.ViewModels;
using StockApp.UI.Commands;
using StockApp.UI.Services;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;

public class NavigtaionZielViewModel : ViewModelBase
{
    public NavigtaionZielViewModel(
            INavigationService<ZielBewerbViewModel> zielTeilnehmerNavigationService,
            IDialogService<LiveResultsZielViewModel> liveResultZielDialogCommand,
            INavigationService<ZielBewerbDruckViewModel> zielDruckNavigationService)
    {

        NavigateTeilnehmerCommand = new NavigateCommand<ZielBewerbViewModel>(zielTeilnehmerNavigationService);
        NavigateLiveResultCommand = new DialogCommand<LiveResultsZielViewModel>(liveResultZielDialogCommand);
        NavigateDruckResultCommand = new NavigateCommand<ZielBewerbDruckViewModel>(zielDruckNavigationService);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {

            }
        }
        base.Dispose(disposing);
    }

    public ICommand NavigateTeilnehmerCommand { get; }
    public ICommand NavigateLiveResultCommand { get; }
    public ICommand NavigateDruckResultCommand { get; }
}
