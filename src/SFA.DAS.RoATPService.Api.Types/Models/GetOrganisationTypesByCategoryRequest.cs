using System.Collections.Generic;
using MediatR;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Api.Types.Models
{
    public class GetOrganisationTypesByCategoryRequest : IRequest<IEnumerable<OrganisationType>>
    {
        public int ProviderTypeId { get; set; }
        public int CategoryId { get; set; }
    }
}