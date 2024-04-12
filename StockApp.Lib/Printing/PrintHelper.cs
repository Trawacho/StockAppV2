using System;
using System.IO;
using System.Printing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;

namespace StockApp.Lib;

/*
 * 
 * https://github.com/sherman89/WpfReporting
 * 
 */

public class PrintHelper
{
    private readonly IPaginator _paginator;
    private readonly IPrinting _printing;

    public PrintHelper()
    {
        _paginator = new Paginator();
        _printing = new Printing();
    }

    /// <summary>
    /// XPS version of the FixedDocument. 
    /// </summary>
    private XpsDocument _xpsDocument;

    private IDocumentPaginatorSource _generatedDocument;
    public IDocumentPaginatorSource GeneratedDocument
    {
        get => _generatedDocument;
        set => _generatedDocument = value;

    }


    public async Task LoadReport(Func<UIElement> reportFactory, CancellationToken cancellationToken)
    {
        var printTicket = new PrintTicket()
        {
            PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4),
            PageOrientation = PageOrientation.Portrait,
            CopyCount = 1,
            OutputColor = OutputColor.Color,
            Duplexing = Duplexing.TwoSidedLongEdge,
            PagesPerSheet = 1,
        };

        var printCapabilities = _printing.GetPrinterCapabilitiesForPrintTicket(printTicket, new System.Drawing.Printing.PrinterSettings().PrinterName);

        if (printCapabilities.OrientedPageMediaWidth.HasValue && printCapabilities.OrientedPageMediaHeight.HasValue)
        {
            var pageSize = new Size(printCapabilities.OrientedPageMediaWidth.Value, printCapabilities.OrientedPageMediaHeight.Value);

            var desiredMargin = new Thickness(15);
            var printerMinMargins = _printing.GetMinimumPageMargins(printCapabilities);
            AdjustMargins(ref desiredMargin, printerMinMargins);

            var pages = await _paginator.PaginateAsync(reportFactory, pageSize, desiredMargin, cancellationToken);
            var fixedDocument = _paginator.GetFixedDocumentFromPages(pages, pageSize);

            // We now could simply assign the fixedDocument to GeneratedDocument
            // But then for some reason the DocumentViewer search feature breaks
            // The solution is to create an XPS file first and get the FixedDocumentSequence
            // from it and then use that in the DocumentViewer

            // Delete old XPS file first
            CleanXpsDocumentResources();



            // Cause of an error with images in the xps file I was disable to save and load procedure
            // instead of that, I set the GeneratedDocument with the fixedDocument
            //
            //_xpsDocument = _printing.GetXpsDocumentFromFixedDocument(fixedDocument);
            //GeneratedDocument = _xpsDocument.GetFixedDocumentSequence();

            //v^v^v^v^v^v^v^v^v^v^v^v^v^v^v^v^v^v^v^v^v^v^v^v^v^v^
            GeneratedDocument = fixedDocument;
        }
    }


    private void CleanXpsDocumentResources()
    {
        if (_xpsDocument != null)
        {
            try
            {
                _xpsDocument.Close();
                File.Delete(_xpsDocument.Uri.AbsolutePath);
            }
            catch
            {
            }
            finally
            {
                _xpsDocument = null;
            }
        }
    }
    private static void AdjustMargins(ref Thickness pageMargins, Thickness minimumMargins)
    {
        if (pageMargins.Left < minimumMargins.Left)
        {
            pageMargins.Left = minimumMargins.Left;
        }

        if (pageMargins.Top < minimumMargins.Top)
        {
            pageMargins.Top = minimumMargins.Top;
        }

        if (pageMargins.Right < minimumMargins.Right)
        {
            pageMargins.Right = minimumMargins.Right;
        }

        if (pageMargins.Bottom < minimumMargins.Bottom)
        {
            pageMargins.Bottom = minimumMargins.Bottom;
        }
    }
}
