using System.Data.SQLite;

namespace PdfTextExtractor.Db
{
    public static class SqliteDbCreator
    {
        public static async Task InitPdfTextExtractorDbAsync(string dbFilename)
        {
            if (File.Exists(dbFilename))
            {
                throw new IOException($"Database file name {dbFilename} already exists.");
            }

            if (string.IsNullOrEmpty(dbFilename))
                throw new ArgumentNullException(dbFilename);
            if (!dbFilename.ToLower().EndsWith(".sqlite"))
                throw new ArgumentException($"Provided file name {dbFilename} is not a valid sqlite path.");
            if (File.Exists(dbFilename))
            {
                throw new IOException($"Database file name {dbFilename} already exists.");
            }

            var connectionString = SqliteConnectionStringFactory.CreateConnectionString(dbFilename);
            SQLiteConnection.CreateFile(dbFilename);
            await CreatePdfTextExtractorTableAsync(connectionString);
        }

        private static async Task CreatePdfTextExtractorTableAsync(string connectionString)
        {
            await using var connection = new SQLiteConnection(connectionString);
            await connection.OpenAsync();
            
            await using var createTableCommand = new SQLiteCommand("""
                CREATE TABLE pdf_text (
                    pdf_name TEXT NOT NULL,
                    pdf_text TEXT
                );
                """, connection);
            await createTableCommand.ExecuteNonQueryAsync();
        }
    }
}
