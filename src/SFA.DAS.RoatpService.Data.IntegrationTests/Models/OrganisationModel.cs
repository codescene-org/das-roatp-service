using System;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Models
{
    public class OrganisationModel : TestModel
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public int StatusId { get; set; }
        public int ProviderTypeId { get; set; }
        public int? OrganisationTypeId { get; set; }
        public long UKPRN { get; set; }
        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public DateTime StatusDate { get; set; }
        public string OrganisationData { get; set; }
    }
}
