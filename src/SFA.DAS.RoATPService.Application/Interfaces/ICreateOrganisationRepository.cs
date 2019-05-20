using System;
using System.Threading.Tasks;
using SFA.DAS.RoATPService.Application.Commands;

namespace SFA.DAS.RoATPService.Application.Interfaces
{
    public interface ICreateOrganisationRepository
    {
        Task<Guid?> CreateOrganisation(CreateOrganisationCommand command);
    }
}
