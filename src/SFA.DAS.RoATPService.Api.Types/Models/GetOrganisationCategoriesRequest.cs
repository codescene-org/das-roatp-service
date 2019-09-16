using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Api.Types.Models
{
    public class GetOrganisationCategoriesRequest : IRequest<IEnumerable<OrganisationCategory>>
    {
        public int ProviderTypeId { get; set; }
    }
}
