namespace SFA.DAS.RoATPService.Api.Types.Models
{
    using System;
    using MediatR;
    public class UpdateOrganisationCharityNumberRequest : IRequest<bool>
    {
        public Guid OrganisationId { get; set; }
        public string CharityNumber { get; set; }
        public string UpdatedBy { get; set; }
    }
}
