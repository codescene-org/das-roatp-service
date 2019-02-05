namespace SFA.DAS.RoATPService.Application.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Domain;

    public interface IOrganisationRepository
    {
        Task<bool> CreateOrganisation(Organisation organisation, string username);
        Task<bool> UpdateOrganisation(Organisation organisation, string username);
        Task<Organisation> GetOrganisation(Guid organisationId);
    }
}
