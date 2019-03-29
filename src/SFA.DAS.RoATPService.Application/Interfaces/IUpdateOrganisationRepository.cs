namespace SFA.DAS.RoATPService.Application.Interfaces
{
    using System;
    using System.Threading.Tasks;

    public interface IUpdateOrganisationRepository
    {
        Task<string> GetLegalName(Guid organisationId);
        Task<bool> UpdateLegalName(Guid organisationId, string legalName, string updatedBy);
        Task<bool> GetFinancialTrackRecord(Guid organisationId);
        Task<bool> UpdateFinancialTrackRecord(Guid organisationId, bool financialTrackRecord, string updatedBy);
    }
}
