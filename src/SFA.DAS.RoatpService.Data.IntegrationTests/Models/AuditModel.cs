namespace SFA.DAS.RoatpService.Data.IntegrationTests.Models
{
    using System;
    using SFA.DAS.RoATPService.Domain;

    public class AuditModel : TestModel
    {
        public int Id { get; set; }
        public Guid OrganisationId { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public AuditData AuditData { get; set; }
    }
}
