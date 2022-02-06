using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Prints.Converters;
using System.Linq;
using System.Windows.Media;

namespace StockApp.Prints.ScoreCards.Base
{
    internal class GameGrid : ScoreCardGrid
    {
        public GameGrid(IGame game, ITeam team, bool is8turnsGame, bool forStockTV) : base(is8turnsGame, forStockTV)
        {

            int colCounter = is8turnsGame ? 27 : 25;
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
                //In Spalte 14(15) die Spielnummer eintragen
                var b1 = new ScoreCardField(NumberOfGame, 0);
                if (is8turnsGame)
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

                for (int i = 3; i < colCounter; i++) //In die Spalten 3 bis 24 einen grauen block eintragen
                {
                    if (is8turnsGame)
                    {
                        if (i == 14 || i == 15) continue; //Die Spalte 13 (Trennstrich) und Spalte 14 (Spielnummer) freilassen

                    }
                    else
                    {
                        if (i == 13 || i == 14) continue; //Die Spalte 13 (Trennstrich) und Spalte 14 (Spielnummer) freilassen
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
                    if (i == (is8turnsGame ? 14 : 13))
                        i++;

                    string t = i switch
                    {
                        0 => NumberOfArea,
                        1 => Opponent,
                        2 => StartOfGame,
                        3 => forStockTV ? ColorName : string.Empty,
                        14 => !is8turnsGame ? NumberOfGame : string.Empty,
                        15 => is8turnsGame ? NumberOfGame : string.Empty,
                        _ => string.Empty,
                    };
                    var b = new ScoreCardField(t, 0);

                    SetColumn(b, i);
                    Children.Add(b);
                }
            }
        }
    }
}
