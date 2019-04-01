namespace SFA.DAS.RoATPService.Application.Interfaces
{
    using System;
    using System.Threading.Tasks;

    public interface IUpdateOrganisationRepository
    {
        Task<string> GetLegalName(Guid organisationId);
        Task<bool> UpdateLegalName(Guid organisationId, string legalName, string updatedBy);
        Task<bool> UpdateUkprn(Guid organisationId, long ukprn, string updatedBy);
        Task<long> GetUkprn(Guid organisationId);
    }
}
