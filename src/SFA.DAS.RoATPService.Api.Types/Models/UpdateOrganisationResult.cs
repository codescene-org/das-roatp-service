namespace SFA.DAS.RoATPService.Api.Types.Models
{
    using Domain;

    public class UpdateOrganisationResult
    {
        public bool Success { get; set; }
        public Organisation OriginalOrganisation { get; set; }
        public Organisation UpdatedOrganisation { get; set; }
    }
}
