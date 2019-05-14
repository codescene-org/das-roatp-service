using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace SFA.DAS.RoATPService.Api.Types.Models
{

    public class UpdateOrganisationCompanyNumberRequest : IRequest<bool>
    {
        public Guid OrganisationId { get; set; }
        public string CompanyNumber { get; set; }
        public string UpdatedBy { get; set; }
    }
}
