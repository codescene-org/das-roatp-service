namespace SFA.DAS.RoATPService.Application.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using Domain;
    using System.Collections.Generic;

    public interface IOrganisationRepository
    {
        
        Task<Organisation> GetOrganisation(Guid organisationId);
        Task<OrganisationRegisterStatus> GetOrganisationRegisterStatus(string ukprn);
        Task<string> GetLegalName(Guid organisationId);
        Task<string> GetTradingName(Guid organisationId);
        Task<bool> GetFinancialTrackRecord(Guid organisationId);
        Task<long> GetUkprn(Guid organisationId);
        Task<string> GetCompanyNumber(Guid organisationId);
        Task<bool> GetParentCompanyGuarantee(Guid organisationId);
        Task<int> GetOrganisationStatus(Guid organisationId);
        Task<RemovedReason> GetRemovedReason(Guid organisationId);
        Task<int> GetOrganisationType(Guid organisationId);
        Task<DateTime?> GetStartDate(Guid organisationId);
        Task<int> GetProviderType(Guid organisationId);
        Task<string> GetCharityNumber(Guid organisationId);
        Task<DateTime?> GetApplicationDeterminedDate(Guid organisationId);
        Task<IEnumerable<Engagement>> GetEngagements();
    }
}
