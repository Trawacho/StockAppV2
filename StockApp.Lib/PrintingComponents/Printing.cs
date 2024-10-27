﻿using StockApp.Lib.PrintingComponents.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;

namespace StockApp.Lib.PrintingComponents;

//todo: Logger implementieren

public class Printing : IPrinting
{
	/// <summary>
	/// Get all printers in <see cref="PrinterType"/>.
	/// </summary>
	public IReadOnlyList<PrinterModel> GetPrinters()
	{
		const PrinterType flags = PrinterType.Usb | PrinterType.Pdf | PrinterType.Xps | PrinterType.Network;
		return GetPrinters(flags);
	}

	public IReadOnlyList<PrinterModel> GetPrinters(PrinterType printerTypes)
	{
		var printers = new List<PrinterModel>();

		PrintQueueCollection localQueues;
		using (var printServer = new LocalPrintServer())
		{
			var flags = new[] { EnumeratedPrintQueueTypes.Local };
			localQueues = printServer.GetPrintQueues(flags);
		}

		if (printerTypes.HasFlag(PrinterType.Usb))
		{
			var usbPrinters = localQueues.Where(q => q.QueuePort.Name.StartsWith("USB"));
			foreach (var usbPrinter in usbPrinters)
			{
				var printCapabilities = usbPrinter.GetPrintCapabilities();
				var pageSizeCapabilities = printCapabilities.PageMediaSizeCapability;
				var pageOrientationCapabilities = GetPageOrientationCapability(printCapabilities);
				printers.Add(new PrinterModel(usbPrinter.FullName, PrinterType.Usb, pageSizeCapabilities, pageOrientationCapabilities));
			}
		}

		if (printerTypes.HasFlag(PrinterType.Pdf))
		{
			var pdfPrintQueue = localQueues.SingleOrDefault(lq => lq.QueueDriver.Name == Constants.PdfPrinterDriveName);
			if (pdfPrintQueue != null)
			{
				var printCapabilities = pdfPrintQueue.GetPrintCapabilities();
				var pageSizeCapabilities = printCapabilities.PageMediaSizeCapability;
				var pageOrientationCapabilities = GetPageOrientationCapability(printCapabilities);
				printers.Add(new PrinterModel(pdfPrintQueue.FullName, PrinterType.Pdf, pageSizeCapabilities, pageOrientationCapabilities));
			}
		}

		if (printerTypes.HasFlag(PrinterType.Xps))
		{
			var xpsPrintQueue = localQueues.SingleOrDefault(lq => lq.QueueDriver.Name == Constants.XpsPrinterDriveName);
			if (xpsPrintQueue != null)
			{
				var printCapabilities = xpsPrintQueue.GetPrintCapabilities();
				var pageSizeCapabilities = printCapabilities.PageMediaSizeCapability;
				var pageOrientationCapabilities = GetPageOrientationCapability(printCapabilities);
				printers.Add(new PrinterModel(xpsPrintQueue.FullName, PrinterType.Xps, pageSizeCapabilities, pageOrientationCapabilities));
			}
		}

		if (printerTypes.HasFlag(PrinterType.Network))
		{
			PrintQueueCollection networkQueues;
			using (var printServer = new LocalPrintServer())
			{
				var flags = new[] { EnumeratedPrintQueueTypes.Connections };
				networkQueues = printServer.GetPrintQueues(flags);
			}

			foreach (var networkQueue in networkQueues)
			{
				var printCapabilities = networkQueue.GetPrintCapabilities();
				var pageSizeCapabilities = printCapabilities.PageMediaSizeCapability;
				var pageOrientationCapabilities = GetPageOrientationCapability(printCapabilities);
				printers.Add(new PrinterModel(networkQueue.FullName, PrinterType.Network, pageSizeCapabilities, pageOrientationCapabilities));
				networkQueue.Dispose();
			}
		}

		foreach (var localQueue in localQueues)
		{
			localQueue.Dispose();
		}

		return printers;
	}

