using StockApp.Core.Wettbewerb;
using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Core.Wettbewerb.Zielbewerb;
using System.ComponentModel;

namespace StockApp.Core.Turnier;

public interface ITurnier
{
    IOrgaDaten OrgaDaten { get; set; }
    IBewerb Wettbewerb { get; }
    IContainerTeamBewerbe ContainerTeamBewerbe { get; }

    void SetBewerb(Wettbewerbsart team);

    void Reset();

    event CancelEventHandler WettbewerbChanging;
    event Action WettbewerbChanged;
}


public class Turnier : ITurnier
{
    public event Action WettbewerbChanged;
    public event CancelEventHandler WettbewerbChanging;

    /// <summary>
    /// true, if the event should be cancled, otherwise false
    /// </summary>
    /// <returns></returns>
    protected virtual bool RaisWettbewerbChanging()
    {
        var cancel = new CancelEventArgs();
        var handler = WettbewerbChanging;
        handler?.Invoke(this, cancel);
        return cancel.Cancel;
    }

    protected virtual void RaiseWettbewerbChanged()
    {
        var handler = WettbewerbChanged;
        handler?.Invoke();
    }

    #region Fields

    private readonly IContainerTeamBewerbe _teamBewerbContainer;
    private readonly IZielBewerb _zielbewerb;
    private IBewerb _wettbewerb;

    #endregion

    #region Properties

    /// <summary>
    /// Organisatorische Daten jedes Turniers
    /// </summary>
    public IOrgaDaten OrgaDaten { get; set; }

    /// <summary>
    /// Kann ein <see cref="TeamBewerb"/> oder <see cref="Zielbewerb.ZielBewerb"/> sein
    /// </summary>
    public IBewerb Wettbewerb
    {
        get => _wettbewerb;
        private set
        {
            if (_wettbewerb == value) return;
            if (RaisWettbewerbChanging()) return;

            _wettbewerb = value;

            RaiseWettbewerbChanged();
        }
    }

    public IContainerTeamBewerbe ContainerTeamBewerbe => _teamBewerbContainer;

    #endregion

    #region Contructor

    /// <summary>
    /// Default Konstruktor
    /// </summary>
    private Turnier()
    {
        OrgaDaten = new OrgaDaten();

        _teamBewerbContainer = Core.Wettbewerb.Teambewerb.ContainerTeamBewerbe.Create();
        _zielbewerb = ZielBewerb.Create();
    }

    public static ITurnier Create() => new Turnier();

    #endregion

    #region Public Functions

    public void SetBewerb(Wettbewerbsart art)
    {
        Wettbewerb = art == Wettbewerbsart.Team
                            ? _teamBewerbContainer
                            : _zielbewerb;
    }

    public void Reset()
    {
        _zielbewerb.Reset();
        _teamBewerbContainer.Reset();
        Wettbewerb = null;
        OrgaDaten = new OrgaDaten();
    }



    #endregion
}
