namespace SFA.DAS.RoATPService.Domain
{
    using System.ComponentModel;

    public class InactiveReason : BaseEntity
    {
        [ExcludeFromAuditLog]
        public int Id { get; set; }
        [DisplayName("Inactive Reason")]
        public string EndReason { get; set; }
        public string Description { get; set; }
    }
}
