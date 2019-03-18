namespace SFA.DAS.RoATPService.Api.Types.Models
{
    using System;
    using MediatR;

    public class DuplicateCharityNumberCheckRequest : IRequest<DuplicateCheckResponse>
    {
        public Guid OrganisationId { get; set; }
        public string CharityNumber { get; set; }
    }
}
