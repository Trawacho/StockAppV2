using StockApp.Core.Turnier;
using StockApp.Lib;
using StockApp.Prints.Template;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Documents;
using System.Windows;

namespace StockApp.Prints.ZielResult;

public static class ZielResultFactory
{
    public static async Task<IDocumentPaginatorSource> Create(ITurnier turnier)
    {
        UIElement reportFactory() => new TeamTemplate(new ZielResultViewModel(turnier));

        var helper = new PrintHelper();
        await helper.LoadReport(reportFactory, CancellationToken.None);
        return helper.GeneratedDocument;
    }
}
