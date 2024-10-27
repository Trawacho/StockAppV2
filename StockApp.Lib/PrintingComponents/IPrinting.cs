﻿using StockApp.Lib.PrintingComponents.Models;
using System.Collections.Generic;
using System.Printing;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;

namespace StockApp.Lib.PrintingComponents;

public interface IPrinting
{
	IReadOnlyList<PrinterModel> GetPrinters();

	IReadOnlyList<PrinterModel> GetPrinters(PrinterType printerTypes);

	PrintTicket GetPrintTicket(string printerName, PageMediaSize paperSize, PageOrientation pageOrientation);

	PrintCapabilities GetPrinterCapabilitiesForPrintTicket(PrintTicket printTicket, string printerName);

	Thickness GetMinimumPageMargins(PrintCapabilities printerCapabilities);

	XpsDocument GetXpsDocumentFromFixedDocument(FixedDocument fixedDocument);

	byte[] GetXpsFileBytesFromFixedDocument(FixedDocument fixedDocument);

	void PrintDocument(string printerName, IDocumentPaginatorSource document, string documentTitle, PrintTicket printTicket);
}
