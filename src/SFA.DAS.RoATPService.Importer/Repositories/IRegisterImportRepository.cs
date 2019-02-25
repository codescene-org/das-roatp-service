namespace SFA.DAS.RoATPService.Importer
{
    using SFA.DAS.RoATPService.Api.Types.Models;
    using System.Threading.Tasks;

    public interface IRegisterImportRepository
    {
        Task<RegisterImportResultsResponse> ImportRegisterData(RegisterImportRequest importRequest);
    }
}
