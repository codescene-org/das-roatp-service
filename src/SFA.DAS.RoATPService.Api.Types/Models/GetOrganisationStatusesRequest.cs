namespace SFA.DAS.RoATPService.Api.Types.Models
{
    using System.Collections.Generic;
    using Domain;
    using MediatR;
 
    public class GetOrganisationStatusesRequest : IRequest<IEnumerable<OrganisationStatus>>
    {
        public int? ProviderTypeId { get; set; }
    }
}
