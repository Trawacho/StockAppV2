using Microsoft.Win32;
using StockApp.Comm.NetMqStockTV;
using StockApp.UI.Commands;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace StockApp.UI.Stores;

public interface IStockTVCommandStore
{
    ICommand StockTVCloseCommand { get; }
    ICommand ResetMarketingImageCommand { get; }
    ICommand SetMarketingImageCommand { get; }
    ICommand ShowMarketingCommand { get; }
    ICommand GetResultCommand { get; }
    ICommand ResetResultCommand { get; }
    ICommand DisconnectCommand { get; }
    ICommand ConnectCommand { get; }
    ICommand SendSettingsCommand { get; }
    ICommand GetSettingsCommand { get; }
    ICommand StockTvOpenWebsiteCommand { get; }
    ICommand StockTvSerivceDiscoverCommand { get; }
}

internal class StockTVCommandStore : IStockTVCommandStore
{
    public StockTVCommandStore(IStockTVService stockTVService)
    {
        ResetMarketingImageCommand = new RelayCommand((p) => (p as IStockTV)?.ClearMarketingImage(), (p) => (p as IStockTV)?.IsConnected ?? false);
        StockTVCloseCommand = new RelayCommand((p) => (p as IStockTV)?.RemoveFromCollection());
        SetMarketingImageCommand = CreateSetMarketingImageCommand();
        GetSettingsCommand = new RelayCommand((p) => (p as IStockTV)?.TVSettingsGet(), (p) => (p as IStockTV)?.IsConnected ?? false);
        SendSettingsCommand = new RelayCommand((p) => (p as IStockTV)?.TVSettingsSend(), (p) => (p as IStockTV)?.IsConnected ?? false);
        ConnectCommand = new RelayCommand((p) => (p as IStockTV)?.Connect(), (p) => !(p as IStockTV)?.IsConnected ?? false);
        DisconnectCommand = new RelayCommand((p) => (p as IStockTV)?.Disconnect(), (p) => (p as IStockTV)?.IsConnected ?? false);
        ResetResultCommand = new RelayCommand((p) => (p as IStockTV)?.TVResultReset(), (p) => (p as IStockTV)?.IsConnected ?? false);
        GetResultCommand = new RelayCommand((p) => (p as IStockTV)?.TVSettingsGet(), (p) => (p as IStockTV)?.IsConnected ?? false);
        ShowMarketingCommand = new RelayCommand((p) => (p as IStockTV)?.ShowMarketing(), (p) => (p as IStockTV)?.IsConnected ?? false);
        StockTvOpenWebsiteCommand = new RelayCommand((p) => OpenWebsite((p as IStockTV)?.Url));
        StockTvSerivceDiscoverCommand = new RelayCommand((p) => stockTVService?.Discover());
    }

    static void OpenWebsite(string url)
    {
        if (url == null) return;

        try
        {
            Process.Start(url);
        }
        catch
        {
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }

    static ICommand CreateSetMarketingImageCommand()
    {
        return new RelayCommand((p) =>
        {
            if (GetFile(out string fileName, out byte[] file))
            {
                (p as IStockTV)?.SetMarketingImage(file, fileName);
            }
        },
        (p) => (p as IStockTV)?.IsConnected ?? false);
    }

    static bool GetFile(out string fileName, out byte[] file)
    {
        var ofd = new OpenFileDialog()
        {
            Filter = "ImageFiles|*.jpg;*.jpeg;*.bmp",
            Multiselect = false,
            Title = "Bilddatei für StockTV auswählen",
        };

        if (true == ofd.ShowDialog())
        {
            using var stream = ofd.OpenFile();
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            fileName = ofd.SafeFileName;
            file = ms.ToArray();
            return true;
        }

        file = null;
        fileName = null;
        return false;

    }


    #region Commands

    public ICommand StockTVCloseCommand { get; }

    public ICommand ResetMarketingImageCommand { get; }

    public ICommand SetMarketingImageCommand { get; }

    public ICommand ShowMarketingCommand { get; }

    public ICommand GetResultCommand { get; }
    public ICommand ResetResultCommand { get; }

    public ICommand DisconnectCommand { get; }

    public ICommand ConnectCommand { get; }

    public ICommand SendSettingsCommand { get; }

    public ICommand GetSettingsCommand { get; }

    public ICommand StockTvOpenWebsiteCommand { get; }

    public ICommand StockTvSerivceDiscoverCommand { get; }

    #endregion

}