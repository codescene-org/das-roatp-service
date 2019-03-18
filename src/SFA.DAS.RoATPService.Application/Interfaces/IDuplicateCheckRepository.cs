namespace SFA.DAS.RoATPService.Application.Interfaces
{
    using SFA.DAS.RoATPService.Api.Types.Models;
    using System;
    using System.Threading.Tasks;

    public interface IDuplicateCheckRepository
    {
        Task<DuplicateCheckResponse> DuplicateUKPRNExists(Guid organisationId, long ukprn);
        Task<DuplicateCheckResponse> DuplicateCompanyNumberExists(Guid organisationId, string companyNumber);
        Task<DuplicateCheckResponse> DuplicateCharityNumberExists(Guid organisationId, string charityNumber);
    }
}
