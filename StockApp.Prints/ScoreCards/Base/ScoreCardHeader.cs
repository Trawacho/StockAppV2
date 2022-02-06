using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace StockApp.Prints.ScoreCards.Base
{
    internal class ScoreCardHeader : ScoreCardGrid
    {
        private readonly FontFamily _fnt = new("Consolas");

        private readonly string _startNummer;
        private readonly string _teamName;

        private ScoreCardHeader(int startNumber, string teamName, bool printTeamName, bool kehren8, bool forStockTV) : base(kehren8, forStockTV)
        {
            int columnSpan = kehren8 ? 8 : 7;

            _startNummer = startNumber.ToString();
            _teamName = teamName;

            Margin = new Thickness(0, 0, 0, 5);

            //StartNummer
            var textBlockStartnummer = new TextBlock()
            {
                Text = string.Concat("Nr. ", _startNummer),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                FontFamily = _fnt
            };
            SetColumnSpan(textBlockStartnummer, 3);
            SetColumn(textBlockStartnummer, 0);
            Children.Add(textBlockStartnummer);

            //Moarschaft
            var textBlockMoarschaft = new TextBlock()
            {
                Text = "Moarschaft:",
                FontWeight = FontWeights.Normal,
                FontSize = 12,
                FontFamily = _fnt,
                TextAlignment = TextAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            SetColumnSpan(textBlockMoarschaft, 3);
            SetColumn(textBlockMoarschaft, 3);
            Children.Add(textBlockMoarschaft);

            //TeamName oder nur Linie
            if (printTeamName)
            {
                var textBlockTeamName = new TextBlock()
                {
                    Text = _teamName,
                    FontWeight = FontWeights.Normal,
                    FontSize = 14,
                    FontFamily = _fnt,
                    TextAlignment = TextAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(5, 0, 0, 0)
                };
                SetColumnSpan(textBlockTeamName, columnSpan);
                SetColumn(textBlockTeamName, 6);
                Children.Add(textBlockTeamName);
            }
            else
            {
                var line = new Line()
                {
                    StrokeThickness = 1,
                    Stroke = Brushes.Black,
                    Stretch = Stretch.Fill,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    X1 = 0,
                    Y1 = 0,
                    X2 = 5,
                    Y2 = 0
                };
                SetColumnSpan(line, columnSpan);
                SetColumn(line, 6);
                Children.Add(line);
            }

            //Gegener
            var textBlockGegner = new TextBlock()
            {
                Text = "Gegner",
                FontWeight = FontWeights.Normal,
                FontSize = 12,
                FontFamily = _fnt,
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            SetColumnSpan(textBlockGegner, columnSpan);
            SetColumn(textBlockGegner, 14);
            Children.Add(textBlockGegner);
        }

        internal ScoreCardHeader(int startNumber, string teamName, bool printTeamName, bool kehren8, int numberOfRound, bool forStockTV) : this(startNumber, teamName, printTeamName, kehren8, forStockTV)
        {
            if (numberOfRound > 0)
            {
                var textBlockRound = new TextBlock()
                {
                    Text = string.Concat("Runde: ", numberOfRound),
                    FontWeight = FontWeights.Bold,
                    FontSize = 12,
                    FontFamily = _fnt,
                    TextAlignment = TextAlignment.Right
                };
                SetColumnSpan(textBlockRound, 2);
                SetColumn(textBlockRound, this.ColumnDefinitions.Count - 2);
                Children.Add(textBlockRound);
            }
        }
    }
}
