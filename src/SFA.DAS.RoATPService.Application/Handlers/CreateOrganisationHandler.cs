﻿namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System;
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
        private readonly ICreateOrganisationRepository _organisationRepository;
        private readonly IEventsRepository _eventsRepository;
        private readonly ILogger<CreateOrganisationHandler> _logger;
        private readonly IOrganisationValidator _organisationValidator;
        private readonly IProviderTypeValidator _providerTypeValidator;
        private readonly IMapCreateOrganisationRequestToCommand _mapper;
        private readonly ITextSanitiser _textSanitiser;

        public CreateOrganisationHandler(ICreateOrganisationRepository repository, IEventsRepository eventsRepository, 
                                        ILogger<CreateOrganisationHandler> logger, IOrganisationValidator organisationValidator, 
                                        IProviderTypeValidator providerTypeValidator, IMapCreateOrganisationRequestToCommand mapper, 
                                        ITextSanitiser textSanitiser)
        {
            _organisationRepository = repository;
            _logger = logger;
            _organisationValidator = organisationValidator;
            _providerTypeValidator = providerTypeValidator;
            _mapper = mapper;
            _textSanitiser = textSanitiser;
            _eventsRepository = eventsRepository;
        }

        public Task<Guid?> Handle(CreateOrganisationRequest request, CancellationToken cancellationToken)
        {
            request.LegalName = _textSanitiser.SanitiseInputText(request.LegalName);
            request.TradingName = _textSanitiser.SanitiseInputText(request.TradingName);

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
                    invalidOrganisationError = $"{invalidOrganisationError}: Duplicate ukprn '{request.Ukprn}' already exists against [{duplicateUkrnDetails.DuplicateOrganisationName}]";


                if (!_organisationValidator.IsValidCompanyNumber(request.CompanyNumber))
                    invalidOrganisationError = $"{invalidOrganisationError}: Invalid company number [{request.CompanyNumber}]";

                if (!_organisationValidator.IsValidApplicationDeterminedDate(request.ApplicationDeterminedDate))
                    invalidOrganisationError = $"{invalidOrganisationError}: Invalid Application Determined Date [{request.ApplicationDeterminedDate}]";

                if (!string.IsNullOrEmpty(request.CompanyNumber))
                {
                    var duplicateCompanyNumber =
                        _organisationValidator.DuplicateCompanyNumberInAnotherOrganisation(request.CompanyNumber,
                            Guid.NewGuid());

                    if (duplicateCompanyNumber.DuplicateFound)
                        invalidOrganisationError =
                            $"{invalidOrganisationError}: Duplicate company number '{request.CompanyNumber}' already exists against [{duplicateCompanyNumber.DuplicateOrganisationName}]";
                }

             
                if (!_organisationValidator.IsValidCharityNumber(request.CharityNumber))
                    invalidOrganisationError = $"{invalidOrganisationError}: Invalid charity registration number [{request.CharityNumber}]";

                if (!string.IsNullOrEmpty(request.CharityNumber))
                {
                    var duplicateCharityNumber =
                        _organisationValidator.DuplicateCharityNumberInAnotherOrganisation(request.CharityNumber,
                            Guid.NewGuid());

                    if (duplicateCharityNumber.DuplicateFound)
                        invalidOrganisationError =
                            $"{invalidOrganisationError}: Duplicate charity registration number '{request.CharityNumber}' already exists against [{duplicateCharityNumber.DuplicateOrganisationName}]";
                }

                _logger.LogInformation(invalidOrganisationError);
                throw new BadRequestException(invalidOrganisationError);
            }

            _logger.LogInformation($@"Handling Create Organisation Search for UKPRN [{request.Ukprn}]");
  
            var command = _mapper.Map(request);
            var organisationId = _organisationRepository.CreateOrganisation(command);

            _eventsRepository.AddOrganisationStatusEvents(command.Ukprn, command.OrganisationStatusId,
                command.StatusDate);

            return organisationId;
        }

        private bool IsValidCreateOrganisation(CreateOrganisationRequest request)
        {
            return (_organisationValidator.IsValidLegalName(request.LegalName)
                    && _organisationValidator.IsValidTradingName(request.TradingName)
                    && _providerTypeValidator.IsValidProviderTypeId(request.ProviderTypeId)        
                    && _organisationValidator.IsValidOrganisationTypeId(request.OrganisationTypeId)
                    && !_organisationValidator.DuplicateUkprnInAnotherOrganisation(request.Ukprn, Guid.NewGuid()).DuplicateFound
                    && !_organisationValidator.DuplicateCompanyNumberInAnotherOrganisation(request.CompanyNumber, Guid.NewGuid()).DuplicateFound
                    && !_organisationValidator.DuplicateCharityNumberInAnotherOrganisation(request.CharityNumber, Guid.NewGuid()).DuplicateFound
                    && _organisationValidator.IsValidStatusDate(request.StatusDate)
                    && _organisationValidator.IsValidUKPRN(request.Ukprn)  
                    && _organisationValidator.IsValidApplicationDeterminedDate(request.ApplicationDeterminedDate)
                    && _organisationValidator.IsValidCompanyNumber(request.CompanyNumber)  
                    && _organisationValidator.IsValidCharityNumber(request.CharityNumber)); 
        }
    }
}
