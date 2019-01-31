namespace SFA.DAS.RoATPService.Application.Exceptions
{
    using System;

    public sealed class UnauthorisedException : Exception
    {
        public UnauthorisedException() : base("") { }
        public UnauthorisedException(string message) : base(message) { }
    }
}
