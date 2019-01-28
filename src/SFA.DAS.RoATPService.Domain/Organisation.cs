namespace SFA.DAS.RoATPService.Domain
{
    using System;

    public class Organisation : BaseEntity
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public int ApplicationRouteId { get; set; }
        public int OrganisationTypeId { get; set; }
        public long UKPRN { get; set; }
        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public DateTime RegisterStartDate { get; set; }
        public DateTime? RegisterEndDate { get; set; }
        public OrganisationData OrganisationData { get; set; }
    }
}
