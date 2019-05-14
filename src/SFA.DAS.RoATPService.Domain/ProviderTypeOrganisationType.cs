namespace SFA.DAS.RoATPService.Domain
{
    public class ProviderTypeOrganisationType:BaseEntity
    {
        public int Id { get; set; }
        public int ProviderTypeId { get; set; }
        public int OrganisationTypeId { get; set; }
    }
}
