namespace SFA.DAS.RoATPService.Application.Validators
{
    using SFA.DAS.RoATPService.Domain;
    using System;

    public interface IOrganisationValidator
    {
        bool IsValidOrganisationId(Guid organisationId);
        bool IsValidProviderType(ProviderType providerType);
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

        string DuplicateUkprnInAnotherOrganisation(long ukprn, Guid organisationId);
    }
}
