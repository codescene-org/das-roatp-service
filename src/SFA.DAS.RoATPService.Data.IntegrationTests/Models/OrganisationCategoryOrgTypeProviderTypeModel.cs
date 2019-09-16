using System;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Models
{
    public class OrganisationCategoryOrgTypeProviderTypeModel:TestModel
    {
        public int Id { get; set; }
        public int OrganisationTypeId { get; set; }
        public int OrganisationCategoryId { get; set; }
        public int ProviderTypeId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Status { get; set; }
    }
}
