namespace SFA.DAS.RoATPService.Api.Types.Models
{
    using System.Collections.Generic;

    public class RegisterImportResultsResponse
    {
        public bool Success { get; set; }
        public List<string> ErrorMessages { get; set; }
        public int EntriesImported { get; set; }
        public long ElapsedTimeMs { get; set; }
    }

}
