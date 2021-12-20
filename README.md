# IronPDFMemoryConsumption
Sample repository to reproduce that MemoryStreams are not released after printing with IronPDF.

Reproduction Steps:
1) Run the application and perform a "warm-up" with one `Print a single PDF from...` options.
2) Using the `Diagnostic Tools` > `Memory Usage`, take a snapshot of the current memory usage.
3) Perform any number of `Print 10 PDFs...` or `Print a single PDF from...`, noting how many PDFs have been generated.
4) Take another snapshot of the used memory and compare it to the first snapshot.

Within the comparison you should see instances of `MemoryStream` equal to the number of PDFs that were printed.

This application will generate and store the printed PDFs in your "Documents" folder, so please remember to clean that up after reproducing the issue :D.

Status: Memory leak has been **fixed** in IronPdf [2021.12.4495](https://www.nuget.org/packages/IronPdf/2021.12.4495)
