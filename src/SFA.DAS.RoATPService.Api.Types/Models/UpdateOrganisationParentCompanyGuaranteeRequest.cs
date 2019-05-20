using System;
using MediatR;

namespace SFA.DAS.RoATPService.Api.Types.Models
{
    public class UpdateOrganisationParentCompanyGuaranteeRequest : IRequest<bool>
    {
        public Guid OrganisationId { get; set; }
        public bool ParentCompanyGuarantee { get; set; }
        public string UpdatedBy { get; set; }
    }


}
