namespace SFA.DAS.RoATPService.Application.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using Domain;

    public interface IUpdateOrganisationRepository
    {
        Task<string> GetLegalName(Guid organisationId);
        Task<bool> UpdateLegalName(Guid organisationId, string legalName, string updatedBy);
        Task<int> GetStatus(Guid organisationId);
        Task<RemovedReason> GetRemovedReason(Guid organisationId);
        Task<bool> UpdateStatus(Guid organisationId, int organisationStatusId, string updatedBy);
        Task<RemovedReason> UpdateStatusWithRemovedReason(Guid organisationId, int organisationStatusId, int removedReasonId, string updatedBy);
        Task<bool> UpdateStartDate(Guid organisationId, DateTime startDate);
    }
}
