using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;

namespace PdfOcr
{
    public class PdfOcrTextExtractor : IAsyncDisposable
    {
        private readonly int _maxDegreeOfParallelism;
        private readonly OcrProcessor _ocrProcessor;

        public PdfOcrTextExtractor(int maxDegreeOfParallelism = 1)
        {
            _maxDegreeOfParallelism = maxDegreeOfParallelism;
            _ocrProcessor = new OcrProcessor(maxDegreeOfParallelism);
        }

        public async Task<string?> ExtractTextFromPdf(string pdfFileName)
        {
            ConcurrentBag<(int PageIdx, string Text)> pages = new();

            await foreach (var pageImgGroup in PdfImageConverter.RenderPdfToImageBatchesAsync(pdfFileName, _maxDegreeOfParallelism))
            {
                var pageImgGroupIndexed = pageImgGroup.Select((Image, Index) => (Image, Index));
                await Parallel.ForEachAsync(pageImgGroupIndexed, async (pageImg, _) =>
                {
                    var pageText = await _ocrProcessor.Process(pageImg.Image);
                    pages.Add((pageImg.Index, RemoveLinesWithoutText(pageText)));
                });
            }

            var pageTexts = pages
                .Where(p => !string.IsNullOrWhiteSpace(p.Text))
                .Select(p => p.Text);

            return pageTexts.Any() ? string.Join(Environment.NewLine, pageTexts) : null;
        }

        private string RemoveLinesWithoutText(string text)
        {
            var res = new StringBuilder();

            foreach (var line in text.Split(["\r\n", "\r", "\n"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (Regex.IsMatch(line, @"\w{2,}", RegexOptions.Compiled))
                {
                    res.AppendLine(line);
                }
            }

            return res.ToString();
        }

        public async ValueTask DisposeAsync()
        {
            await _ocrProcessor.DisposeAsync();
        }
    }
}
