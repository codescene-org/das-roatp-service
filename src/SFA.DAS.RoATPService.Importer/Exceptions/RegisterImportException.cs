namespace SFA.DAS.RoATPService.Importer.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    public class RegisterImportException : Exception
    {
        public long UKPRN { get; set; }
        public string ImportErrorMessage { get; set; }

        public RegisterImportException(string message) : base(message)
        {

        }

        public RegisterImportException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(
            serializationInfo, streamingContext)
        {

        }

        public RegisterImportException(string message, Exception baseException) : base(message, baseException)
        {

        }
    }
}
