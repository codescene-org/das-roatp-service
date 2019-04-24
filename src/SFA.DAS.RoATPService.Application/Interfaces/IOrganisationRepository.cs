namespace SFA.DAS.RoATPService.Application.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using Domain;
    using SFA.DAS.RoATPService.Application.Commands;

    public interface IOrganisationRepository
    {
        
        Task<Organisation> GetOrganisation(Guid organisationId);
        Task<string> GetLegalName(Guid organisationId);
    }
}
