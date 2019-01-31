namespace SFA.DAS.RoATPService.Application.Validators
{
    using System;

    public class OrganisationSearchValidator : IOrganisationSearchValidator
    {
        public bool IsValidSearchTerm(string searchTerm)
        {
            if (String.IsNullOrWhiteSpace(searchTerm))
            {
                return false;
            }

            if (searchTerm.Trim().Length < 2)
            {
                return false;
            }

            return true;
        }

        public bool IsValidUKPRN(string candidateUkPrn)
        {
            long ukPrn;

            bool isValid = long.TryParse(candidateUkPrn, out ukPrn);

            if (!isValid)
            {
                return false;
            }

            return ukPrn >= 10000000 && ukPrn <= 99999999;
        }
    }
}
