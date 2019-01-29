namespace SFA.DAS.RoATPService.Domain
{
    using System;

    public class AuditLogEntry
    {
        public Guid OrganisationId { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string FieldChanged { get; set; }
        public string PreviousValue { get; set; }
        public string NewValue { get; set; }
    }
}
