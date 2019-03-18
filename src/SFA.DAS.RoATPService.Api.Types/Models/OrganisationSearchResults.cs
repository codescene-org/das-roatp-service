namespace SFA.DAS.RoATPService.Api.Types.Models
{
    using System.Collections.Generic;
    using SFA.DAS.RoATPService.Domain;

    public class OrganisationSearchResults
    {
        public IEnumerable<Organisation> SearchResults { get; set; }

        public int TotalCount { get; set; }
    }
}
