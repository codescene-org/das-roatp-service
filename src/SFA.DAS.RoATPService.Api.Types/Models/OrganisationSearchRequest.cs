namespace SFA.DAS.RoATPService.Api.Types.Models
{
    using MediatR;

    public class OrganisationSearchRequest  : IRequest<OrganisationSearchResults>
    {
        public string SearchTerm { get; set; }
    }
}
