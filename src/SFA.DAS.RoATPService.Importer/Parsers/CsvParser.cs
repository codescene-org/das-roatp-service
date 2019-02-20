namespace SFA.DAS.RoATPService.Importer.Parsers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using CsvHelper;
    using CsvHelper.TypeConversion;
    using Exceptions;
    using SFA.DAS.RoATPService.Importer.Models;

    public class CsvParser
    {
        public CsvImportResult ParseCsvFile(StreamReader csvFileReader)
        {
            List<RegisterEntry> entries = new List<RegisterEntry>();
            List<string> errorLog = new List<string>();

            using (var csvReader = new CsvReader(csvFileReader))
            {
                csvReader.Configuration.CultureInfo = CultureInfo.CreateSpecificCulture("en-GB");
                int rowNumber = 1;
                while (csvReader.Read())
                {
                    try
                    {
                        var record = csvReader.GetRecord<RegisterEntry>();

                        RegisterEntryValidator validator = new RegisterEntryValidator();
                        RegisterEntryValidationResult validationResult = validator.ValidateRegisterEntry(record);

                        if (!validationResult.IsValid)
                        {
                            errorLog.Add("Error on row " + rowNumber);

                            foreach (var errorMessage in validationResult.ValidationMessages)
                            {
                                errorLog.Add(errorMessage);
                            }

                            continue;
                        }

                        entries.Add(record);
                        rowNumber++;
                    }
                    catch (TypeConverterException typeConverterException)
                    {
                        errorLog.Add("Error on row " + typeConverterException.ReadingContext.Row);
                        errorLog.Add("Invalid data:" + typeConverterException.ReadingContext.RawRecord);
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
    }
}
