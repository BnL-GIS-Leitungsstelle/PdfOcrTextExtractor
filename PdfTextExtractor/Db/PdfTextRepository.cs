using System.Data.SQLite;

namespace PdfTextExtractor.Db
{
    public static class PdfTextRepository
    {
        public static async Task InsertPdfTextMappingAsync(string dbFilename, PdfTextMappingModel pdfTextMapping)
        {
            var connectionString = SqliteConnectionStringFactory.CreateConnectionString(dbFilename);
            await using var connection = new SQLiteConnection(connectionString);
            await connection.OpenAsync();

            await using var insertPdfTextCommand = new SQLiteCommand("""
                INSERT INTO pdf_text (pdf_name, pdf_text)
                VALUES (@pdf_name, @pdf_text);
                """, connection);
            insertPdfTextCommand.Parameters.AddWithValue("@pdf_name", pdfTextMapping.PdfName);
            insertPdfTextCommand.Parameters.AddWithValue("@pdf_text", pdfTextMapping.PdfText);

            await insertPdfTextCommand.ExecuteNonQueryAsync();
        }
    }
}
