namespace SFA.DAS.RoATPService.Application.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using Domain;
    using SFA.DAS.RoATPService.Application.Commands;

    public interface IUpdateOrganisationRepository
    {
        Task<Guid?> CreateOrganisation(CreateOrganisationCommand command);

      
        Task<bool> UpdateLegalName(Guid organisationId, string legalName, string updatedBy);

        Task<bool> GetFinancialTrackRecord(Guid organisationId);
        Task<bool> UpdateFinancialTrackRecord(Guid organisationId, bool financialTrackRecord, string updatedBy);

        Task<long> GetUkprn(Guid organisationId);
        Task<bool> UpdateUkprn(Guid organisationId, long ukprn, string updatedBy);

        Task<bool> GetParentCompanyGuarantee(Guid organisationId);
        Task<bool> UpdateParentCompanyGuarantee(Guid organisationId, bool parentCompanyGuarantee, string updatedBy);

        Task<string> GetTradingName(Guid organisationId);
        Task<bool> UpdateTradingName(Guid organisationId, string tradingName, string updatedBy);

        Task<int> GetStatus(Guid organisationId);
        Task<RemovedReason> GetRemovedReason(Guid organisationId);
        Task<bool> UpdateStatus(Guid organisationId, int organisationStatusId, string updatedBy);
        Task<RemovedReason> UpdateStatusWithRemovedReason(Guid organisationId, int organisationStatusId, int removedReasonId, string updatedBy);

        Task<int> GetOrganisationType(Guid organisationId);
        Task<bool> UpdateOrganisationType(Guid organisationId, int organisationTypeId, string updatedBy);

         Task<bool> UpdateStartDate(Guid organisationId, DateTime startDate);
        Task<DateTime?> GetStartDate(Guid organisationId);

        Task<int> GetProviderType(Guid organisationId);
        Task<bool> UpdateProviderTypeAndOrganisationType(Guid organisationId, int providerTypeId, int organisationTypeId, string updatedBy);

        Task<bool> WriteFieldChangesToAuditLog(AuditData auditFieldChanges);
    }
}
