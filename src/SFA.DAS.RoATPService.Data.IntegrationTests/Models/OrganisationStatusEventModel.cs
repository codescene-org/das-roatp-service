using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Models
{
    public class OrganisationStatusEventModel : TestModel
    {
        public long Id { get; set; }
        public long ProviderId { get; set; }
        public int OrganisationStatusId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
