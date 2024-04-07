using StockApp.Core.Turnier;
using StockApp.Lib;
using StockApp.Prints.Template;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace StockApp.Prints.Teamresult;

public static class TeamTemplateFactory
{
    public static async Task<IDocumentPaginatorSource> Create(ITurnier turnier)
    {
        UIElement reportFactory() => new TeamTemplate(new TeamTemplateViewModel(turnier));

        var helper = new PrintHelper();
        await helper.LoadReport(reportFactory, CancellationToken.None);
        return helper.GeneratedDocument;
    }
}
