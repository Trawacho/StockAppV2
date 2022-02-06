using StockApp.Comm.NetMqStockTV;
using StockApp.UI.Commands;
using StockApp.UI.Stores;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;

public class StockTVViewModel : ViewModelBase
{
    #region readonly Fields

    private readonly IStockTV _stockTv;
    private readonly IStockTVCommandStore _stockTVCommandStore;
    private readonly IStockTVSettings _stockTVSettings;

    #endregion

    public StockTVViewModel(IStockTV stockTv, IStockTVCommandStore stockTVCommandStore)
    {
        _stockTv = stockTv;
        _stockTVCommandStore = stockTVCommandStore;
        _stockTVSettings = _stockTv.TVSettings;
        _stockTv.StockTVSettingsChanged += StockTVSettings_Changed;
        _stockTv.StockTVOnlineChanged += StockTVOnline_Changed;
        _stockTv.StockTVDirectorChanged += StockTVDirectorChanged;

        ColorModes = Enum.GetValues(typeof(ColorMode)).Cast<ColorMode>().ToList();
        GameModes = Enum.GetValues(typeof(GameMode)).Cast<GameMode>().ToList();
        NextCourts = Enum.GetValues(typeof(NextCourtMode)).Cast<NextCourtMode>().ToList();
        GameGroups = new List<string>() { "--", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };
    }



    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _stockTv.StockTVSettingsChanged -= StockTVSettings_Changed;
                _stockTv.StockTVOnlineChanged -= StockTVOnline_Changed;
                _stockTv.StockTVDirectorChanged -= StockTVDirectorChanged;
            }
            _disposed = true;
        }
        base.Dispose(disposing);
    }

    private void StockTVDirectorChanged(object sender, bool e)
    {
        RaisePropertyChanged(nameof(Director));
    }

    private void StockTVOnline_Changed(object sender, bool e)
    {
        RaisePropertyChanged(nameof(IsConnected));
        RaisePropertyChanged(nameof(IsNotConnected));
        RaisePropertyChanged(nameof(Identifier));
    }

    private void StockTVSettings_Changed(object sender, PropertyChangedEventArgs e)
    {

        switch (e.PropertyName)
        {
            case nameof(IStockTVSettings.Bahn):
                RaisePropertyChanged(nameof(this.CourtNumber));
                break;
            case nameof(IStockTVSettings.Spielgruppe):
                RaisePropertyChanged(nameof(this.GameGroup));
                break;
            case nameof(IStockTVSettings.NextBahnModus):
                RaisePropertyChanged(nameof(this.NextCourt));
                break;
            case nameof(IStockTVSettings.ColorModus):
                RaisePropertyChanged(nameof(this.ColorMode));
                break;
            case nameof(IStockTVSettings.PointsPerTurn):
                RaisePropertyChanged(nameof(this.PointsPerTurn));
                break;
            case nameof(IStockTVSettings.TurnsPerGame):
                RaisePropertyChanged(nameof(this.TurnsPerGame));
                break;
            case nameof(IStockTVSettings.MidColumnLength):
                RaisePropertyChanged(nameof(this.MidColumnLength));
                break;
            case nameof(IStockTVSettings.GameModus):
                RaisePropertyChanged(nameof(this.GameMode));
                break;

            default:
                break;
        }
    }

    #region Value lists

    public IList<string> GameGroups { get; }
    public IList<NextCourtMode> NextCourts { get; }
    public IList<ColorMode> ColorModes { get; }
    public IList<GameMode> GameModes { get; }

    #endregion

    #region Properties

    #region readonly - Properties

    public IStockTV StockTV => _stockTv;

    public string HostName => _stockTv?.HostName;
    public string IpAddress => _stockTv?.IPAddress;
    public string FirmwareVersion => $"{_stockTv?.FW ?? ""}";
    public string Identifier => $"{HostName}" + Environment.NewLine + $"IP: {IpAddress}" + Environment.NewLine + $"Fw: v{FirmwareVersion}{Environment.NewLine}{(IsConnected ? "verbunden" : "getrennt")}";
    public string Url => _stockTv?.Url;
    public bool IsConnected => _stockTv?.IsConnected ?? false;
    public bool IsNotConnected => !IsConnected;

    #endregion


    public int CourtNumber
    {
        get => _stockTVSettings?.Bahn ?? 0;
        set
        {
            if (_stockTVSettings.Bahn == value) return;
            _stockTVSettings.Bahn = value;
            RaisePropertyChanged();
        }
    }

    public string GameGroup
    {
        get => _stockTVSettings?.Spielgruppe switch { 1 => "A", 2 => "B", 3 => "C", 4 => "D", 5 => "E", 6 => "F", 7 => "G", 8 => "H", 9 => "I", 10 => "J", 0 or _ => "--" };
        set
        {
            _stockTVSettings.Spielgruppe = value switch { "A" => 1, "B" => 2, "C" => 3, "D" => 4, "E" => 5, "F" => 6, "G" => 7, "H" => 8, "I" => 9, "J" => 10, _ => 0 };
            RaisePropertyChanged();
        }
    }

    public NextCourtMode NextCourt
    {
        get => _stockTVSettings?.NextBahnModus ?? NextCourtMode.Left;
        set
        {
            if (_stockTVSettings.NextBahnModus == value) return;
            _stockTVSettings.NextBahnModus = value;
            RaisePropertyChanged();
        }
    }

    public ColorMode ColorMode
    {
        get => _stockTVSettings?.ColorModus ?? ColorMode.Normal;
        set
        {
            if (_stockTVSettings.ColorModus == value) return;
            _stockTVSettings.ColorModus = value;
            RaisePropertyChanged();
        }
    }

    public GameMode GameMode
    {
        get => _stockTVSettings?.GameModus ?? GameMode.Training;
        set
        {
            if (_stockTVSettings.GameModus == value) return;
            _stockTVSettings.GameModus = value;
            RaisePropertyChanged();

        }
    }

    public int PointsPerTurn
    {
        get => _stockTVSettings?.PointsPerTurn ?? 15;
        set
        {
            if (_stockTVSettings.PointsPerTurn == value) return;
            _stockTVSettings.PointsPerTurn = value;
            RaisePropertyChanged();
        }
    }

    public int TurnsPerGame
    {
        get => _stockTVSettings?.TurnsPerGame ?? 6;
        set
        {
            if (_stockTVSettings.TurnsPerGame == value) return;
            _stockTVSettings.TurnsPerGame = value;
            RaisePropertyChanged();
        }
    }

    public int MidColumnLength
    {
        get => _stockTVSettings?.MidColumnLength ?? 8;
        set
        {
            if (_stockTVSettings.MidColumnLength == value) return;
            _stockTVSettings.MidColumnLength = value;
            RaisePropertyChanged();
        }
    }

    public bool SendSettingsDirekt
    {
        get => _stockTv?.UpdateImmediately ?? false;
        set
        {
            if (_stockTv?.UpdateImmediately == value) return;
            _stockTv.UpdateImmediately = value;
            RaisePropertyChanged();
        }
    }

    public bool Director
    {
        get => _stockTv?.Director ?? false;
        set
        {
            if (_stockTv.Director == value) return;
            _stockTv.Director = value;
            RaisePropertyChanged(nameof(Director));
        }
    }

    #endregion

    #region Commands
    public ICommand OnLoadedCommand => new RelayCommand((p) => SendSettingsDirekt = true);

    public ICommand GetSettingsCommand => _stockTVCommandStore?.GetSettingsCommand;
    public ICommand SendSettingsCommand => _stockTVCommandStore?.SendSettingsCommand;
    public ICommand ResetResultCommand => _stockTVCommandStore?.ResetResultCommand;
    public ICommand GetResultCommand => _stockTVCommandStore?.GetResultCommand;
    public ICommand ShowMarketingCommand => _stockTVCommandStore?.ShowMarketingCommand;
    public ICommand SetMarketingImageCommand => _stockTVCommandStore?.SetMarketingImageCommand;
    public ICommand ResetMarketingImageCommand => _stockTVCommandStore?.ResetMarketingImageCommand;
    public ICommand ConnectCommand => _stockTVCommandStore?.ConnectCommand;
    public ICommand DisconnectCommand => _stockTVCommandStore?.DisconnectCommand;
    public ICommand StockTvCloseCommand => _stockTVCommandStore?.StockTVCloseCommand;
    public ICommand StockTvOpenWebsiteCommand => _stockTVCommandStore?.StockTvOpenWebsiteCommand;

    #endregion

}


