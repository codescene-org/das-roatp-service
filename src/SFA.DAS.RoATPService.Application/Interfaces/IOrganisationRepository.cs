namespace SFA.DAS.RoATPService.Application.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using Domain;
    using SFA.DAS.RoATPService.Application.Commands;

    public interface IOrganisationRepository
    {
        Task<Guid?> CreateOrganisation(CreateOrganisationCommand command);
        [Obsolete("Use operations in IUpdateOrganisationRepository instead")]
        Task<bool> UpdateOrganisation(Organisation organisation, string username);
        Task<Organisation> GetOrganisation(Guid organisationId);
        Task<OrganisationReapplyStatus> GetOrganisationReapplyStatus(Guid organisationId);
    }
}