	private static ReadOnlyCollection<PageOrientation> GetPageOrientationCapability(PrintCapabilities printCapabilities)
	{
		// I have only tested Portrain and Landscape printing, not sure if others will work.
		return new ReadOnlyCollection<PageOrientation>(printCapabilities.PageOrientationCapability
			.Where(poc => poc == PageOrientation.Portrait || poc == PageOrientation.Landscape).ToList());
	}

	public PrintTicket GetPrintTicket(string printerName, PageMediaSize paperSize, PageOrientation pageOrientation)
	{
		using (var printQueue = GetPrintQueue(printerName))
		{
			if (printQueue != null)
			{
				var myTicket = new PrintTicket
				{
					CopyCount = 1,
					PageOrientation = pageOrientation,
					OutputColor = OutputColor.Color,
					PageMediaSize = paperSize
				};

				var mergeTicketResult = printQueue.MergeAndValidatePrintTicket(printQueue.DefaultPrintTicket, myTicket);

				var isValid = ValidateMergedPrintTicket(myTicket, mergeTicketResult.ValidatedPrintTicket);
				if (isValid)
				{
					return mergeTicketResult.ValidatedPrintTicket;
				}
				else
				{
					throw new Exception($"PrintTicket settings are incompatible with printer.");
				}
			}
		}

		throw new Exception($"Printer name \"{printerName}\" not found in local or network queues.");
	}

	private static bool ValidateMergedPrintTicket(PrintTicket desiredTicket, PrintTicket actualTicket)
	{
		return desiredTicket.PageMediaSize.PageMediaSizeName == actualTicket.PageMediaSize.PageMediaSizeName &&
			desiredTicket.PageOrientation == actualTicket.PageOrientation &&
			desiredTicket.OutputColor == actualTicket.OutputColor &&
			desiredTicket.CopyCount == actualTicket.CopyCount;
	}

	public PrintCapabilities GetPrinterCapabilitiesForPrintTicket(PrintTicket printTicket, string printerName)
	{
		using var queue = GetPrintQueue(printerName);
		return queue?.GetPrintCapabilities(printTicket);
	}

