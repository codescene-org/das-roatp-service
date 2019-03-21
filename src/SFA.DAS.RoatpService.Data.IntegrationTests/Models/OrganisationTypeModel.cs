using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Models
{
    public class OrganisationTypeModel:TestModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string Status { get; set; }
    }
}
