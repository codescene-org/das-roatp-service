namespace SFA.DAS.RoATPService.Api.Types.Models
{
    using System.Collections.Generic;
    using Domain;
    using MediatR;

    public class GetOrganisationTypesRequest : IRequest<IEnumerable<OrganisationType>>
    {
        public int ProviderTypeId { get; set; }
    }
}
