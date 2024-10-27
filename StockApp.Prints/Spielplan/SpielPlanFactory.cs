using StockApp.Core.Turnier;
using StockApp.Lib.PrintingComponents;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace StockApp.Prints.Spielplan;

public static class SpielPlanFactory
{
    public static async Task<IDocumentPaginatorSource> Create(ITurnier turnier)
    {
        UIElement reportFactory() => new SpielPlanTemplate(new SpielPlanTemplateViewModel(turnier));

        var helper = new PrintHelper();
        await helper.LoadReport(reportFactory, CancellationToken.None);
        return helper.GeneratedDocument;
    }
}

