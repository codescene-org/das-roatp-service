namespace SFA.DAS.RoATPService.Api.Types.Models
{
    using System;
    using MediatR;

    public class UpdateOrganisationLegalNameRequest : IRequest<bool>
    {
        public Guid OrganisationId { get; set; }
        public string LegalName { get; set; }
        public string UpdatedBy { get; set; }
    }
}
