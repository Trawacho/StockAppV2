using StockApp.UI.Components;
using System.Windows.Documents;

namespace StockApp.UI.Extensions;

internal static class FixedDocumentExtensions
{
    internal static bool? ShowAsDialog(this FixedDocument value)
    {
        return new PrintPreview(value).ShowDialog();
    }
}