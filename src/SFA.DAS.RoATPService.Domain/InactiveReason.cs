namespace SFA.DAS.RoATPService.Domain
{
    public class InactiveReason : BaseEntity
    { 
        public int Id { get; set; }
        public string EndReason { get; set; }
        public string Description { get; set; }
    }
}
