using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StockApp.Prints.Spielplan
{
    internal static class SpielplanUIElements
    {
        internal static UIElement GetTextblock(string text, FontWeight fontWeight)
        {
            var textblock = new TextBlock()
            {
                Text = text,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontFamily = new FontFamily("Bahnschrift"),
                FontSize = 12.0,
                FontStretch = FontStretches.Normal,
                TextAlignment = TextAlignment.Center,
                FontWeight = fontWeight,
                Margin = new Thickness(10),
            };
            return textblock;
        }


        internal static UIElement GetSpielplanSpielText(SpielplanGame game)
        {
            var border = new Border() { BorderThickness = new Thickness(0.5), BorderBrush = Brushes.Black };
            var stackPnl = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
            };
            var teamA = GetTextblock(game.TeamA.ToString(),
                                             game.AnspielTeamA ? FontWeights.Bold : FontWeights.Regular);
            if (game.AnspielTeamA)
            {
                (teamA as TextBlock).FontSize++;
                (teamA as TextBlock).FontStyle = FontStyles.Italic;
            }
            else
            {
                (teamA as TextBlock).FontSize--;
            }
            var dp = GetTextblock(":", FontWeights.Regular);
            (dp as TextBlock).Margin = new Thickness(0);

            var teamB = GetTextblock(game.TeamB.ToString(),
                                            !game.AnspielTeamA ? FontWeights.Bold : FontWeights.Regular);
            if (!game.AnspielTeamA)
            {
                (teamB as TextBlock).FontSize++;
                (teamB as TextBlock).FontStyle = FontStyles.Italic;
            }
            else
            {
                (teamB as TextBlock).FontSize--;
            }


            stackPnl.Children.Add(teamA);
            stackPnl.Children.Add(dp);
            stackPnl.Children.Add(teamB);
            border.Child = stackPnl;
            return border;
        }
    }


}

