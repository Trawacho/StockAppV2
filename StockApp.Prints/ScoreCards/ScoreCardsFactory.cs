using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Prints.BaseClasses;
using StockApp.Prints.ScoreCards.Base;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace StockApp.Prints.ScoreCards
{
    public static class ScoreCardsFactory
    {

        public static FixedDocument CreateScoreCards(Size pageSize, ITeamBewerb bewerb, bool summarizedScoreCards, bool namesOnScoreCard, bool stockTvOptimized)
        {
            return new ScoreCardHelper(pageSize, bewerb, summarizedScoreCards, namesOnScoreCard, stockTvOptimized).CreateScoreCards();
        }
    }

    class ScoreCardHelper : PrintsBaseClass
    {
        private readonly ITeamBewerb _teamBewerb;
        private readonly bool _summarizedScoreCards;
        private readonly bool _namesOnScoreCard;
        private readonly bool _stockTvOptimized;

        internal ScoreCardHelper(Size pageSize, ITeamBewerb bewerb, bool summarizedScoreCards, bool namesOnScoreCard, bool stockTvOptimized) : base(pageSize)
        {
            _teamBewerb = bewerb;
            _summarizedScoreCards = summarizedScoreCards;
            _namesOnScoreCard = namesOnScoreCard;
            _stockTvOptimized = stockTvOptimized;
        }

        internal FixedDocument CreateScoreCards()
        {
            var teamPanels = new List<StackPanel>();

            foreach (var team in _teamBewerb.Teams.Where(v => !v.IsVirtual)
                                                 .OrderBy(t => t.StartNumber)) //Für jedes Team eine Wertungs-Karte
            {
                if (_summarizedScoreCards)
                {
                    teamPanels.Add(
                        GetTeamPanel(
                            team,
                            team.Games
                                .OrderBy(g => g.GameNumberOverAll),
                            _teamBewerb.Is8TurnsGame,
                            0
                            ));
                }
                else
                {
                    int maxRounds = team.Games.Max(r => r.RoundOfGame);
                    for (int gameRound = 1; gameRound <= maxRounds; gameRound++)
                    {
                        teamPanels.Add(
                            GetTeamPanel(
                                team,
                                team.Games
                                    .Where(g => g.RoundOfGame == gameRound)
                                    .OrderBy(r => r.GameNumber),
                                _teamBewerb.Is8TurnsGame,
                                maxRounds == 1 ? 0 : gameRound
                                ));
                    }


                }
            }

            return base.CreateFixedDocument(teamPanels, false);
        }



        /// <summary>
        /// Eine Wertungskarte
        /// </summary>
        /// <param name="team">Wertungskarte für dieses Team</param>
        /// <param name="games">Diese Spiele in der Wertungskarte anzeigen</param>
        /// <param name="is8TurnGame">Wenn TRUE, dann werden 8 anstatt 7 Kehren gedruckt</param>
        /// <param name="numberOfGameRound">optional, wenn >0, dann wird eine Information der Runde angedruckt</param>
        /// <returns></returns>
        private StackPanel GetTeamPanel(ITeam team, IEnumerable<IGame> games, bool is8TurnsGame, int numberOfGameRound)
        {
            // alles was eine Wertungskarte braucht, kommt in ein Stackpanel
            var panel = new StackPanel();

            // Schneide Linie oben
            panel.Children.Add(Components.CutterLineTop());

            // Kopfzeile mit Mannschaftsnamen und weitere Infos
            panel.Children.Add(new ScoreCardHeader(team.StartNumber, team.TeamName, _namesOnScoreCard, is8TurnsGame, numberOfGameRound, _stockTvOptimized));

            // Spaltenüberschriften
            panel.Children.Add(new ScoreCardHeaderGrid(is8TurnsGame, _stockTvOptimized));


            // pro Spiel eine weitere Zeile
            foreach (var game in games)
            {
                panel.Children.Add(new GameGrid(game, team, is8TurnsGame, _stockTvOptimized));
            }

            // Summenzeile am Ende
            panel.Children.Add(new GameSummaryGrid(is8TurnsGame, _stockTvOptimized));

            // Schneidelinie unten
            panel.Children.Add(Components.CutterLine());

            return panel;
        }
    }
}
