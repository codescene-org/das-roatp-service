using System;
using MediatR;

namespace SFA.DAS.RoATPService.Api.Types.Models
{
    public class UpdateOrganisationUkprnRequest : IRequest<bool>
    {
        public Guid OrganisationId { get; set; }
        public long Ukprn { get; set; }
        public string UpdatedBy { get; set; }
    }
}
