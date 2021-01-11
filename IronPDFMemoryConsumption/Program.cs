using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace IronPDFMemoryConsumption
{
	public static class Program
	{
		public static void Main(String[] args)
		{
			IServiceCollection serviceCollection = new ServiceCollection();
			serviceCollection.AddLogging();
			serviceCollection.AddScoped<IPdfPrinter, PdfPrinter>();
			serviceCollection.AddScoped<IMenuHandler, MenuHandler>();

			ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
			ILogger<PdfPrinter> logger = serviceProvider.GetService<ILogger<PdfPrinter>>();
			IMenuHandler menuHandler = serviceProvider.GetService<IMenuHandler>();

			// Explizitly left this out blank
			//IronPdf.License.LicenseKey = "IRONPDF.DEVTEAM....";

			try
			{
				menuHandler.ShowMenu();
			}
			catch (Exception exc)
			{
				logger.LogError(exc, "General error while showing menu.");
			}
		}
	}
}