namespace SFA.DAS.RoATPService.Domain
{
    using System;
    using System.ComponentModel;

    public class Organisation : BaseEntity
    {
        [ExcludeFromAuditLog]
        public Guid Id { get; set; }
        public ApplicationRoute ApplicationRoute { get; set; }
        public OrganisationType OrganisationType{ get; set; }
        public long UKPRN { get; set; }
        [DisplayName("Legal Name")]
        public string LegalName { get; set; }
        [DisplayName("Trading Name")]
        public string TradingName { get; set; }
        [DisplayName("Status Effective From Date")]
        public DateTime StatusDate { get; set; }
        public OrganisationData OrganisationData { get; set; }
    }
}
