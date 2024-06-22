using StockApp.Prints.Converters;
using System.Windows;
using System.Windows.Controls;

namespace StockApp.Prints.ScoreCards.Base;

internal class GameSummaryGrid : ScoreCardGrid
{
    internal GameSummaryGrid(bool is8TurnsGame, bool forStockTv, bool opponentOnScoreCards) : base(is8TurnsGame, forStockTv, opponentOnScoreCards)
    {
        RowDefinitions.Add(new RowDefinition()
        {
            Height = new GridLength(PixelConverter.CmToPx(0.6))
        });

        var textBlockWerbung = new TextBlock()
        {
            Text = "created by StockApp",
            FontSize = 7.0,
            VerticalAlignment = VerticalAlignment.Bottom
        };
        SetColumn(textBlockWerbung, 0);
        SetColumnSpan(textBlockWerbung, 8);
        Children.Add(textBlockWerbung);

        int startColumn = is8TurnsGame ? 9 : 8;
        startColumn += forStockTv ? 1 : 0;
        if (opponentOnScoreCards) startColumn = 7;


        var textBlockGesamt = new TextBlockGesamt();
        SetColumn(textBlockGesamt, startColumn);
        SetColumnSpan(textBlockGesamt, 2);
        Children.Add(textBlockGesamt);
        startColumn += 2;

        var BorderSumme = new ScoreCardField(string.Empty, 0);
        BorderSumme.Textblock.FontWeight = FontWeights.Bold;
        SetColumn(BorderSumme, startColumn);
        Children.Add(BorderSumme);
        startColumn++;

        if (!opponentOnScoreCards && !forStockTv)
        {
            var BorderStrafSumme = new ScoreCardField(string.Empty, 0);
            SetColumn(BorderStrafSumme, startColumn);
            Children.Add(BorderStrafSumme);
            startColumn++;
        }

        var BorderPunkte = new ScoreCardField(string.Empty, 0);
        BorderPunkte.Textblock.FontWeight = FontWeights.Bold;
        SetColumn(BorderPunkte, startColumn);
        Children.Add(BorderPunkte);

        startColumn = opponentOnScoreCards
                        ? startColumn + 7
                        : is8TurnsGame
                            ? startColumn + 9
                            : startColumn + 8;

        var textBlockGesamtG = new TextBlockGesamt();
        SetColumn(textBlockGesamtG, startColumn);
        SetColumnSpan(textBlockGesamtG, 2);
        Children.Add(textBlockGesamtG);
        startColumn += 2;

        var BorderSummeG = new ScoreCardField(string.Empty, 0);
        BorderSummeG.Textblock.FontWeight = FontWeights.Bold;
        SetColumn(BorderSummeG, startColumn);
        Children.Add(BorderSummeG);
        startColumn++;

        if (!opponentOnScoreCards && !forStockTv)
        {
            var BorderStrafSummeG = new ScoreCardField(string.Empty, 0);
            SetColumn(BorderStrafSummeG, startColumn);
            Children.Add(BorderStrafSummeG);
            startColumn++;
        }

        var BorderPunkteG = new ScoreCardField(string.Empty, 0);
        BorderPunkteG.Textblock.FontWeight = FontWeights.Bold;
        SetColumn(BorderPunkteG, startColumn);
        Children.Add(BorderPunkteG);
    }

    internal GameSummaryGrid(bool is8TurnsGame, bool opponentOnScoreCards):this(is8TurnsGame, false, opponentOnScoreCards) { }
}
