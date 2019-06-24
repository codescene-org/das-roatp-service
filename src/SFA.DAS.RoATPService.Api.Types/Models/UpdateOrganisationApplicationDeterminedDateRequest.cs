using System;
using MediatR;

namespace SFA.DAS.RoATPService.Api.Types.Models
{
    public class UpdateOrganisationApplicationDeterminedDateRequest : IRequest<bool>
    { 
        public DateTime ApplicationDeterminedDate { get; set; }

        public Guid OrganisationId { get; set; }
        public string LegalName { get; set; }
        public string UpdatedBy { get; set; }
    }
}
