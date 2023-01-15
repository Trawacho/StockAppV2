using StockApp.Core.Wettbewerb.Teambewerb;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace StockApp.Lib.Models;

public class KehrenBaseModel : IDisposable
{
    protected readonly IGame _game;

    public KehrenBaseModel(IGame game)
    {
        _game = game;
    }
    public bool _disposed;
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
#if DEBUG
        GC.SuppressFinalize(this);
#endif
    }

    public virtual int StockPunkte1 { get => throw new NotSupportedException("must be overriden in subclass"); }
    public virtual int StockPunkte2 { get => throw new NotSupportedException("must be overriden in subclass"); }

    public virtual int Spielpunkte1 { get => throw new NotSupportedException("must be overriden in subclass"); }
    public virtual int Spielpunkte2 { get => throw new NotSupportedException("must be overriden in subclass"); }

    #region Kehren von Team1
    /// <summary>
    /// Kehre 1 von Team1
    /// </summary>
    public int Kehre1vonTeam1
    {
        get => GetKehre(1, true);
        set => SetKehre(1, value, team1: true, nameof(Kehre1vonTeam2));
    }

    /// <summary>
    /// Kehre 2 von Team1
    /// </summary>
    public int Kehre2vonTeam1
    {
        get => GetKehre(2, true);
        set => SetKehre(2, value, team1: true, nameof(Kehre2vonTeam2));
    }

    public int Kehre3vonTeam1
    {
        get => GetKehre(3, true);
        set => SetKehre(3, value, team1: true, nameof(Kehre3vonTeam2));
    }

    public int Kehre4vonTeam1
    {
        get => GetKehre(4, true);
        set => SetKehre(4, value, team1: true, nameof(Kehre4vonTeam2));
    }

    public int Kehre5vonTeam1
    {
        get => GetKehre(5, true);
        set => SetKehre(5, value, team1: true, nameof(Kehre5vonTeam2));
    }

    public int Kehre6vonTeam1
    {
        get => GetKehre(6, true);
        set => SetKehre(6, value, team1: true, nameof(Kehre6vonTeam2));
    }

    public int Kehre7vonTeam1
    {
        get => GetKehre(7, true);
        set => SetKehre(7, value, team1: true, nameof(Kehre7vonTeam2));
    }

    public int Kehre8vonTeam1
    {
        get => GetKehre(8, true);
        set => SetKehre(8, value, team1: true, nameof(Kehre8vonTeam2));
    }

    #endregion

    #region Kehren von Team2
    public int Kehre1vonTeam2
    {
        get => GetKehre(1, false);
        set => SetKehre(1, value, team1: false, nameof(Kehre1vonTeam1));
    }
    public int Kehre2vonTeam2
    {
        get => GetKehre(2, false);
        set => SetKehre(2, value, team1: false, nameof(Kehre2vonTeam1));
    }
    public int Kehre3vonTeam2
    {
        get => GetKehre(3, false);
        set => SetKehre(3, value, team1: false, nameof(Kehre3vonTeam1));
    }
    public int Kehre4vonTeam2
    {
        get => GetKehre(4, false);
        set => SetKehre(4, value, team1: false, nameof(Kehre4vonTeam1));
    }
    public int Kehre5vonTeam2
    {
        get => GetKehre(5, false);
        set => SetKehre(5, value, team1: false, nameof(Kehre5vonTeam1));
    }
    public int Kehre6vonTeam2
    {
        get => GetKehre(6, false);
        set => SetKehre(6, value, team1: false, nameof(Kehre6vonTeam1));
    }

    public int Kehre7vonTeam2
    {
        get => GetKehre(7, false);
        set => SetKehre(7, value, team1: false, nameof(Kehre7vonTeam1));
    }
    public int Kehre8vonTeam2
    {
        get => GetKehre(8, false);
        set => SetKehre(8, value, team1: false, nameof(Kehre8vonTeam1));
    }

    #endregion

    protected virtual void SetKehre(int kehrenNummer, int value, bool team1, string propName1, [CallerMemberName] string propName2 = default)
        => throw new NotSupportedException("must be overriden in subclass");

    protected virtual int GetKehre(int kehrenNummer, bool team1)
        => throw new NotSupportedException("must be overriden in subclass");

    public IKehre GetMasterKehre(int kehrenNummer) => _game.Spielstand.Kehren_Master.FirstOrDefault(k => k.KehrenNummer == kehrenNummer);
    public IKehre GetLiveKehre(int kehrenNummer) => _game.Spielstand.Kehren_Live.FirstOrDefault(k => k.KehrenNummer == kehrenNummer);

}