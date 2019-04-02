namespace SFA.DAS.RoATPService.Domain
{
    public class RemovedReason : BaseEntity
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public string Description { get; set; }
    }
}
