namespace SFA.DAS.RoATPService.Domain
{
    using System.ComponentModel;

    public class RemovedReason : BaseEntity
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public string Description { get; set; }
    }
}
