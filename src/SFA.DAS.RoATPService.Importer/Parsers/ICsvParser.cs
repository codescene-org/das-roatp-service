namespace SFA.DAS.RoATPService.Importer.Parsers
{
    using System.IO;
    using SFA.DAS.RoATPService.Importer.Models;

    public interface ICsvParser
    {
        CsvImportResult ParseCsvFile(StreamReader csvFileReader);
    }
}
