namespace SFA.DAS.RoATPService.Importer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Settings;
    using SFA.DAS.RoATPService.Api.Types.Models;
    using SFA.DAS.RoATPService.Importer.Exceptions;
    using SFA.DAS.RoATPService.Importer.Models;
    using SFA.DAS.RoATPService.Importer.Parsers;

    public class RegisterImportRepository : IRegisterImportRepository
    {
        private IConfiguration AppConfiguration { get; }

        private IWebConfiguration WebConfiguration { get; }

        public RegisterImportRepository(IConfiguration appConfiguration, IWebConfiguration webConfiguration)
        {
            AppConfiguration = appConfiguration;
            WebConfiguration = webConfiguration;
        }

        public async Task<RegisterImportResultsResponse> ImportRegisterData(string containerName, string blobReference)
        {
            var stopWatch = Stopwatch.StartNew();
            
            var importResults = new RegisterImportResultsResponse
            {
                EntriesImported = 0,
                Success = false,
                ErrorMessages = new List<string>(),
                ElapsedTimeMs = 0
            };

            if (String.IsNullOrWhiteSpace(containerName) || String.IsNullOrWhiteSpace(blobReference))
            {
                return await Task.FromResult(importResults);
            }

            CloudStorageAccount storageAccount = null;
            CloudStorageAccount.TryParse(AppConfiguration["ConfigurationStorageConnectionString"], out storageAccount);
                
            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = cloudBlobClient.GetContainerReference(containerName);

            CloudBlob blob = container.GetBlobReference(blobReference);

            var blobStream = new MemoryStream();
            Task downloadTask = blob.DownloadToStreamAsync(blobStream);

            downloadTask.GetAwaiter().GetResult();

            blobStream.Position = 0;

            using (var reader = new StreamReader(blobStream))
            {
                CsvParser parser = new CsvParser();
                CsvImportResult results = parser.ParseCsvFile(reader);

                if (results.ErrorLog.Count == 0)
                {
                    RegisterImporter importer = new RegisterImporter(WebConfiguration.SqlConnectionString);

                    try
                    {
                        importer.ImportRegisterEntries(results.Entries).GetAwaiter().GetResult();
                    }
                    catch (RegisterImportException importException)
                    {
                        importResults.ErrorMessages.Add("Unexpected error when importing organisation " + importException.UKPRN);
                        importResults.ErrorMessages.Add(importException.ImportErrorMessage);
                        return await Task.FromResult(importResults);
                    }

                    importResults.Success = true;
                    importResults.EntriesImported = results.Entries.Count;
                }
                else
                {
                    foreach (var errorMessage in results.ErrorLog)
                    {
                        importResults.ErrorMessages.Add(errorMessage);
                    }
                }

                importResults.ElapsedTimeMs = stopWatch.ElapsedMilliseconds;
                return await Task.FromResult(importResults);
            }
        }
    }
}
