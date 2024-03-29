﻿using StockApp.Comm.Broadcasting;
using StockApp.Comm.NetMqStockTV;
using StockApp.Core.Factories;

namespace StockApp.Core.Wettbewerb.Teambewerb;

public interface IContainerTeamBewerbe : IBewerb
{
    /// <summary>
    /// list of all TeamBewerbe, at least one TeamBewerb has to exists
    /// </summary>
    IOrderedEnumerable<ITeamBewerb> TeamBewerbe { get; }

    /// <summary>
    /// the current TeamBewerb
    /// </summary>
    ITeamBewerb CurrentTeamBewerb { get; }

    /// <summary>
    /// Add a new TeamBewerb to <see cref="TeamBewerbe"/>
    /// </summary>
    TeamBewerb AddNew();

    void Remove(ITeamBewerb teamBewerb);
    void Remove(int teamBewerbId);

    /// <summary>
    /// Reset and Remove all TeamBewerbe and set one new TeamBewerb/>
    /// </summary>
    void RemoveAll();

    void SetCurrentTeamBewerb(ITeamBewerb teamBewerb);

    /// <summary>
    /// occours after a TeamBewerb was added or removed
    /// </summary>
    public event EventHandler TeamBewerbeChanged;

    /// <summary>
    /// Occours when the <see cref="CurrentTeamBewerb"/> changes
    /// </summary>
    public event EventHandler CurrentTeamBewerbChanged;

    /// <summary>
    /// Occours when from <see cref="CurrentTeamBewerb"/> the Event <see cref="ITeamBewerb.TeamsChanged"/> was raised
    /// </summary>
    public event EventHandler CurrentTeambewerb_TeamsChanged;

    /// <summary>
    /// Occours when from <see cref="CurrentTeamBewerb"/> the Event <see cref="ITeamBewerb.GamesChanged"/> was raised
    /// </summary>
    public event EventHandler CurrentTeambewerb_GamesChanged;

    public IEnumerable<IGameplan> Gameplans { get; }
}

public class ContainerTeamBewerbe : IContainerTeamBewerbe
{
    private ContainerTeamBewerbe()
    {
        _teamBewerbe = new List<ITeamBewerb>() { TeamBewerb.Create(1) };
        SetCurrentTeamBewerb(_teamBewerbe.First());
        Gameplans = GamePlanFactory.LoadAllGameplans();
    }

    public static IContainerTeamBewerbe Create() => new ContainerTeamBewerbe();

    private readonly List<ITeamBewerb> _teamBewerbe;
    private ITeamBewerb _currentTeamBewerb;

    public event EventHandler CurrentTeamBewerbChanged;
    public event EventHandler CurrentTeambewerb_TeamsChanged;
    public event EventHandler CurrentTeambewerb_GamesChanged;
    public event EventHandler TeamBewerbeChanged;

    protected void RaiseCurrentTeamBewerbChanged() => CurrentTeamBewerbChanged?.Invoke(this, EventArgs.Empty);
    protected void RaiseCurrentTeam_TeamsChanged() => CurrentTeambewerb_TeamsChanged?.Invoke(this, EventArgs.Empty);
    protected void RaiseCurrentTeam_GamesChanged() => CurrentTeambewerb_GamesChanged?.Invoke(this, EventArgs.Empty);
    protected void RaiseTeamBewerbeChanged() => TeamBewerbeChanged?.Invoke(this, EventArgs.Empty);


    public IOrderedEnumerable<ITeamBewerb> TeamBewerbe => _teamBewerbe.OrderBy(b => b.ID);
    public ITeamBewerb CurrentTeamBewerb => _currentTeamBewerb;

    public IEnumerable<IGameplan> Gameplans { get; private set; }

    public void SetCurrentTeamBewerb(ITeamBewerb teamBewerb)
    {
        if (_currentTeamBewerb != null)
        {
            _currentTeamBewerb.GamesChanged -= (s, e) => RaiseCurrentTeam_GamesChanged();
            _currentTeamBewerb.TeamsChanged -= (s, e) => RaiseCurrentTeam_TeamsChanged();
        }

        if (teamBewerb != null)
        {
            _currentTeamBewerb = teamBewerb;
            _currentTeamBewerb.GamesChanged += (s, e) => RaiseCurrentTeam_GamesChanged();
            _currentTeamBewerb.TeamsChanged += (s, e) => RaiseCurrentTeam_TeamsChanged();
        }

        RaiseCurrentTeamBewerbChanged();
    }


    public TeamBewerb AddNew()
    {
        if (TeamBewerbe.Count() == 1 && _teamBewerbe.First().SpielGruppe == 0)
            TeamBewerbe.First().SpielGruppe = 1;

        var teamBewerb = _teamBewerbe.Any()
            ? TeamBewerb.Create(_teamBewerbe.OrderBy(t => t.ID).Last().ID + 1)
            : TeamBewerb.Create(1);

        teamBewerb.SpielGruppe = GetNextFreeSpielgruppe();

        _teamBewerbe.Add(teamBewerb);
        RaiseTeamBewerbeChanged();
        return teamBewerb;
    }

    public void Remove(ITeamBewerb teamBewerb)
    {
        Remove(teamBewerb.ID);
    }

    public void Remove(int teamBewerbId)
    {
        _teamBewerbe.RemoveAll(t => t.ID == teamBewerbId);
        RaiseTeamBewerbeChanged();
    }

    public void RemoveAll()
    {
        Reset();

        SetCurrentTeamBewerb(null);

        _teamBewerbe.Clear();

        SetCurrentTeamBewerb(AddNew());

        RaiseTeamBewerbeChanged();
    }

    public void SetBroadcastData(IBroadCastTelegram telegram)
    {
        foreach (var bewerb in TeamBewerbe.Where(t => t.SpielGruppe == telegram.SpielGruppe))
        {
            bewerb.SetBroadcastData(telegram);
        }
    }

    public void SetStockTVResult(IStockTVResult tVResult)
    {
        foreach (var bewerb in TeamBewerbe.Where(t => t.SpielGruppe == tVResult.TVSettings.Spielgruppe))
        {
            bewerb.SetStockTVResult(tVResult);
        }
    }

    public void Reset()
    {
        foreach (var teamBewerb in TeamBewerbe)
        {
            teamBewerb.Reset();
        }
    }

    private int GetNextFreeSpielgruppe()
    {
        for (int i = 1; i < 10; i++)
        {
            if (!TeamBewerbe.Any(t => t.SpielGruppe == i))
                return i;
        }
        return 0;
    }
}

