namespace SFA.DAS.RoATPService.Importer
{
    using System;
    using System.Diagnostics;
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            var stopWatch = Stopwatch.StartNew();
            Console.WriteLine("ESFA RoATP Data Importer");

            if (args.Length != 1)
            {
                Console.WriteLine("Usage: dotnet RoATPImporter.dll [path to CSV file]");
                Environment.Exit(-1);
            }

            string csvFilePath = args[0];

            using (var reader = new StreamReader(csvFilePath))
            {
                CsvParser parser = new CsvParser();
                CsvImportResult results = parser.ParseCsvFile(reader);

                if (results.ErrorLog.Count == 0)
                {
                    RegisterImporter importer = new RegisterImporter();

                    importer.ImportRegisterEntries(results.Entries).GetAwaiter().GetResult();

                    Console.WriteLine(results.Entries.Count + " register entries imported successfully");
                }
                else
                {
                    foreach (var errorMessage in results.ErrorLog)
                    {
                        Console.WriteLine(errorMessage);
                    }
                }
                Console.WriteLine("Completed in " + stopWatch.ElapsedMilliseconds + "ms");
            }
        }
    }
}
