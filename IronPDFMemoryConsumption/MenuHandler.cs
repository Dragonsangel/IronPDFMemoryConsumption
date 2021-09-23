using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IronPDFMemoryConsumption
{
	public class MenuHandler : IMenuHandler
	{
		private readonly IServiceProvider serviceProvider;

		public MenuHandler(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
		}

		public void ShowMenu()
		{
			String menuOption = String.Empty;

			while (menuOption != "99")
			{
				this.ShowMainMenu();
				menuOption = Console.ReadLine();

				Dictionary<String, Func<Boolean>> knownCommands = new()
				{
					{ "1", () => this.PrintPdfFromFile(1) },
					{ "2", () => this.PrintPdfFromFile(10) },
					{ "3", () => this.PrintPdfFromJson(1) },
					{ "4", () => this.PrintPdfFromJson(10) },
					{ "5", () => this.PrintPdfFromHtml(1) },
					{ "6", () => this.PrintPdfFromHtml(10) },
					{ "99", () => this.ExitProgramm() },
				};

				if (knownCommands.Any(x => x.Key == menuOption))
				{
					Console.Clear();
					try
					{
						knownCommands[menuOption].Invoke();
					}
					catch (Exception exc)
					{
						Console.WriteLine($"Unhandled exception occured.! {exc.Message}");
						this.WriteOutInnerExceptions(exc, 0);
					}
				}
				else
				{
					this.UnknownCommand();
				}

				Console.ReadKey();
			}
		}

		private void ShowMainMenu()
		{
			Console.Clear();
			Console.WriteLine("=================================================");
			Console.WriteLine("Choose the required action");
			Console.WriteLine("-------------------------------------------------");
			Console.WriteLine("  1) Print a single PDF from Binary PDF File.");
			Console.WriteLine("  2) Print 10 PDFs from Binary PDF File.");
			Console.WriteLine("  3) Print a single PDF from JSON File with Base64 Data.");
			Console.WriteLine("  4) Print 10 PDFs from JSON File with Base64 Data.");
			Console.WriteLine("  5) Print a single PDF from HTML File.");
			Console.WriteLine("  6) Print 10 PDFs from HTML File."); 
			Console.WriteLine("  99) Quit");
			Console.WriteLine("=================================================");
		}

		private Boolean PrintPdfFromFile(Int32 count)
		{
			Byte[] sourceBytes = File.ReadAllBytes("SamplePdf.pdf");

			PrintPdfRequest printPdfRequest = new()
			{
				PrintType = PrintType.FromBytes,
				FileData = sourceBytes,
				DocumentName = $"SamplePdf_Printed_FromFile.pdf",
				PrinterName = "Microsoft Print to PDF"
			};

			this.PrintPdf(count, printPdfRequest);

			Console.WriteLine("All documents have been printed. Press any key to return to the menu.");
			return true;
		}

		private Boolean PrintPdfFromJson(Int32 count)
		{
			PrintPdfRequest printPdfRequest = JsonConvert.DeserializeObject<PrintPdfRequest>(File.ReadAllText("SamplePdfAsJson.json"));

			this.PrintPdf(count, printPdfRequest);

			Console.WriteLine("All documents have been printed. Press any key to return to the menu.");
			return true;
		}

		private Boolean PrintPdfFromHtml(Int32 count)
		{
			PrintPdfRequest printPdfRequest = new()
			{
				PrintType = PrintType.FromHtml,
				DocumentName = "SamplePdf_Printed_FromHtml.pdf",
				PrinterName = "Microsoft Print to PDF"
			};
			this.PrintPdf(count, printPdfRequest);

			Console.WriteLine("All documents have been printed. Press any key to return to the menu.");
			return true;
		}

		private void PrintPdf(Int32 count, PrintPdfRequest printPdfRequest)
		{
			String baseFileName = Path.GetFileNameWithoutExtension(printPdfRequest.DocumentName);
			using IServiceScope serviceScope = this.serviceProvider.CreateScope();
			IPdfPrinter pdfPrinter = serviceScope.ServiceProvider.GetRequiredService<IPdfPrinter>();

			for (Int32 i = 0; i < count; i++)
			{
				printPdfRequest.DocumentName = $"{baseFileName}_{DateTime.UtcNow:yyyyMMddHHmmssffff}.pdf";
				Console.Write($"Printig document {i + 1}/{count}.");
				pdfPrinter.Print(printPdfRequest);
				Console.WriteLine(" Done");
			}
		}

		private void WriteOutInnerExceptions(Exception exception, Int32 level)
		{
			if (exception.InnerException != null)
			{
				Console.WriteLine($"{(level < 1 ? "Exception" : "Innerexception")}: {exception.InnerException.Message}");
				this.WriteOutInnerExceptions(exception.InnerException, level + 1);
			}
		}

		private Boolean ExitProgramm()
		{
			Console.WriteLine("Quitting.");
			return true;
		}

		private Boolean UnknownCommand()
		{
			Console.WriteLine("Invalid option selected...");
			return true;
		}
	}
}