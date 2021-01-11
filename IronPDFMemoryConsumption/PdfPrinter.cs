using IronPdf;
using Microsoft.Extensions.Logging;
using System;
using System.Drawing.Printing;
using System.IO;

namespace IronPDFMemoryConsumption
{
	public class PdfPrinter : IPdfPrinter
	{
		private readonly ILogger<PdfPrinter> logger;

		public PdfPrinter(ILogger<PdfPrinter> logger)
		{
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public void Print(PrintPdfRequest data)
		{
			if (data is null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			using PdfDocument pdf = new PdfDocument(data.FileData);

			// Bug #2:
			// After calling PdfDocumet(Byte[]) constructor, the stored Binary[] is different than the Byte[] passed in.
			// Both can be compared here by writting both to file.
			// File.WriteAllBytes("OutputPdf_Broken.pdf", pdf.BinaryData);
			// File.WriteAllBytes("OutputPdf_Working.pdf", data.FileData);

			using PrintDocument printDocument = pdf.GetPrintDocument();
			printDocument.DocumentName = data.DocumentName;

			if (!String.IsNullOrWhiteSpace(data.PrinterName))
			{
				printDocument.PrinterSettings.PrinterName = data.PrinterName;
			}

			if (printDocument.PrinterSettings.PrinterName == "Microsoft Print to PDF")
			{
				String directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

				printDocument.PrinterSettings.PrintToFile = true;
				printDocument.PrinterSettings.PrintFileName = Path.Combine(directory, printDocument.DocumentName);
			}

			try
			{
				printDocument.Print();
			}
			catch (InvalidPrinterException printerException)
			{
				this.logger.LogError(printerException, "Printing failed, printer not found.");
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, $"Printing {printDocument.DocumentName} failed.");
			}
		}
	}
}