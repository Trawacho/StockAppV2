using StockApp.Prints.Converters;
using System.Windows;
using System.Windows.Controls;

namespace StockApp.Prints.ScoreCards.Base
{
    internal class ScoreCardHeaderGrid : ScoreCardGrid
    {
        public ScoreCardHeaderGrid(bool is8TurnsGame, bool forStockTV) : base(is8TurnsGame, forStockTV)
        {
            //Two Rows
            RowDefinitions.Add(new RowDefinition() { Height = new GridLength(PixelConverter.CmToPx(0.60)) });
            RowDefinitions.Add(new RowDefinition() { Height = new GridLength(PixelConverter.CmToPx(0.60)) });

            int colCounter = 0;
            int kehrenColSpan = is8TurnsGame ? 8 : 7;

            #region Texte  Moarschaft

            //Borders
            var BorderSpiel = new ScoreCardField("Bahn", 270);
            SetRowSpan(BorderSpiel, 2);
            SetColumn(BorderSpiel, colCounter);
            SetRow(BorderSpiel, 0);
            Children.Add(BorderSpiel);
            colCounter++;

            var BorderBahn = new ScoreCardField("Gegner", 270);
            SetRowSpan(BorderBahn, 2);
            SetColumn(BorderBahn, colCounter);
            SetRow(BorderSpiel, 0);
            Children.Add(BorderBahn);
            colCounter++;

            var BorderAnspiel = new ScoreCardField("Anspiel", 270);
            SetRowSpan(BorderAnspiel, 2);
            SetColumn(BorderAnspiel, colCounter);
            SetRow(BorderSpiel, 0);
            Children.Add(BorderAnspiel);
            colCounter++;

            if (forStockTV)
            {
                var BorderEingabe = new ScoreCardField("Eingabe", 0);
                SetColumn(BorderEingabe, 4);
                SetColumn(BorderEingabe, colCounter);
                SetRowSpan(BorderEingabe, 4);
                Children.Add(BorderEingabe);
                colCounter++;
            }


            var BorderKehre = new ScoreCardField("K e h r e n");
            SetColumnSpan(BorderKehre, kehrenColSpan);
            SetColumn(BorderKehre, colCounter);
            SetRow(BorderKehre, 0);
            Children.Add(BorderKehre);



            var BorderKehre1 = new ScoreCardField("1");
            SetColumn(BorderKehre1, colCounter);
            SetRow(BorderKehre1, 1);
            Children.Add(BorderKehre1);
            colCounter++;

            var BorderKehre2 = new ScoreCardField("2");
            SetColumn(BorderKehre2, colCounter);
            SetRow(BorderKehre2, 1);
            Children.Add(BorderKehre2);
            colCounter++;

            var BorderKehre3 = new ScoreCardField("3");
            SetColumn(BorderKehre3, colCounter);
            SetRow(BorderKehre3, 1);
            Children.Add(BorderKehre3);
            colCounter++;

            var BorderKehre4 = new ScoreCardField("4");
            SetColumn(BorderKehre4, colCounter);
            SetRow(BorderKehre4, 1);
            Children.Add(BorderKehre4);
            colCounter++;

            var BorderKehre5 = new ScoreCardField("5");
            SetColumn(BorderKehre5, colCounter);
            SetRow(BorderKehre5, 1);
            Children.Add(BorderKehre5);
            colCounter++;

            var BorderKehre6 = new ScoreCardField("6");
            SetColumn(BorderKehre6, colCounter);
            SetRow(BorderKehre6, 1);
            Children.Add(BorderKehre6);
            colCounter++;

            var BorderKehre7 = new ScoreCardField("7");
            SetColumn(BorderKehre7, colCounter);
            SetRow(BorderKehre7, 1);
            Children.Add(BorderKehre7);
            colCounter++;
            if (is8TurnsGame)
            {
                var BorderKehre8 = new ScoreCardField("8");
                SetColumn(BorderKehre8, colCounter);
                SetRow(BorderKehre8, 1);
                Children.Add(BorderKehre8);
                colCounter++;
            }

            var BorderSumme = new ScoreCardField("Summe", 0);
            BorderSumme.Textblock.FontWeight = FontWeights.Bold;
            SetColumn(BorderSumme, colCounter);
            SetRowSpan(BorderSumme, 2);
            Children.Add(BorderSumme);
            colCounter++;

            if (!forStockTV)
            {
                var BorderStrafSumme = new ScoreCardField("Straf-\r\npunkte", 0);
                SetColumn(BorderStrafSumme, colCounter);
                SetRowSpan(BorderStrafSumme, 2);
                Children.Add(BorderStrafSumme);
                colCounter++;
            }


            var BorderPunkte = new ScoreCardField("Gewinn-\r\npunkte", 0);
            BorderPunkte.Textblock.FontWeight = FontWeights.Bold;
            SetColumn(BorderPunkte, colCounter);
            SetRowSpan(BorderPunkte, 2);
            Children.Add(BorderPunkte);
            colCounter++;

            #endregion

            colCounter++;


            #region Texte Gegner

            var BorderGegner = new ScoreCardField("Spiel", 270);
            SetRowSpan(BorderGegner, 2);
            SetColumn(BorderGegner, colCounter);
            Children.Add(BorderGegner);
            colCounter++;

            var BorderKehreG = new ScoreCardField("K e h r e n");
            SetColumnSpan(BorderKehreG, kehrenColSpan);
            SetColumn(BorderKehreG, colCounter);
            SetRow(BorderKehreG, 0);
            Children.Add(BorderKehreG);

            var BorderKehre1G = new ScoreCardField("1");
            SetColumn(BorderKehre1G, colCounter);
            SetRow(BorderKehre1G, 1);
            Children.Add(BorderKehre1G);
            colCounter++;

            var BorderKehre2G = new ScoreCardField("2");
            SetColumn(BorderKehre2G, colCounter);
            SetRow(BorderKehre2G, 1);
            Children.Add(BorderKehre2G);
            colCounter++;

            var BorderKehre3G = new ScoreCardField("3");
            SetColumn(BorderKehre3G, colCounter);
            SetRow(BorderKehre3G, 1);
            Children.Add(BorderKehre3G);
            colCounter++;

            var BorderKehre4G = new ScoreCardField("4");
            SetColumn(BorderKehre4G, colCounter);
            SetRow(BorderKehre4G, 1);
            Children.Add(BorderKehre4G);
            colCounter++;

            var BorderKehre5G = new ScoreCardField("5");
            SetColumn(BorderKehre5G, colCounter);
            SetRow(BorderKehre5G, 1);
            Children.Add(BorderKehre5G);
            colCounter++;

            var BorderKehre6G = new ScoreCardField("6");
            SetColumn(BorderKehre6G, colCounter);
            SetRow(BorderKehre6G, 1);
            Children.Add(BorderKehre6G);
            colCounter++;

            var BorderKehre7G = new ScoreCardField("7");
            SetColumn(BorderKehre7G, colCounter);
            SetRow(BorderKehre7G, 1);
            Children.Add(BorderKehre7G);
            colCounter++;

            if (is8TurnsGame)
            {
                var BorderKehre8G = new ScoreCardField("8");
                SetColumn(BorderKehre8G, colCounter);
                SetRow(BorderKehre8G, 1);
                Children.Add(BorderKehre8G);
                colCounter++;
            }

            var BorderSummeG = new ScoreCardField("Summe", 0);
            BorderSummeG.Textblock.FontWeight = FontWeights.Bold;
            SetColumn(BorderSummeG, colCounter);
            SetRowSpan(BorderSummeG, 2);
            Children.Add(BorderSummeG);
            colCounter++;


            if (!forStockTV)
            {
                var BorderStrafSummeG = new ScoreCardField("Straf-\r\npunkte", 0);
                SetColumn(BorderStrafSummeG, colCounter);
                SetRowSpan(BorderStrafSummeG, 2);
                Children.Add(BorderStrafSummeG);
                colCounter++;
            }

            var BorderPunkteG = new ScoreCardField("Gewinn-\r\npunkte", 0);
            BorderPunkteG.Textblock.FontWeight = FontWeights.Bold;
            SetColumn(BorderPunkteG, colCounter);
            SetRowSpan(BorderPunkteG, 2);
            Children.Add(BorderPunkteG);

            #endregion

        }
    }
}