	private static PrintQueue GetPrintQueue(string printerName)
	{
		using var printServer = new LocalPrintServer();
		// GetPrintQueue(queueName) might not work with some types of network printers,
		// but giving the queue description strangely works, but this is not a safe solution.
		// Instead we just get all queues and filter them, that always works.
		var queues = printServer.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections });
		return queues.SingleOrDefault(pq => pq.FullName == printerName);
	}

	/// <summary>
	/// Returns the minimum page margins supported by the printer for a specific page size. Make sure the <see cref="PrintCapabilities"/> parameter
	/// contains the correct printer and page size, otherwise you will get the wrong margins.
	/// </summary>
	/// <param name="printCapabilities"><see cref="PrintCapabilities"/> for a specific printer and page size.</param>
	/// <returns>Minimum margins that this printer supports for a given page size.</returns>
	/// <exception cref="ArgumentNullException">
	/// <paramref name="printCapabilities" /> is <see langword="null" />.</exception>
	public Thickness GetMinimumPageMargins(PrintCapabilities printCapabilities)
	{

#pragma warning disable CA2208
		if (printCapabilities is null)
		{
			throw new ArgumentNullException(nameof(PrintCapabilities), $"{nameof(printCapabilities)} cannot be null.");
		}

		if (!printCapabilities.OrientedPageMediaWidth.HasValue)
		{
			throw new ArgumentNullException(nameof(printCapabilities.OrientedPageMediaWidth), $"{nameof(printCapabilities.OrientedPageMediaWidth)} cannot be null.");
		}

		if (!printCapabilities.OrientedPageMediaHeight.HasValue)
		{
			throw new ArgumentNullException(nameof(printCapabilities.OrientedPageMediaHeight), $"{nameof(printCapabilities.OrientedPageMediaHeight)} cannot be null.");
		}

		if (printCapabilities.PageImageableArea == null)
		{
			throw new ArgumentNullException(nameof(printCapabilities.PageImageableArea), $"{nameof(printCapabilities.PageImageableArea)} cannot be null.");
		}
#pragma warning restore CA2208

		var minLeftMargin = printCapabilities.PageImageableArea.OriginWidth;
		var minTopMargin = printCapabilities.PageImageableArea.OriginHeight;
		var minRightMargin = printCapabilities.OrientedPageMediaWidth.Value - printCapabilities.PageImageableArea.ExtentWidth - minLeftMargin;
		var minBottomMargin = printCapabilities.OrientedPageMediaHeight.Value - printCapabilities.PageImageableArea.ExtentHeight - minTopMargin;

		return new Thickness(minLeftMargin, minTopMargin, minRightMargin, minBottomMargin);
	}

	/// <summary>
	/// Writes the <see cref="FixedDocument"/> to an <see cref="XpsDocument"/> and saves it as at temporary file.
	/// The temporary file is then read and returned as an <see cref="XpsDocument"/>. This step is necessary because
	/// it's the only way I could get the <see cref="DocumentViewer"/> search feature to work.
	/// </summary>
	public XpsDocument GetXpsDocumentFromFixedDocument(FixedDocument fixedDocument)
	{
		if (fixedDocument == null)
		{
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
			throw new ArgumentNullException(nameof(FixedDocument), "FixedDocument cannot be null.");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
		}

		var appDataLocalPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PrintPreviewGui");
		Directory.CreateDirectory(appDataLocalPath);

		var randomFileName = Path.ChangeExtension(Path.GetRandomFileName(), ".xps");
		var tempFilePath = @$"{appDataLocalPath}\{randomFileName}";

		var xpsDoc = new XpsDocument(tempFilePath, FileAccess.Write);
		var writer = XpsDocument.CreateXpsDocumentWriter(xpsDoc);
		writer.Write(fixedDocument);
		xpsDoc.Close();

		var doc = new XpsDocument(tempFilePath, FileAccess.Read, CompressionOption.NotCompressed);
		return doc;
	}

	/// <summary>
	/// Writes the <see cref="FixedDocument"/> to an <see cref="XpsDocument"/> in memory and returns it as a bytearray.
	/// </summary>
	public byte[] GetXpsFileBytesFromFixedDocument(FixedDocument fixedDocument)
	{
		if (fixedDocument == null)
		{
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
			throw new ArgumentNullException(nameof(FixedDocument), "FixedDocument cannot be null.");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
		}

		// Convert FixedDocument to XPS file in memory
		var ms = new MemoryStream();
		var package = Package.Open(ms, FileMode.Create);
		var doc = new XpsDocument(package);
		var writer = XpsDocument.CreateXpsDocumentWriter(doc);
		writer.Write(fixedDocument);
		doc.Close();
		package.Close();

		// Get XPS file bytes
		var bytes = ms.ToArray();
		ms.Dispose();

		return bytes;
	}

	public void PrintDocument(string printerName, IDocumentPaginatorSource document, string documentTitle, PrintTicket printTicket)
	{
		if (!printTicket.PageMediaSize.Width.HasValue || !printTicket.PageMediaSize.Height.HasValue)
		{
			throw new Exception("PrintTicket missing page size information.");
		}

		using var printQueue = GetPrintQueue(printerName);
		var dlg = new PrintDialog
		{
			PrintTicket = printTicket,
			PrintQueue = printQueue,
		};

		document.DocumentPaginator.PageSize = new Size(printTicket.PageMediaSize.Width.Value, printTicket.PageMediaSize.Height.Value);
		dlg.PrintDocument(document.DocumentPaginator, documentTitle);
	}
}
