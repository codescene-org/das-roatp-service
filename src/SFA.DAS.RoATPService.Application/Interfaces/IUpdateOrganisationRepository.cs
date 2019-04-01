namespace SFA.DAS.RoATPService.Application.Interfaces
{
    using System;
    using System.Threading.Tasks;

    public interface IUpdateOrganisationRepository
    {
        Task<string> GetLegalName(Guid organisationId);
        Task<bool> UpdateLegalName(Guid organisationId, string legalName, string updatedBy);
        Task<string> GetTradingName(Guid organisationId);
        Task<bool> UpdateTradingName(Guid organisationId, string tradingName, string updatedBy);
    }
}
