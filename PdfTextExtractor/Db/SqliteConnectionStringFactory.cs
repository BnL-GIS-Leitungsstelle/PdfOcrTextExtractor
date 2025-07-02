namespace PdfTextExtractor.Db
{
    public static class SqliteConnectionStringFactory
    {
        public static string CreateConnectionString(string dbFilename)
        {
            if (string.IsNullOrEmpty(dbFilename))
                throw new ArgumentNullException(dbFilename);
            if (!dbFilename.ToLower().EndsWith(".sqlite"))
                throw new ArgumentException($"Provided file name {dbFilename} is not a valid sqlite path.");

            return $"Data Source={dbFilename};Version=3;";
        }
    }
}
