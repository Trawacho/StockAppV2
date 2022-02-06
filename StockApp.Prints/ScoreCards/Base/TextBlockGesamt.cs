using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StockApp.Prints.ScoreCards.Base
{
    internal class TextBlockGesamt : TextBlock
    {
        internal TextBlockGesamt()
        {
            Text = "Gesamt";
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;
            FontFamily = new FontFamily("Bahnschrift");
            FontSize = 12.0;
            FontStretch = FontStretches.Normal;
            TextAlignment = TextAlignment.Center;
            FontWeight = FontWeights.Bold;
        }
    }
}
