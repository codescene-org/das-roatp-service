namespace SFA.DAS.RoATPService.Domain
{
    using System.ComponentModel;

    public class OrganisationType : BaseEntity
    {
        [ExcludeFromAuditLog]
        public int Id { get; set; }
        [DisplayName("Organisation Type")]
        public string Type { get; set; }
        public string Description { get; set; }
    }
}
