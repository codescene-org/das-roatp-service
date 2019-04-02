
namespace SFA.DAS.RoATPService.Application.Interfaces
{
    using Domain;
    using System.Threading.Tasks;

    public interface IOrganisationStatusRepository
    {
        Task<OrganisationStatus> GetOrganisationStatus(int statusId);
    }
}
