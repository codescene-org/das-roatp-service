using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Models
{
    public class AuditModel:TestModel
    {
        public int Id { get; set; }
        public Guid OrganisationId { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string FieldChanged { get; set; }
        public string PreviousValue { get; set; }
        public string NewValue { get; set; }
    }
}
