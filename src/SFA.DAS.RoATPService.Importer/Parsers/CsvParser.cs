namespace SFA.DAS.RoATPService.Importer.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using CsvHelper;
    using CsvHelper.TypeConversion;
    using Exceptions;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.RoATPService.Importer.Models;

    public class CsvParser : ICsvParser
    {
        private ILogger<CsvParser> _logger;

        public CsvParser(ILogger<CsvParser> logger)
        {
            _logger = logger;
        }

        public CsvImportResult ParseCsvFile(StreamReader csvFileReader)
        {
            List<RegisterEntry> entries = new List<RegisterEntry>();
            List<string> errorLog = new List<string>();

            using (var csvReader = new CsvReader(csvFileReader))
            {
                csvReader.Configuration.CultureInfo = CultureInfo.CreateSpecificCulture("en-GB");
                int rowNumber = 1; // include the header
                while (csvReader.Read())
                {
                    try
                    {
                        var record = csvReader.GetRecord<RegisterEntry>();
                        rowNumber++;
                        RegisterEntryValidator validator = new RegisterEntryValidator();
                        RegisterEntryValidationResult validationResult = validator.ValidateRegisterEntry(record);

                        if (!validationResult.IsValid)
                        {
                            var validationErrors = new List<string>();
                            validationErrors.Add("Error on row " + rowNumber);

                            foreach (var errorMessage in validationResult.ValidationMessages)
                            {
                                validationErrors.Add(errorMessage);
                            }
                            LogErrors(errorLog, validationErrors);

                            continue;
                        }

                        entries.Add(record);
                    }
                    catch (TypeConverterException typeConverterException)
                    {
                        var conversionErrors = new List<string>();
                        conversionErrors.Add("Error on row " + typeConverterException.ReadingContext.Row);
                        conversionErrors.Add("Invalid data:" + typeConverterException.ReadingContext.RawRecord);
                        LogErrors(errorLog, conversionErrors);
                    }
                    catch (HeaderValidationException headerValidationException)
                    {
                        throw new RegisterImportException(headerValidationException.Message)
                        {
                            ImportErrorMessage = $"Invalid header at index {headerValidationException.HeaderNameIndex}"
                        };
                    }
                }
            }

            return new CsvImportResult {Entries = entries, ErrorLog = errorLog};
        }

        private void LogErrors(List<string> errorLog, List<string> errorMessages)
        {
            errorLog.AddRange(errorMessages);

            string logErrorMessages = String.Join('\n', errorMessages);
            string errorMessage = $"Error encountered during the register import\n{logErrorMessages}";
            _logger.LogError(errorMessage);
        }
    }
}
