using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.RoATPService.Domain
{
    public class OrganisationCategory : BaseEntity
    {
        public int Id { get; set; }
        public string Category { get; set; }
    }
}
