namespace SFA.DAS.RoATPService.Application.Validators
{
    using System;

    public interface IOrganisationValidator
    {
        bool IsValidOrganisationId(Guid organisationId);
        bool IsValidApplicationRouteId(int applicationRouteId);
        bool IsValidUKPRN(long ukPrn);
        bool IsValidLegalName(string legalName);
        bool IsValidStatusDate(DateTime statusDate);
        bool IsValidStatus(string status);
    }
}
