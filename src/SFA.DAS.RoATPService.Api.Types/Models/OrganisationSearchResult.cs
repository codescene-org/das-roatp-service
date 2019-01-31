namespace SFA.DAS.RoATPService.Api.Types.Models
{
    using System;

    public class OrganisationSearchResult
    {
        public Guid Id { get; set; }
        public long UKPRN { get; set; }
        public string LegalName { get; set; }
        public string TradingName { get; set; }
    }
}
