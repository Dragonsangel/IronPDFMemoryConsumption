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

			switch (data.PrintType)
			{
				case PrintType.FromBytes: { this.PrintFromBytes(data); break; }
				case PrintType.FromHtml: { this.PrintFromHtml(data); break; }
			}
		}

		private void PrintFromBytes(PrintPdfRequest data)
		{
			using PdfDocument pdf = new(data.FileData);
			// Bug #2:
			// After calling the PdfDocumet(Byte[]) constructor, the stored Binary[] is different than the Byte[] passed in.
			// Both can be compared here by writting both to file.
			// File.WriteAllBytes("OutputPdf_Broken.pdf", pdf.BinaryData);
			// File.WriteAllBytes("OutputPdf_Working.pdf", data.FileData);

			PrintDocument(data, pdf);
		}

		private void PrintFromHtml(PrintPdfRequest data)
		{
			ChromePdfRenderer renderer = new ();
			renderer.RenderingOptions.FitToPaper = true;
			renderer.RenderingOptions.CssMediaType = IronPdf.Rendering.PdfCssMediaType.Screen;
			renderer.RenderingOptions.PrintHtmlBackgrounds = true;
			renderer.RenderingOptions.CreatePdfFormsFromHtml = true;

			using PdfDocument pdf = renderer.RenderHtmlFileAsPdf("SamplePdf.html");

			this.PrintDocument(data, pdf);
		}

		private void PrintDocument(PrintPdfRequest data, PdfDocument pdf)
		{
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