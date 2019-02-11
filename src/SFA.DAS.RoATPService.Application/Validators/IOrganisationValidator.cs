namespace SFA.DAS.RoATPService.Application.Validators
{
    using System;

    public interface IOrganisationValidator
    {
        bool IsValidOrganisationId(Guid organisationId);
        bool IsValidProviderTypeId(int providerTypeId);
        bool IsValidUKPRN(long ukPrn);
        bool IsValidLegalName(string legalName);
        bool IsValidStatusDate(DateTime statusDate);
        bool IsValidStatus(int status);
    }
}
