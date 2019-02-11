namespace SFA.DAS.RoATPService.Domain
{
    using System;

    public class Organisation : BaseEntity
    {
        public Guid Id { get; set; }
        public ProviderType ProviderType { get; set; }
        public OrganisationType OrganisationType{ get; set; }
        public long UKPRN { get; set; }
        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public OrganisationStatus OrganisationStatus { get; set; }
        public DateTime StatusDate { get; set; }
        public OrganisationData OrganisationData { get; set; }
    }
}
