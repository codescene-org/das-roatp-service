namespace SFA.DAS.RoATPService.Importer
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using CsvHelper;
    using CsvHelper.TypeConversion;

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
                    catch (TypeConverterException ex)
                    {
                        errorLog.Add("Error on row " + ex.ReadingContext.Row);
                        errorLog.Add("Invalid data:" + ex.ReadingContext.RawRecord);
                    }
                }
            }

            return new CsvImportResult {Entries = entries, ErrorLog = errorLog};
        }
    }
}
