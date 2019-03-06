﻿namespace SFA.DAS.RoATPService.Application.Validators
{
    using SFA.DAS.RoATPService.Domain;
    using System;
    using System.Text.RegularExpressions;

    public class OrganisationValidator : IOrganisationValidator
    {
        private const string CompaniesHouseNumberRegex = "[A-Za-z0-9]{2}[0-9]{6}";
        private const string CharityNumberInvalidCharactersRegex = "[^a-zA-Z0-9\\-]";

        public bool IsValidOrganisationId(Guid organisationId)
        {
            if (organisationId == null || organisationId == Guid.Empty)
            {
                return false;
            }

            return true;
        }

        public bool IsValidProviderType(ProviderType providerType)
        {
            if (providerType == null)
            {
                return false;
            }

            return (providerType.Id >= 1 && providerType.Id <= 3);
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

            return legalName.Length <= 200; 
        }

        public bool IsValidTradingName(string tradingName)
        {
            return tradingName.Length <= 200;
        }

        public bool IsValidStatusDate(DateTime statusDate)
        {
            return (statusDate > DateTime.MinValue);
        }

        public bool IsValidStatus(OrganisationStatus status)
        {
            if (status == null)
            {
                return false;
            }

            return (status.Id >= 0 && status.Id <= 2);
        }

        public bool IsValidCompanyNumber(string companyNumber)
        {
            if (String.IsNullOrWhiteSpace(companyNumber))
            {
                return true;
            }

            if ((companyNumber.Length != 8) ||
                !Regex.IsMatch(companyNumber, CompaniesHouseNumberRegex))
            {
                return false;
            }

            return true;
        }

        public bool IsValidCharityNumber(string charityNumber)
        {
            if (String.IsNullOrWhiteSpace(charityNumber))
            {
                return true;
            }

            if (Regex.IsMatch(charityNumber, CharityNumberInvalidCharactersRegex))
            {
                return false;
            }

            return true;
        }

        public bool IsValidOrganisationType(OrganisationType organisationType)
        {
            if (organisationType == null)
            {
                return false;
            }

            if (organisationType.Id < 0 || organisationType.Id > 6)
            {
                return false;
            }

            return true;
        }
    }
}
