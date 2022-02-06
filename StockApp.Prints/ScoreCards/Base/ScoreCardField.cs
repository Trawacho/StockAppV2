using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StockApp.Prints.ScoreCards.Base
{
    internal class ScoreCardField : Border
    {
        public FontFamily FntFamily { get; set; }

        public TextBlock Textblock { get; set; }

        public ScoreCardField(string Text, int RotateAngle = 0)
        {
            //FntFamily = new FontFamily("Bahnschrift");
            FntFamily = new FontFamily("Segoe UI");

            BorderBrush = Brushes.Black;
            BorderThickness = new Thickness(0.5);

            Textblock = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontFamily = FntFamily,
                FontSize = 10.0,
                FontStretch = FontStretches.UltraCondensed,
                TextAlignment = TextAlignment.Center,
                FontWeight = FontWeights.Normal,
                LayoutTransform = new RotateTransform(RotateAngle),
                Margin = new Thickness(0),
                Padding = new Thickness(0),
                Text = Text
            };

            Child = Textblock;
        }

        public ScoreCardField(Brush brush)
        {
            BorderBrush = Brushes.Black;
            BorderThickness = new Thickness(0.5);
            Margin = new Thickness(0);
            Background = brush;
        }
    }
}
