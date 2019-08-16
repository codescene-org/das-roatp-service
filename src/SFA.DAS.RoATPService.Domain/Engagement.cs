using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.RoATPService.Domain
{
    public class Engagement
    {
        public long Id { get; set; }
        public long ProviderId { get; set; }
        public string Event { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
