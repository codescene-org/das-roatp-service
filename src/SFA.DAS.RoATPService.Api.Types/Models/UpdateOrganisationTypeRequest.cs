namespace SFA.DAS.RoATPService.Api.Types.Models
{
    using System;
    using MediatR;

    public class UpdateOrganisationTypeRequest : IRequest<bool>
    {
        public Guid OrganisationId { get; set; }
        public int OrganisationTypeId { get; set; }
        public string UpdatedBy { get; set; }
    }
}
