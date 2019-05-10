namespace SFA.DAS.RoATPService.Application.Validators
{
    using Api.Types.Models;
    using Domain;
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
        private readonly IOrganisationRepository _organisationRepository;

        public OrganisationValidator(IDuplicateCheckRepository duplicateCheckRepository, ILookupDataRepository lookupRepository, IOrganisationRepository organisationRepository)
        {
            _duplicateCheckRepository = duplicateCheckRepository;
            _lookupRepository = lookupRepository;
            _organisationRepository = organisationRepository;
        }


        public bool IsValidOrganisationId(Guid organisationId)
        {
            return organisationId != Guid.Empty;
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
            var providerTypes = _lookupRepository.GetProviderTypes().Result;
            return providerTypes.Any(x => x.Id == providerTypeId);
        }

        public bool IsValidUKPRN(long ukPrn)
        {
            return ukPrn >= 10000000 && ukPrn <= 99999999;
        }

        public bool IsValidLegalName(string legalName)
        {
            if (string.IsNullOrWhiteSpace(legalName))
            {
                return false;
            }

            return legalName.Length <= 200; 
        }

        public bool IsValidTradingName(string tradingName)
        {
            if (string.IsNullOrWhiteSpace(tradingName))
            {
                return true;
            }

            return tradingName.Length <= 200;
        }

        public bool IsValidStatusDate(DateTime statusDate)
        {
            return statusDate > DateTime.MinValue;
        }

        public bool IsValidStatus(OrganisationStatus status)
        {
            return status != null && IsValidStatusId(status.Id);
        }

        public bool IsValidStatusId(int statusId)
        {
            var organisationStatuses = _lookupRepository.GetOrganisationStatuses().Result;
            return organisationStatuses.Any(x => x.Id == statusId);
        }

        public bool IsValidCompanyNumber(string companyNumber)
        {
            if (string.IsNullOrWhiteSpace(companyNumber))
            {
                return true;
            }

            if (companyNumber.Length != 8 ||
                !Regex.IsMatch(companyNumber, CompaniesHouseNumberRegex))
            {
                return false;
            }

            return true;
        }

        public bool IsValidCharityNumber(string charityNumber)
        {
            if (string.IsNullOrWhiteSpace(charityNumber))
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
            var organisationTypes = _lookupRepository.GetOrganisationTypes().Result;
            return organisationTypes.Any(x => x.Id == organisationTypeId);
        }

        public bool IsValidOrganisationStatusIdForOrganisation(int organisationStatusId, Guid organisationId)
        {
             var providerTypeId = _organisationRepository.GetProviderType(organisationId).Result;
     
            if (!IsValidStatusId(organisationStatusId))
            {
                return false;
            }

            var organisationStatuses = _lookupRepository.GetOrganisationStatusesForProviderTypeId(providerTypeId).Result;
            var organisationStatus = organisationStatuses.FirstOrDefault(x => x.Id == organisationStatusId);
            return organisationStatus != null;
        }

        public async Task<bool> IsValidOrganisationTypeIdForProvider(int organisationTypeId, int providerTypeId)
        {
            if (!IsValidOrganisationTypeId(organisationTypeId))
            {
                return false;
            }

            var organisationTypes = await _lookupRepository.GetOrganisationTypesForProviderTypeId(providerTypeId);

            var organisationType = organisationTypes.FirstOrDefault(x => x.Id == organisationTypeId);

            return organisationType != null;
        }

        public string DuplicateUkprnInAnotherOrganisation(long ukprn, Guid organisationId)
        {
            var response = _duplicateCheckRepository.DuplicateUKPRNExists(organisationId, ukprn).Result;
            return response.DuplicateOrganisationName;
        }

        public DuplicateCheckResponse DuplicateCompanyNumberInAnotherOrganisation(string companyNumber, Guid organisationId)
        {
            return _duplicateCheckRepository.DuplicateCompanyNumberExists(organisationId, companyNumber).Result;
        }

        DuplicateCheckResponse IOrganisationValidator.DuplicateUkprnInAnotherOrganisation(long ukprn, Guid organisationId)
        {
            return _duplicateCheckRepository.DuplicateUKPRNExists(organisationId, ukprn).Result;
        }

        public bool IsValidOrganisationTypeIdForOrganisation(int organisationTypeId, Guid organisationId)
        {
            var providerTypeId = _organisationRepository.GetProviderType(organisationId).Result;
            return IsValidOrganisationTypeIdForProvider(organisationTypeId, providerTypeId).Result;
        }
    }
}
