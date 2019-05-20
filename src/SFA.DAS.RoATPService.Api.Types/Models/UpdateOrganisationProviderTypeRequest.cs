namespace SFA.DAS.RoATPService.Api.Types.Models
{
    using MediatR;
    using System;

    public class UpdateOrganisationProviderTypeRequest : IRequest<bool>
    {
        public Guid OrganisationId { get; set; }
        public int ProviderTypeId { get; set; }
        public int OrganisationTypeId { get; set; }
        public string UpdatedBy { get; set; }
    }
}
