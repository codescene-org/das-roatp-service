namespace SFA.DAS.RoATPService.Api.Types.Models
{
    using System.Collections.Generic;
    using Domain;
    using MediatR;

    public class OrganisationSearchRequest  : IRequest<IEnumerable<Organisation>>
    {
        public string SearchTerm { get; set; }
    }
}
