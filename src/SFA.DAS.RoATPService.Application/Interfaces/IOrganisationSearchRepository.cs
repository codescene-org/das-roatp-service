namespace SFA.DAS.RoATPService.Application.Interfaces
{
    using System.Threading.Tasks;
    using SFA.DAS.RoATPService.Api.Types.Models;

    public interface IOrganisationSearchRepository
    {
        Task<OrganisationSearchResults> OrganisationSearchByUkPrn(string ukPrn);
        Task<OrganisationSearchResults> OrganisationSearchByName(string organisationName);
    }
}
