namespace SFA.DAS.RoATPService.Importer
{
    using System.Collections.Generic;

    public class CsvImportResult
    {
        public List<RegisterEntry> Entries { get; set; }
        public List<string> ErrorLog { get; set; } 
    }
}
