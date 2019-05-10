using SFA.DAS.RoATPService.Api.Types.Models;

namespace SFA.DAS.RoATPService.Application.Validators
{
    using SFA.DAS.RoATPService.Domain;
    using System;
    using System.Threading.Tasks;

    public interface IOrganisationValidator
    {
        bool IsValidOrganisationId(Guid organisationId);
        bool IsValidProviderType(ProviderType providerType);
        bool IsValidProviderTypeId(int providerTypeId);
        bool IsValidUKPRN(long ukPrn);
        bool IsValidLegalName(string legalName);
        bool IsValidTradingName(string tradingName);
        bool IsValidStatusDate(DateTime statusDate);
        bool IsValidStatus(OrganisationStatus status);
        bool IsValidStatusId(int statusId);
        bool IsValidCompanyNumber(string companyNumber);
        bool IsValidCharityNumber(string charityNumber);
        bool IsValidOrganisationType(OrganisationType organisationType);
        bool IsValidOrganisationTypeId(int organisationTypeId);
        bool IsValidOrganisationTypeIdForOrganisation(int organisationTypeId, Guid organisationId);
        bool IsValidOrganisationStatusIdForOrganisation(int organisationStatusId, Guid organisationId);
        Task<bool> IsValidOrganisationTypeIdForProvider(int organisationTypeId, int providerTypeId);
        DuplicateCheckResponse DuplicateUkprnInAnotherOrganisation(long ukprn, Guid organisationId);
        DuplicateCheckResponse DuplicateCompanyNumberInAnotherOrganisation(string companyNumber, Guid organisationId);

    }
}
