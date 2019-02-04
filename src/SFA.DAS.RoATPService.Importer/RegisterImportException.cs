namespace SFA.DAS.RoATPService.Importer
{
    using System;

    public class RegisterImportException : Exception
    {
        public long UKPRN { get; set; }
        public string ImportErrorMessage { get; set; }
    }
}
