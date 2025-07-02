using PdfOcr;
using PdfTextExtractor.Db;
using Spectre.Console;

var inputFolder = @"D:\Z70ANIT1\PdfTextExtractor\pdfs";
var outputSqlite = @"D:\Z70ANIT1\PdfTextExtractor\result.sqlite";

AnsiConsole.MarkupLine("[bold green]Initialize...[/]");

await AnsiConsole.Progress()
    .HideCompleted(true)
    .Columns(
    [
        new TaskDescriptionColumn(),
        new ProgressBarColumn(),
        new PercentageColumn(),
        new SpinnerColumn()
    ])
    .StartAsync(async ctx =>
    {
        var pdfExtractTask = ctx.AddTask($"[green]Extracting Text from Pdfs[/]");
        pdfExtractTask.StartTask();
        await new PdfTextExtractorRunner().RunAsync(inputFolder, outputSqlite, (progress) => 
        {
            pdfExtractTask.Value = (double)progress.ProcessedCount / progress.TotalCount * 100;
            pdfExtractTask.Description($"[green]Extracting Text from Pdfs ({progress.ProcessedCount} of {progress.TotalCount})[/]");
        });

        pdfExtractTask.StopTask();
    });

AnsiConsole.MarkupLine($"[green]Successfully stored exported text from all Pdfs to: {outputSqlite}.[/]");

Console.ReadLine();

public class PdfTextExtractorRunner
{
    public async Task RunAsync(string inputFolder, string outputSqlite, Action<(int ProcessedCount, int TotalCount)>? reportProgress)
    {
        await SqliteDbCreator.InitPdfTextExtractorDbAsync(outputSqlite);
        await using var pdfTextExtractor = new PdfOcrTextExtractor(Environment.ProcessorCount * 2);
        var pdfs = GetPdfsFromFolder(inputFolder);
        var processedCount = 0;

        foreach (var pdfChunck in pdfs.Chunk(Environment.ProcessorCount)) 
        {
            await Parallel.ForEachAsync(pdfChunck, async (pdf, _) =>
            {
                var pdfText = await pdfTextExtractor.ExtractTextFromPdf(pdf);

                await PdfTextRepository.InsertPdfTextMappingAsync(outputSqlite, new PdfTextMappingModel(Path.GetFileName(pdf), pdfText));
            });

            if (reportProgress != null)
            {
                processedCount += pdfChunck.Count();
                reportProgress((processedCount, pdfs.Length));
            }
        }
    }

    private string[] GetPdfsFromFolder(string inputFolder)
    {
        return Directory.GetFiles(inputFolder, "*.pdf", SearchOption.TopDirectoryOnly);
    }
}