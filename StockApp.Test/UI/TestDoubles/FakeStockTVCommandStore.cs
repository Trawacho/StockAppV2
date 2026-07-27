using StockApp.UI.Stores;
using System.Windows.Input;

namespace StockApp.Test.UI.TestDoubles;

/// <summary>
/// Minimaler Test-Double für <see cref="IStockTVCommandStore"/> - alle Commands bleiben null,
/// da die hier getesteten ViewModels (z.B. StockTVCollectionViewModel) sie nur durchreichen,
/// nie selbst ausführen.
/// </summary>
public class FakeStockTVCommandStore : IStockTVCommandStore
{
    public ICommand StockTVCloseCommand => null;
    public ICommand ResetMarketingImageCommand => null;
    public ICommand SetMarketingImageCommand => null;
    public ICommand ShowMarketingCommand => null;
    public ICommand GetResultCommand => null;
    public ICommand ResetResultCommand => null;
    public ICommand DisconnectCommand => null;
    public ICommand ConnectCommand => null;
    public ICommand SendSettingsCommand => null;
    public ICommand GetSettingsCommand => null;
    public ICommand StockTvOpenWebsiteCommand => null;
    public ICommand StockTvSerivceDiscoverCommand => null;
}
