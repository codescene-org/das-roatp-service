namespace SFA.DAS.RoATPService.Api.Types.Models
{
    using System;
    using Domain;
    using MediatR;

    public class GetOrganisationRequest: IRequest<Organisation>
    {
        public Guid OrganisationId { get; set; }
    }
}
