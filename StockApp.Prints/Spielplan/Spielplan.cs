using StockApp.Core.Wettbewerb.Teambewerb;
using System.Collections.Generic;

namespace StockApp.Prints.Spielplan
{
    internal class Spielplan
    {
        readonly List<SpielplanGame> _planSpiele;
        public Spielplan(IEnumerable<IGame> games)
        {
            _planSpiele = new List<SpielplanGame>();
            foreach (var game in games)
            {
                _planSpiele.Add(new SpielplanGame(game));
            }
        }
        public IEnumerable<SpielplanGame> Spiele => _planSpiele;
    }


    internal class SpielplanGame
    {
        public int Bahn { get; }
        public int SpielNummer { get; }
        public int TeamA { get; }
        public int TeamB { get; }
        public bool AnspielTeamA { get; }

        public SpielplanGame(IGame game)
        {
            Bahn = game.CourtNumber;
            SpielNummer = game.GameNumberOverAll;
            TeamA = game.TeamA.StartNumber;
            TeamB = game.TeamB.StartNumber;
            AnspielTeamA = game.IsTeamA_Starting;
        }
    }
}
