namespace SFA.DAS.RoATPService.Application.Interfaces
{
    using System;
    using System.Threading.Tasks;

    public interface IDuplicateCheckRepository
    {
        Task<bool> DuplicateUKPRNExists(Guid organisationId, long ukprn);
        Task<bool> DuplicateCompanyNumberExists(Guid organisationId, string companyNumber);
        Task<bool> DuplicateCharityNumberExists(Guid organisationId, string charityNumber);
    }
}
