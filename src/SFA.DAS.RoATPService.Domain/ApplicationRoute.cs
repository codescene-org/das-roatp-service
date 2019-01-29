namespace SFA.DAS.RoATPService.Domain
{
    using System.ComponentModel;

    public class ApplicationRoute : BaseEntity
    {
        [ExcludeFromAuditLog]
        public int Id { get; set; }
        [DisplayName("Application Route")]
        public string Route { get; set; }
        public string Description { get; set; }
    }
}
