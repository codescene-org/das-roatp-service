namespace SFA.DAS.RoATPService.Domain
{ 
    public class ProviderTypeOrganisationStatus : BaseEntity
    {
        public int Id { get; set; }
        public int ProviderTypeId { get; set; }
        public int OrganisationStatusId { get; set; }
    }
}
