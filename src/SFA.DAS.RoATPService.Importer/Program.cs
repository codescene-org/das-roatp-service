namespace SFA.DAS.RoATPService.Importer
{
    using Microsoft.Extensions.Configuration;
    using SFA.DAS.RoATPService.Importer.Exceptions;
    using SFA.DAS.RoATPService.Importer.Models;
    using SFA.DAS.RoATPService.Importer.Parsers;
    using SFA.DAS.RoATPService.Settings;
    using System;
    using System.Diagnostics;
    using System.IO;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    class Program
    {
        private const string ServiceName = "SFA.DAS.RoATPService";
        private const string Version = "1.0";
        public static IConfiguration Configuration { get; set; }
        public static IWebConfiguration ApplicationConfiguration { get; set; }

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

            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            ApplicationConfiguration = ConfigurationService.GetConfig(Configuration["EnvironmentName"], Configuration["ConfigurationStorageConnectionString"], Version, ServiceName).Result;

            using (var reader = new StreamReader(csvFilePath))
            {
                CsvParser parser = new CsvParser(new Logger<CsvParser>(NullLoggerFactory.Instance));
                CsvImportResult results = parser.ParseCsvFile(reader);

                if (results.ErrorLog.Count == 0)
                {
                    RegisterImporter importer = new RegisterImporter(new Logger<RegisterImporter>(NullLoggerFactory.Instance));

                    try
                    {
                        importer.ImportRegisterEntries(ApplicationConfiguration.SqlConnectionString, results.Entries).GetAwaiter().GetResult();
                    }
                    catch (RegisterImportException importException)
                    {
                        Console.WriteLine("Unexpected error when importing organisation " + importException.UKPRN);
                        Console.WriteLine(importException.ImportErrorMessage);
                        Environment.Exit(-1);
                    }

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
