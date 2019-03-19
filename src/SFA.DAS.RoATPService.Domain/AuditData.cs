namespace SFA.DAS.RoATPService.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class AuditData
    {
        public Guid OrganisationId { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<AuditLogEntry> FieldChanges { get; set; }
    }
}
