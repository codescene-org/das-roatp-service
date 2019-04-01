using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Models
{
    public class AuditModel:TestModel
    {
        public int Id { get; set; }
        public Guid OrganisationId { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public AuditData AuditData { get; set; }
    }
}
