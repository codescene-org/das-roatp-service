namespace SFA.DAS.RoATPService.Api.Types.Models
{
    using System;
    using MediatR;
    using SFA.DAS.RoATPService.Domain;

    public class GetOrganisationReapplyStatusRequest : IRequest<OrganisationReapplyStatus>
    {
        public Guid OrganisationId { get; set; }
    }
}
