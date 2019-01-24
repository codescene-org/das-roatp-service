namespace SFA.DAS.RoATPService.Application.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain;

    public interface IOrganisationSearchRepository
    {
        Task<IEnumerable<Organisation>> OrganisationSearchByUkPrn(string ukPrn);
        Task<IEnumerable<Organisation>> OrganisationSearchByName(string organisationName);
    }
}
