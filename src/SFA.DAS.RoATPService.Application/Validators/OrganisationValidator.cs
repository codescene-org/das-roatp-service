using System.Xml.XPath;
using SFA.DAS.RoATPService.Application.Interfaces;

namespace SFA.DAS.RoATPService.Application.Validators
{
    using SFA.DAS.RoATPService.Api.Types.Models;
    using SFA.DAS.RoATPService.Domain;
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Interfaces;
    using System.Threading.Tasks;

    public class OrganisationValidator : IOrganisationValidator
    {
        private const string CompaniesHouseNumberRegex = "[A-Za-z0-9]{2}[0-9]{6}";
        private const string CharityNumberInvalidCharactersRegex = "[^a-zA-Z0-9\\-]";
        private readonly IDuplicateCheckRepository _duplicateCheckRepository;
        private readonly ILookupDataRepository _lookupRepository;

        public OrganisationValidator(IDuplicateCheckRepository duplicateCheckRepository, ILookupDataRepository lookupRepository)
        {
            _duplicateCheckRepository = duplicateCheckRepository;
            _lookupRepository = lookupRepository;
        }


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

            return IsValidProviderTypeId(providerType.Id);
        }

        public bool IsValidProviderTypeId(int providerTypeId)
        {
            return (providerTypeId >= 1 && providerTypeId <= 3);
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
            if (String.IsNullOrWhiteSpace(tradingName))
            {
                return true;
            }

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

            return IsValidStatusId(status.Id);
        }

        public bool IsValidStatusId(int statusId)
        {
            return (statusId >= 0 && statusId <= 2);
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

            return IsValidOrganisationTypeId(organisationType.Id);
        }

        public bool IsValidOrganisationTypeId(int organisationTypeId)
        {
            return organisationTypeId >= 0 && organisationTypeId <= 20;
        }

        public bool IsValidOrganiationTypeIdForOrganisationProvider(int organisationTypeId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsValidOrganisationTypeIdForProvider(int organisationTypeId, int providerTypeId)
        {
            if (!IsValidOrganisationTypeId(organisationTypeId))
            {
                return false;
            }

            var organisationTypes = await _lookupRepository.GetOrganisationTypes(providerTypeId);

            var organisationType = organisationTypes.FirstOrDefault(x => x.Id == organisationTypeId);

            return (organisationType != null);
        }

        public string DuplicateUkprnInAnotherOrganisation(long ukprn, Guid organisationId)
        {
            var response = _duplicateCheckRepository.DuplicateUKPRNExists(organisationId, ukprn).Result;
            return response.DuplicateOrganisationName;
        }

        DuplicateCheckResponse IOrganisationValidator.DuplicateUkprnInAnotherOrganisation(long ukprn, Guid organisationId)
        {
            return _duplicateCheckRepository.DuplicateUKPRNExists(organisationId, ukprn).Result;
        }

        public bool IsValidOrganisationTypeIdForOrganisationProvider(int organisationTypeId, Guid organisationId)
        {
            return _lookupRepository.IsOrganisationTypeValidForOrganisation(organisationTypeId, organisationId).Result;
        }
    }
}
