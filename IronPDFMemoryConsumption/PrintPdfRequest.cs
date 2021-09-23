using System;
using System.Diagnostics.CodeAnalysis;

namespace IronPDFMemoryConsumption
{
	[ExcludeFromCodeCoverage]
	public class PrintPdfRequest
	{
		public PrintType PrintType { get; set; }
		public Byte[] FileData { get; set; }
		public String PrinterName { get; set; }
		public String DocumentName { get; set; }
	}
}