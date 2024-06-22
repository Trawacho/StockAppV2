using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace StockApp.Prints.ScoreCards;

public static class ScoreCardsFactory
{
    public static async Task<IDocumentPaginatorSource> Create(ITeamBewerb bewerb, int startNummer, bool namesOnScoreCard, bool summarizedScoreCards, bool opponentOnScoreCard)
    {
        UIElement reportFactory() => new ScoreCardTemplate(new ScoreCardTemplateViewModel(bewerb, startNummer, namesOnScoreCard, summarizedScoreCards, opponentOnScoreCard));

        var helper = new PrintHelper();
        await helper.LoadReport(reportFactory, CancellationToken.None);
        return helper.GeneratedDocument;
    }

    public static async Task<IDocumentPaginatorSource> Create(ITeamBewerb bewerb, bool namesOnScoreCard, bool summarizedScoreCards, bool opponentOnScoreCard)
    {
        UIElement reportFactory() => new ScoreCardTemplate(new ScoreCardTemplateViewModel(bewerb, namesOnScoreCard, summarizedScoreCards, opponentOnScoreCard));

        var helper = new PrintHelper();
        await helper.LoadReport(reportFactory, CancellationToken.None);
        return helper.GeneratedDocument;
    }
}