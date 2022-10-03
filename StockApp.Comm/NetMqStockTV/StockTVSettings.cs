using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StockApp.Comm.NetMqStockTV;

public interface IStockTVSettings : IEquatable<IStockTVSettings>
{
    public int Bahn { get; set; }
    public int Spielgruppe { get; set; }
    public int PointsPerTurn { get; set; }
    public int TurnsPerGame { get; set; }
    public NextCourtMode NextBahnModus { get; set; }
    public GameMode GameModus { get; set; }
    public ColorMode ColorModus { get; set; }
    public int MidColumnLength { get; set; }
    public int MessageVersion { get; set; }
    byte[] GetSettings();
    void SetSettings(byte[] messageValue);

    event EventHandler<PropertyChangedEventArgs> SettingsChanged;

}

public class StockTVSettings : IStockTVSettings
{
    #region Fields

    private int _bahn;
    private int _pointsPerTurn;
    private int _turnsPerGame;
    private int _spielgruppe;
    private int _midColumnLength;
    private NextCourtMode _nextBahnModus;
    private GameMode _gameModus;
    private ColorMode _colorModus;
    private int _messageVersion;

    public event EventHandler<PropertyChangedEventArgs> SettingsChanged;
    protected virtual void RaiseSettingsChanged([CallerMemberName] string propertyName = null)
    {
        var handler = SettingsChanged;
        handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    #endregion

    #region Contructor

    /// <summary>
    /// Default-Constructor
    /// </summary>
    public StockTVSettings()
    {

    }

    #endregion

    #region Properties

    public int Bahn { get => _bahn; set { _bahn = value; RaiseSettingsChanged(); } }
    public int Spielgruppe { get => _spielgruppe; set { _spielgruppe = value; RaiseSettingsChanged(); } }
    public int PointsPerTurn { get => _pointsPerTurn; set { _pointsPerTurn = value; RaiseSettingsChanged(); } }
    public int TurnsPerGame { get => _turnsPerGame; set { _turnsPerGame = value; RaiseSettingsChanged(); } }
    public NextCourtMode NextBahnModus { get => _nextBahnModus; set { _nextBahnModus = value; RaiseSettingsChanged(); } }
    public GameMode GameModus
    {
        get => _gameModus;
        set
        {
            _gameModus = value;
            {
                switch (value)
                {
                    case GameMode.Training:
                        PointsPerTurn = 30;
                        TurnsPerGame = 30;
                        break;
                    case GameMode.Turnier:
                    case GameMode.BestOf:
                        PointsPerTurn = 15;
                        TurnsPerGame = 6;
                        break;
                    case GameMode.Ziel:
                    default:
                        break;
                }
            }
            RaiseSettingsChanged();
        }
    }
    public ColorMode ColorModus { get => _colorModus; set { _colorModus = value; RaiseSettingsChanged(); } }
    public int MidColumnLength { get => _midColumnLength; set { _midColumnLength = value; RaiseSettingsChanged(); } }
    public int MessageVersion { get => _messageVersion; set { _messageVersion = value; RaiseSettingsChanged(); } }
    #endregion

    #region Implementation IEquatable
    public bool Equals(IStockTVSettings other)
    {
        if (other == null) return false;
        return Bahn == other.Bahn
            && PointsPerTurn == other.PointsPerTurn
            && TurnsPerGame == other.TurnsPerGame
            && NextBahnModus == other.NextBahnModus
            && GameModus == other.GameModus
            && ColorModus == other.ColorModus
            && MidColumnLength == other.MidColumnLength
            && MessageVersion == other.MessageVersion;
    }

    #endregion

    #region Public Functions 

    /// <summary>
    /// Set Values from Byte-Array
    /// </summary>
    /// <param name="valueString"></param>
    public void SetSettings(byte[] value)
    {
        Bahn = value[0];
        Spielgruppe = value[1];
        GameModus = (GameMode)value[2];
        NextBahnModus = (NextCourtMode)value[3];
        ColorModus = (ColorMode)value[4];
        PointsPerTurn = value[5];
        TurnsPerGame = value[6];
        MidColumnLength = value[7];
        MessageVersion = value[8];
        _ = value[9];

    }

    /// <summary>
    /// Get all Settings as Byte-Array
    /// </summary>
    /// <returns></returns>
    public byte[] GetSettings()
    {
        byte[] s = new byte[10];
        s[0] = (byte)Bahn;                          //Bahnnummer
        s[1] = (byte)Spielgruppe;                   //SpielGruppe
        s[2] = Convert.ToByte(GameModus);           //Modus
        s[3] = Convert.ToByte(NextBahnModus);       //Spielrichtung
        s[4] = Convert.ToByte(ColorModus);          //FarbModus (hell,dunkel)
        s[5] = (byte)PointsPerTurn;                 //Anzahl max. Punkte pro Kehre
        s[6] = (byte)TurnsPerGame;                  //Anzahl der Kehren
        s[7] = (byte)MidColumnLength;               //Breite der mittleren Spalte (nur bei der Anzeige von TeamNamen relevant)
        s[8] = (byte)MessageVersion;                //Version des Datenpakets
        s[9] = 0;

        return s;
    }

    #endregion

    #region Static Functions

    internal static IStockTVSettings CreateDefaultSetting(GameMode modus)
    {
        var s = new StockTVSettings()
        {
            Bahn = -1,
            NextBahnModus = NextCourtMode.Left,
            ColorModus = ColorMode.Normal,
            GameModus = modus,
            MidColumnLength = 10,
            MessageVersion = -1
        };
        switch (modus)
        {
            case GameMode.Training:
                s.PointsPerTurn = 30;
                s.TurnsPerGame = 30;
                break;
            case GameMode.Turnier:
            case GameMode.BestOf:
                s.PointsPerTurn = 15;
                s.TurnsPerGame = 6;
                break;
            case GameMode.Ziel:
                break;
            default:
                break;
        }

        return s;
    }

    #endregion

    public override string ToString()
    {
        return $"Bahn:{Bahn} | Spielgruppe:{Spielgruppe} | GameModus:{GameModus} | NextBahn:{NextBahnModus} | ColorModus:{ColorModus} | PointsPerTurn:{PointsPerTurn} | TurnsPerGame:{TurnsPerGame} | MidColumnLength:{MidColumnLength} | MessageVersion:{MessageVersion} ";
    }
}