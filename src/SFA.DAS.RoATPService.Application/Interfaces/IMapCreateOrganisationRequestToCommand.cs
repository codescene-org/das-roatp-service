using SFA.DAS.RoATPService.Api.Types.Models;
using SFA.DAS.RoATPService.Application.Commands;

namespace SFA.DAS.RoATPService.Application.Interfaces
{
    public interface IMapCreateOrganisationRequestToCommand
    {
        CreateOrganisationCommand Map(CreateOrganisationRequest request);
    }
}
