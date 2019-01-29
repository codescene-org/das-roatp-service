namespace SFA.DAS.RoATPService.Domain
{
    using System;

    public class BaseEntity
    {
        [ExcludeFromAuditLog]
        public string CreatedBy { get; set; }
        [ExcludeFromAuditLog]
        public DateTime CreatedAt { get; set; }
        [ExcludeFromAuditLog]
        public string UpdatedBy { get; set; }
        [ExcludeFromAuditLog]
        public DateTime? UpdatedAt { get; set; }
        public string Status { get; set; }
    }
}
