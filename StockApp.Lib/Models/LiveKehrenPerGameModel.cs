using StockApp.Core.Wettbewerb.Teambewerb;
using System.Linq;

namespace StockApp.Lib.Models;

public class LiveKehrenPerGameModel : KehrenBaseModel
{
    public LiveKehrenPerGameModel(IGame game) : base(game)
    {
    }

    public string TeamName1 => _game.IsTeamA_Starting ? _game.TeamA.TeamNameShort : _game.TeamA.TeamNameShort;

    public string TeamName2 => _game.IsTeamA_Starting ? _game.TeamB.TeamName : _game.TeamB.TeamNameShort;

    public override int StockPunkte1 => _game.Spielstand.Punkte_Live_TeamA;

    public override int StockPunkte2 => _game.Spielstand.Punkte_Live_TeamB;

    public override int Spielpunkte1 => _game.Spielstand.GetSpielPunkteTeamA(true);
    public override int Spielpunkte2 => _game.Spielstand.GetSpielPunkteTeamB(true);

    public int StockPunkteDifferenz => Spielpunkte1 - Spielpunkte2;

    public bool Team1Anspiel => _game.IsTeamA_Starting;

    public bool Is8TurnsGame => _game.Spielstand.Kehren_Live.Count() == 8;

    protected override int GetKehre(int kehrenNummer, bool team1)
    {
        return team1
            ? _game.Spielstand.Kehren_Live.FirstOrDefault(k => k.KehrenNummer == kehrenNummer)?.PunkteTeamA ?? 0
            : _game.Spielstand.Kehren_Live.FirstOrDefault(k => k.KehrenNummer == kehrenNummer)?.PunkteTeamB ?? 0;
    }
}
