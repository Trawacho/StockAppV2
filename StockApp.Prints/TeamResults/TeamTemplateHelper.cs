using StockApp.Core.Turnier;
using StockApp.Lib;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System;
using System.Printing;
using StockApp.Lib.Models;
using System.Linq;
using System.Diagnostics;
using StockApp.Prints.Template;


namespace StockApp.Prints.Teamresult;

/* 
 * 
 * https://github.com/sherman89/WpfReporting
 * 
 */

public static class TeamTemplateFactory
{
    public static async Task<IDocumentPaginatorSource> Create(ITurnier turnier)
    {
        var token = new CancellationTokenSource();
        Func<UIElement> reportFactory = () => new TeamTemplate(new TeamTemplateViewModel(turnier));

        var paginator = new Paginator();
        var printing = new Printing();
        var helper = new TeamTemplateHelper(paginator, printing, turnier);
        await helper.LoadReport(reportFactory, token.Token);
        return helper.GeneratedDocument;
    }
}


internal class TeamTemplateHelper
{
    private readonly ITurnier _turnier;

    public TeamTemplateHelper(IPaginator paginator, IPrinting printing, ITurnier turnier)
    {
        _paginator = paginator;
        _printing = printing;
        _turnier = turnier;
    }

    /// <summary>
    /// XPS version of the FixedDocument. 
    /// </summary>
    private XpsDocument xpsDocument;

    private IDocumentPaginatorSource generatedDocument;
    public IDocumentPaginatorSource GeneratedDocument
    {
        get => generatedDocument;
        set => generatedDocument = value;

    }
    private readonly IPaginator _paginator;
    private readonly IPrinting _printing;

    private PrinterModel _selectedPrinter;
    public PrinterModel SelectedPrinter
    {
        get => _selectedPrinter;
        set => _selectedPrinter = value;
    }

    private PageSizeModel _selectedPageSize;
    public PageSizeModel SelectedPageSize
    {
        get => _selectedPageSize;
        set => _selectedPageSize = value;
    }

    private PageOrientationModel _selectedPageOrientation;
    public PageOrientationModel SelectedPageOrientation
    {
        get => _selectedPageOrientation;
        set => _selectedPageOrientation = value;
    }


    internal async Task LoadReport(Func<UIElement> reportFactory, CancellationToken cancellationToken)
    {
        string pdfPrinterName = "Microsoft Print to PDF";
        SelectedPrinter = _printing.GetPrinters().Where(t => t.FullName == pdfPrinterName).FirstOrDefault();
        SelectedPageSize = new PageSizeModel(new PageMediaSize(PageMediaSizeName.ISOA4));
        SelectedPageOrientation = new PageOrientationModel(PageOrientation.Portrait);

        var printTicket = _printing.GetPrintTicket(SelectedPrinter.FullName, SelectedPageSize.PageMediaSize, SelectedPageOrientation.PageOrientation);

        var printCapabilities = _printing.GetPrinterCapabilitiesForPrintTicket(printTicket, SelectedPrinter.FullName);

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

            xpsDocument = _printing.GetXpsDocumentFromFixedDocument(fixedDocument);
            GeneratedDocument = xpsDocument.GetFixedDocumentSequence();

        }
    }


    private void CleanXpsDocumentResources()
    {
        if (xpsDocument != null)
        {
            try
            {
                xpsDocument.Close();
                File.Delete(xpsDocument.Uri.AbsolutePath);
            }
            catch
            {
            }
            finally
            {
                xpsDocument = null;
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
