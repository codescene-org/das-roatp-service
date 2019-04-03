namespace SFA.DAS.RoatpService.Data.IntegrationTests.Models
{
    using System;

    public class RemovedReasonModel : TestModel
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string Status { get; set; }
    }
}
