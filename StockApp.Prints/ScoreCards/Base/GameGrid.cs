using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Prints.Converters;
using System.Linq;
using System.Windows.Media;

namespace StockApp.Prints.ScoreCards.Base
{
    internal class GameGrid : ScoreCardGrid
    {
        /// <summary>
        /// 6,7 oder 8 Kehren mit optionalem Feld für die StockTV-Farbe oder Name für Gegener
        /// </summary>
        /// <param name="game"></param>
        /// <param name="team"></param>
        /// <param name="is8turnsGame"></param>
        /// <param name="forStockTV"></param>
        public GameGrid(IGame game, ITeam team, bool is8turnsGame, bool forStockTV, bool opponentOnScoreCards) : base(is8turnsGame, forStockTV, opponentOnScoreCards)
        {

            int colCounter = this.ColumnDefinitions.Count;
            string NumberOfGame = game.GameNumberOverAll.ToString();
            string NumberOfArea = game.CourtNumber.ToString();
            string Opponent = team.StartNumber == game.TeamA.StartNumber ? game.TeamB.StartNumber.ToString() : game.TeamA.StartNumber.ToString();
            string StartOfGame = game.GetStartingTeam().StartNumber.ToString();
            string ColorName = team.SpieleAufStartSeite().Contains(game.GameNumberOverAll) ? "GRÜN" : "ROT";

            RowDefinitions.Add(new System.Windows.Controls.RowDefinition()
            {
                Height = new System.Windows.GridLength(PixelConverter.CmToPx(0.6))
            });

            if (game.IsPauseGame())
            {
                //In Spalte 12,14 oder 15 die Spielnummer eintragen
                var b1 = new ScoreCardField(NumberOfGame, 0);
                if (opponentOnScoreCards)
                {
                    SetColumn(b1, 12);
                }
                else if (is8turnsGame)
                {
                    SetColumn(b1, 15);
                }
                else
                {
                    SetColumn(b1, 14);
                }
                Children.Add(b1);

                //von Spalte 0 bis 2 "aussetzen" eintragen
                var b2 = new ScoreCardField("aussetzen", 0);
                SetColumn(b2, 0);
                SetColumnSpan(b2, 3);
                Children.Add(b2);

                for (int i = 3; i < colCounter; i++) //In die Spalten einen grauen block eintragen
                {
                    //Die Spalte Trennstrich und Spalte Spielnummer freilassen
                    if (opponentOnScoreCards)
                    {
                        if (i == 11 | i == 12) continue;
                    }
                    else if (is8turnsGame)
                    {
                        if (i == 14 || i == 15) continue;
                    }
                    else
                    {
                        if (i == 13 || i == 14) continue;
                    }
                    var b = new ScoreCardField(Brushes.LightGray);
                    SetColumn(b, i);
                    Children.Add(b);
                }
            }
            else
            {
                for (int i = 0; i < colCounter; i++)
                {
                    //leerer Spalt übersrpingen
                    if (i == (opponentOnScoreCards ? 11 : is8turnsGame ? 14 : 13))
                        i++;

                    string text = i switch
                    {
                        0 => NumberOfArea,
                        1 => Opponent,
                        2 => StartOfGame,
                        3 => !opponentOnScoreCards && forStockTV ? ColorName : string.Empty,
                        12 => opponentOnScoreCards ? NumberOfGame : string.Empty,
                        14 => !opponentOnScoreCards && !is8turnsGame ? NumberOfGame : string.Empty,
                        15 => !opponentOnScoreCards && is8turnsGame ? NumberOfGame : string.Empty,
                        21 => opponentOnScoreCards ? (game.TeamA.StartNumber == team.StartNumber) ? game.TeamB.TeamNameShort : game.TeamA.TeamNameShort : string.Empty,
                        _ => string.Empty,
                    };
                    var b = new ScoreCardField(text, 0);

                    if (i == 21 && opponentOnScoreCards)
                    {
                        b.Textblock.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        b.Textblock.Padding = new System.Windows.Thickness(3, 0, 0, 0);
                    }

                    SetColumn(b, i);
                    Children.Add(b);
                }
            }
        }
    }
}
