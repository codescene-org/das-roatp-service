using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Models
{
    public class OrganisationCategoryModel: TestModel
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Status { get; set; }
    }
}
