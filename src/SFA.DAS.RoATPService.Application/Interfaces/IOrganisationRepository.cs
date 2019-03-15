using SFA.DAS.RoATPService.Application.commands;

namespace SFA.DAS.RoATPService.Application.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using Domain;

    public interface IOrganisationRepository
    {
        Task<Guid?> CreateOrganisation(CreateOrganisationCommand command);
        Task<bool> UpdateOrganisation(Organisation organisation, string username);
        Task<Organisation> GetOrganisation(Guid organisationId);
    }
}
