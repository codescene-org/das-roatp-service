namespace SFA.DAS.RoATPService.Domain
{
    using System;

    public class BaseEntity
    {
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
    }
}
