using StockApp.Lib.ReportPaginator;
using System;
using System.IO;
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

		Size pageSize = new(width: 793, height: 1122);
		//height: 1122, width: 793

		var desiredMargin = new Thickness(30);
		//bottom:16.0, left:22.7, right:22.7, top:16.2 ... werte von meinem Drucker

		//AdjustMargins(ref desiredMargin, printerMinMargins); --> nicht notwendig, da ich feste Werte hab

		_logger.Debug("Print Pagination start");
		var pages = await _paginator.PaginateAsync(reportFactory, tableHeaderFactory, pageSize, desiredMargin, cancellationToken);

		_logger.Debug($"Get a FixedDocument for {pages.Count} page(s)");
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




	private void CleanXpsDocumentResources()
	{
		if (_xpsDocument != null)
		{
			try
			{
				_xpsDocument.Close();
				File.Delete(_xpsDocument.Uri.AbsolutePath);
			}
			catch (Exception ex)
			{
				_logger.Error("Error while trying to delete old xps files", ex);
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
