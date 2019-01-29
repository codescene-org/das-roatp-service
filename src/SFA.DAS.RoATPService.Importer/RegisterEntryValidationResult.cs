namespace SFA.DAS.RoATPService.Importer
{
    using System.Collections.Generic;

    public class RegisterEntryValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> ValidationMessages { get; set; }
    }
}
