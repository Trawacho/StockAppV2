using StockApp.Core.Turnier;
using StockApp.Lib;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace StockApp.Prints.Receipts;

public static class ReceiptsFactory
{
    public static async Task<IDocumentPaginatorSource> Create(ITurnier turnier)
    {
        UIElement reportFactory() => new ReceiptTemplate(new ReceiptTemplateViewModel(turnier));

        var helper = new PrintHelper();
        await helper.LoadReport(reportFactory, CancellationToken.None);
        return helper.GeneratedDocument;
    }
}
