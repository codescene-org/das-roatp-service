using System;
using AutoMapper;
using SFA.DAS.RoATPService.Application.Commands;


namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Exceptions;
    using Interfaces;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Validators;

    public class CreateOrganisationHandler : IRequestHandler<CreateOrganisationRequest, Guid?>
    {
        private readonly IUpdateOrganisationRepository _organisationRepository;
        private readonly ILogger<CreateOrganisationHandler> _logger;
        private readonly IOrganisationValidator _organisationValidator;
        private readonly IProviderTypeValidator _providerTypeValidator;
        private readonly IMapCreateOrganisationRequestToCommand _mapper;

        public CreateOrganisationHandler(IUpdateOrganisationRepository repository, ILogger<CreateOrganisationHandler> logger, 
                                         IOrganisationValidator organisationValidator, IProviderTypeValidator providerTypeValidator, 
                                         IMapCreateOrganisationRequestToCommand mapper)
        {
            _organisationRepository = repository;
            _logger = logger;
            _organisationValidator = organisationValidator;
            _providerTypeValidator = providerTypeValidator;
            _mapper = mapper;
        }

        public Task<Guid?> Handle(CreateOrganisationRequest request, CancellationToken cancellationToken)
        {
            if (!IsValidCreateOrganisation(request))
            {
                string invalidOrganisationError = $@"Invalid Organisation data";
                if (!_organisationValidator.IsValidLegalName(request.LegalName))
                    invalidOrganisationError = $"{invalidOrganisationError}: Invalid Legal Name [{request.LegalName}]";

                if (!_organisationValidator.IsValidTradingName(request.TradingName))
                    invalidOrganisationError = $"{invalidOrganisationError}: Invalid Trading Name [{request.TradingName}]";

                if (!_providerTypeValidator.IsValidProviderTypeId(request.ProviderTypeId))
                    invalidOrganisationError = $"{invalidOrganisationError}: Invalid Provider Type Id [{request.ProviderTypeId}]";

                if (!_organisationValidator.IsValidOrganisationTypeId(request.OrganisationTypeId))
                    invalidOrganisationError = $"{invalidOrganisationError}: Invalid Organisation Type Id [{request.OrganisationTypeId}]";

                if (!_organisationValidator.IsValidStatusDate(request.StatusDate))
                    invalidOrganisationError = $"{invalidOrganisationError}: Invalid Status Date [{request.StatusDate}]";

                if (!_organisationValidator.IsValidUKPRN(request.Ukprn))
                    invalidOrganisationError = $"{invalidOrganisationError}: Invalid ukprn [{request.Ukprn}]";

                var duplicateUkrnDetails = _organisationValidator.DuplicateUkprnInAnotherOrganisation(request.Ukprn, Guid.NewGuid());

                if (duplicateUkrnDetails.DuplicateFound)
                    invalidOrganisationError = $"{invalidOrganisationError}: Duplicate ukprn '{request.Ukprn}' already exists against [{duplicateUkrnDetails}]";

                if (!_organisationValidator.IsValidCompanyNumber(request.CompanyNumber))
                    invalidOrganisationError = $"{invalidOrganisationError}: Invalid company number [{request.CompanyNumber}]";

                if (!_organisationValidator.IsValidCharityNumber(request.CharityNumber))
                    invalidOrganisationError = $"{invalidOrganisationError}: Invalid charity number [{request.CharityNumber}]";

                _logger.LogInformation(invalidOrganisationError);
                throw new BadRequestException(invalidOrganisationError);
            }

            _logger.LogInformation($@"Handling Create Organisation Search for UKPRN [{request.Ukprn}]");

            var command = _mapper.Map(request);

           // var x = Map.Mapper<>;


            var x =
                Mapper.Map<CreateOrganisationCommand>(request);

            return _organisationRepository.CreateOrganisation(command);
        }

        private bool IsValidCreateOrganisation(CreateOrganisationRequest request)
        {
            return (_organisationValidator.IsValidLegalName(request.LegalName)
                    && _organisationValidator.IsValidTradingName(request.TradingName)
                    && _providerTypeValidator.IsValidProviderTypeId(request.ProviderTypeId)        
                    && _organisationValidator.IsValidOrganisationTypeId(request.OrganisationTypeId)
                    && !_organisationValidator.DuplicateUkprnInAnotherOrganisation(request.Ukprn, Guid.NewGuid()).DuplicateFound
                    && _organisationValidator.IsValidStatusDate(request.StatusDate)
                    && _organisationValidator.IsValidUKPRN(request.Ukprn)  
                    && _organisationValidator.IsValidCompanyNumber(request.CompanyNumber)  
                    && _organisationValidator.IsValidCharityNumber(request.CharityNumber)); 
        }
    }
}
