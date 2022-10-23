using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StockApp.Prints.Spielplan
{
    internal class SpielPlanGrid : Grid
    {
        private readonly IEnumerable<SpielplanGame> _games;

        public SpielPlanGrid(IEnumerable<SpielplanGame> games)
        {
            _games = games.OrderBy(g => g.SpielNummer).ThenBy(g => g.Bahn);

            for (int i = 0; i <= _games.Max(g => g.Bahn); i++)
                ColumnDefinitions.Add(new ColumnDefinition());

            for (int i = 0; i <= _games.Max(g => g.SpielNummer); i++)
                RowDefinitions.Add(new RowDefinition());

            for (int i = 1; i <= _games.Max(g => g.Bahn); i++)
            {
                var t = SpielplanUIElements.GetTextblock($"Bahn {i}", FontWeights.Bold);
                SetColumn(t, i);
                SetRow(t, 0);
                Children.Add(t);
            }

            for (int i = 1; i <= _games.Max(g => g.SpielNummer); i++)
            {
                var t = SpielplanUIElements.GetTextblock($"Spiel {i}", FontWeights.Bold);
                SetColumn(t, 0);
                SetRow(t, i);
                Children.Add(t);
            }


            for (int s = 1; s <= _games.Max(g => g.SpielNummer); s++)
            {
                var spieleVonS = _games.Where(v => v.SpielNummer == s);

                for (int b = 1; b <= spieleVonS.Max(h => h.Bahn); b++)
                {
                    var game = spieleVonS.FirstOrDefault(x => x.Bahn == b);
                    if (game == null) continue;

                    var t = SpielplanUIElements.GetSpielplanSpielText(game);
                    SetRow(t, s);
                    SetColumn(t, b);
                    Children.Add(t);
                }
            }
        }
    }


}

