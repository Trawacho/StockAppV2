using System.ComponentModel;

namespace StockApp.Core.Wettbewerb.Zielbewerb;
//TODO: Enumerations vereinheitlichen oder zusammenführen mit StockTVZielDisziplinName
public enum Disziplinart
{
    MassenMitte = 1,
    Schiessen = 2,
    MassenSeite = 3,
    Kombinieren = 4
}

public interface IDisziplin
{
    /// <summary>
    /// Art der Disziplin dieser 6 Versuche
    /// </summary>
    Disziplinart Disziplinart { get; }
    /// <summary>
    /// Friendly name of Disziplinart
    /// </summary>
    string Name { get; }
    int Versuch1 { get; set; }
    int Versuch2 { get; set; }
    int Versuch3 { get; set; }
    int Versuch4 { get; set; }
    int Versuch5 { get; set; }
    int Versuch6 { get; set; }

    /// <summary>
    /// Summe aller Versuche die >= 0 sind
    /// </summary>
    int Summe { get; }

    /// <summary>
    /// Anzahl der Versuche die eingegeben wurden ( 0 bis 6 )
    /// </summary>
    /// <returns></returns>
    int VersucheCount();

    /// <summary>
    /// Jeder Versuch wird auf -1 gesetzt
    /// </summary>
    void Reset();

    /// <summary>
    /// Wird ausgelöst, wenn sich ein Wert in einem Versuch ändert
    /// </summary>
    event EventHandler ValuesChanged;

    /// <summary>
    /// Einem Versuch einen Wert zuweisen
    /// </summary>
    /// <param name="nr">Nummer des Versuchs (1.....6)</param>
    /// <param name="value">Wert des Versuchs</param>
    void SetVersuch(int nr, int value);
}

internal class Disziplin : IDisziplin
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public event EventHandler ValuesChanged;
    protected void RaiseValuesChanged()
    {
        var handler = ValuesChanged;
        handler?.Invoke(this, EventArgs.Empty);
    }

    private readonly int[] _versuche;

    /// <summary>
    /// Standard Konstruktor
    /// </summary>
    /// <param name="art"></param>
    private Disziplin(Disziplinart art)
    {
        Disziplinart = art;

        _versuche = new int[6];
        _versuche[0] = -1;
        _versuche[1] = -1;
        _versuche[2] = -1;
        _versuche[3] = -1;
        _versuche[4] = -1;
        _versuche[5] = -1;
    }

    public static IDisziplin Create(Disziplinart disziplinart) => new Disziplin(disziplinart);

    #region Properties

    public int Versuch1
    {
        get => _versuche[0];
        set => AddVersuch(0, value);
    }
    public int Versuch2
    {
        get => _versuche[1];
        set => AddVersuch(1, value);
    }
    public int Versuch3
    {
        get => _versuche[2];
        set => AddVersuch(2, value);
    }
    public int Versuch4
    {
        get => _versuche[3];
        set => AddVersuch(3, value);
    }
    public int Versuch5
    {
        get => _versuche[4];
        set => AddVersuch(4, value);
    }
    public int Versuch6
    {
        get => _versuche[5];
        set => AddVersuch(5, value);
    }


    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Disziplinart Disziplinart { get; }


    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Summe => _versuche.Where(x => x >= 0).Sum();

    /// <summary>
    /// <inheritdoc/>>
    /// </summary>
    public string Name
    {
        get
        {
            return Disziplinart switch
            {
                Disziplinart.MassenMitte => "Massen Mitte",
                Disziplinart.MassenSeite => "Massen Seite",
                Disziplinart.Kombinieren => "Kombinieren",
                Disziplinart.Schiessen => "Schiessen",
                _ => throw new InvalidEnumArgumentException("ungültige Disziplinart")
            };
        }
    }

    #endregion

    #region Funktionen

    #region Public

    /// <summary>
    ///<inheritdoc/>
    /// </summary>
    public void Reset()
    {
        Versuch1 = -1;
        Versuch2 = -1;
        Versuch3 = -1;
        Versuch4 = -1;
        Versuch5 = -1;
        Versuch6 = -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="nr"><inheritdoc/></param>
    /// <param name="value"></param>
    public void SetVersuch(int nr, int value)
    {
        AddVersuch(nr - 1, value);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public int VersucheCount() => _versuche.Where(v => v >= 0).Count();

    #endregion

    #region Private

    /// <summary>
    /// Setzt den Wert in das Array der Versuche. Der Wert wird validiert.
    /// </summary>
    /// <param name="index">0 ... 5</param>
    /// <param name="value"></param>
    private void AddVersuch(int index, int value)
    {
        if (value == -1)
        {
            _versuche[index] = value;
        }
        else if (Disziplinart == Disziplinart.Schiessen && IsSchussValue(value))
        {
            _versuche[index] = value;
        }
        else if (Disziplinart != Disziplinart.Schiessen && IsMassValue(value))
        {
            _versuche[index] = value;
        }

        RaiseValuesChanged();
    }

    /// <summary>
    /// Prüft, ob der Wert in einem Schuss-Versuch gültig ist
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static bool IsSchussValue(int value)
    {
        return value switch
        {
            0 or 2 or 5 or 10 => true,
            _ => false,
        };
    }

    /// <summary>
    /// Prüft, ob der Wert in einem Mass oder Komibinier-Versuch gültig ist
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static bool IsMassValue(int value)
    {
        return value switch
        {
            0 or 2 or 4 or 6 or 8 or 10 => true,
            _ => false,
        };
    }
    #endregion

    #endregion

    public override string ToString()
    {
        return $"Disziplin: {Disziplinart} | Summe: {Summe} | {Versuch1} # {Versuch2} # {Versuch3} # {Versuch4} # {Versuch5} # {Versuch6}";
    }
}
