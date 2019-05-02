namespace SFA.DAS.RoATPService.Api.Types.Models
{
    using System;

    public class DuplicateCheckResponse
    {
        public bool DuplicateFound { get; set; }
        public string DuplicateOrganisationName { get; set; }
        public Guid DuplicateOrganisationId { get; set; }
    }
}
