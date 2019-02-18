namespace SFA.DAS.RoATPService.Application.Validators
{
    using System;

    public class OrganisationValidator : IOrganisationValidator
    {
        public bool IsValidOrganisationId(Guid organisationId)
        {
            if (organisationId == null || organisationId == Guid.Empty)
            {
                return false;
            }

            return true;
        }

        public bool IsValidApplicationRouteId(int applicationRouteId)
        {
            return (applicationRouteId > 0);
        }

        public bool IsValidUKPRN(long ukPrn)
        {
            return (ukPrn >= 10000000 && ukPrn <= 99999999);
        }

        public bool IsValidLegalName(string legalName)
        {
            if (String.IsNullOrWhiteSpace(legalName))
            {
                return false;
            }

            return true;
        }

        public bool IsValidStatusDate(DateTime statusDate)
        {
            return (statusDate > DateTime.MinValue);
        }

        public bool IsValidStatus(string status)
        {
            return !String.IsNullOrWhiteSpace(status);
        }
    }
}
