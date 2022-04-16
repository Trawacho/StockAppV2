using StockApp.Prints.Converters;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace StockApp.Prints.ScoreCards.Base
{
    internal class ScoreCardGrid : Grid
    {

        double _fixedValuesWidth, _turnValuesWidth, _sumValueWidth, _pointsValueWidth, _spaceValueWidth, _opponentValueWidth, _penaltyValueWidth, _inputColorValueWidth = 0;
        int _lineColumn = 0;

        /// <summary>
        /// Standard
        /// </summary>
        /// <param name="kehren8"></param>
        /// <param name="forStockTV"></param>
        internal ScoreCardGrid(bool kehren8, bool forStockTV, bool opponentOnScoreCards)
        {
            SetWidthValues(kehren8, forStockTV, opponentOnScoreCards);

            #region ColumnDefinitions

            #region Moarschaft

            //Bahn#
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_fixedValuesWidth) });
            //Gegner Startnummer
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_fixedValuesWidth) });
            //Anspiel
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_fixedValuesWidth) });

            //Eingabe Farbe
            if (forStockTV) ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_inputColorValueWidth) });


            //Kehre 1 - 6,7,8
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_turnValuesWidth) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_turnValuesWidth) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_turnValuesWidth) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_turnValuesWidth) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_turnValuesWidth) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_turnValuesWidth) });
           
            if (!opponentOnScoreCards)
            {
                ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_turnValuesWidth) });

                if (kehren8)
                    ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_turnValuesWidth) });
            }

            //Summe
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_sumValueWidth) });
            //Strafpunkte
            if (!opponentOnScoreCards && !forStockTV) ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_penaltyValueWidth) });
            //Punkte
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_pointsValueWidth) });
            #endregion

            //Leer-Raum
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_spaceValueWidth) });

            #region Gegner

            //Spiel#
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_fixedValuesWidth) });

            //Kehre 1 - 6,7,8
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_turnValuesWidth) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_turnValuesWidth) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_turnValuesWidth) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_turnValuesWidth) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_turnValuesWidth) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_turnValuesWidth) });

            if (!opponentOnScoreCards)
            {
                ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_turnValuesWidth) });

                if (kehren8)
                    ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_turnValuesWidth) });
            }

            //Summe
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_sumValueWidth) });
            //Strafpunkte
            if (!opponentOnScoreCards && !forStockTV) ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_penaltyValueWidth) });
            //Punkte
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_pointsValueWidth) });

            //Gegner Name
            if (opponentOnScoreCards) ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(_opponentValueWidth) });

            #endregion

            #endregion

            var line = new Line
            {
                StrokeThickness = 0.75,
                X1 = 0,
                Y1 = 0,
                X2 = 0,
                Y2 = 0.1,
                Stroke = Brushes.Black,
                Stretch = Stretch.Fill,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            SetColumn(line, _lineColumn);
            SetRowSpan(line, 2);
            Children.Add(line);
        }

        private void SetWidthValues(bool kehren8, bool forStockTV, bool opponentName)
        {
            if (opponentName)
            {
                _fixedValuesWidth = PixelConverter.CmToPx(0.6);

                _turnValuesWidth = PixelConverter.CmToPx(0.65);
                _sumValueWidth = PixelConverter.CmToPx(1.0);
                _pointsValueWidth = PixelConverter.CmToPx(1.1);
                _spaceValueWidth = PixelConverter.CmToPx(0.3);
                _opponentValueWidth = PixelConverter.CmToPx(3.5);
                _lineColumn = 11;
            }
            else
            {
                _fixedValuesWidth = kehren8 || forStockTV ? PixelConverter.CmToPx(0.55) : PixelConverter.CmToPx(0.6);

                _turnValuesWidth = kehren8 ? PixelConverter.CmToPx(0.65) : PixelConverter.CmToPx(0.7);
                _sumValueWidth = kehren8 ? PixelConverter.CmToPx(1.0) : PixelConverter.CmToPx(1.2);
                _penaltyValueWidth = kehren8 ? PixelConverter.CmToPx(0.9) : PixelConverter.CmToPx(1.0);
                _pointsValueWidth = kehren8 ? PixelConverter.CmToPx(1.1) : PixelConverter.CmToPx(1.1);
                _spaceValueWidth = kehren8 ? PixelConverter.CmToPx(0.3) : PixelConverter.CmToPx(0.5);
                _inputColorValueWidth = PixelConverter.CmToPx(1.4);
                _lineColumn = kehren8 ? 14 : 13;
            }

        }

    }
}
