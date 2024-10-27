using StockApp.Lib.ReportPaginator;
using System;
using System.IO;
using System.Printing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;

namespace StockApp.Lib.PrintingComponents;

/*
 * 
 * https://github.com/sherman89/WpfReporting
 * 
 */
public class PrintHelper
{
	private readonly IPaginator _paginator;
	private readonly IPrinting _printing;
	private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
		=> await LoadReport(reportFactory, null, cancellationToken);

	public async Task LoadReport(Func<UIElement> reportFactory, Func<UIElement> tableHeaderFactory, CancellationToken cancellationToken)
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
		PrintCapabilities printCapabilities = default;
		try
		{
			var allPrinters = System.Drawing.Printing.PrinterSettings.InstalledPrinters;
			var printerName = new System.Drawing.Printing.PrinterSettings().PrinterName;

			

			_logger.Debug($"Get capabilities from printer: {printerName}");
			printCapabilities = _printing.GetPrinterCapabilitiesForPrintTicket(printTicket, printerName);
		}
		catch (Exception ex)
		{
			_logger.Error("GetPrinterCapabilitiesForPrintTicket", ex);
		}

		if (printCapabilities.OrientedPageMediaWidth.HasValue && printCapabilities.OrientedPageMediaHeight.HasValue)
		{
			//todo: PageSize und Margins auf feste Werte umstellen
			var pageSize = new Size(printCapabilities.OrientedPageMediaWidth.Value, printCapabilities.OrientedPageMediaHeight.Value);
			//height: 1122, width: 793

			var desiredMargin = new Thickness(15);
			var printerMinMargins = _printing.GetMinimumPageMargins(printCapabilities);
			//bottom:16.0, left:22.7, right:22.7, top:16.2
			AdjustMargins(ref desiredMargin, printerMinMargins);

			_logger.Debug("Print Pagination start");
			var pages = await _paginator.PaginateAsync(reportFactory, tableHeaderFactory, pageSize, desiredMargin, cancellationToken);

			_logger.Debug("Get Fixed Document");
			var fixedDocument = _paginator.GetFixedDocumentFromPages(pages, pageSize);

			// We now could simply assign the fixedDocument to GeneratedDocument
			// But then for some reason the DocumentViewer search feature breaks
			// The solution is to create an XPS file first and get the FixedDocumentSequence
			// from it and then use that in the DocumentViewer

			_logger.Debug("Delete old XPS files first");
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
				_logger.Error("Error while trying to delete old xps files");
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
