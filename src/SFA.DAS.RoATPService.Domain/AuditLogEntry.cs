namespace SFA.DAS.RoATPService.Domain
{
    using System;

    public class AuditLogEntry
    {
        public string FieldChanged { get; set; }
        public string PreviousValue { get; set; }
        public string NewValue { get; set; }
    }
}
