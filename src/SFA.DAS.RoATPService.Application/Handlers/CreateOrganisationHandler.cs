using System;
using System.Globalization;
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
        private readonly IOrganisationRepository _organisationRepository;
        private readonly ILogger<CreateOrganisationHandler> _logger;
        private readonly IOrganisationValidator _organisationValidator;
        private readonly IProviderTypeValidator _providerTypeValidator;

        public CreateOrganisationHandler(IOrganisationRepository repository, ILogger<CreateOrganisationHandler> logger, 
                                         IOrganisationValidator organisationValidator, IProviderTypeValidator providerTypeValidator)
        {
            _organisationRepository = repository;
            _logger = logger;
            _organisationValidator = organisationValidator;
            _providerTypeValidator = providerTypeValidator;
        }

        public Task<Guid?> Handle(CreateOrganisationRequest request, CancellationToken cancellationToken)
        {
            if (!IsValidCreateOrganisation(request))
            {
                string invalidOrganisationError = $@"Invalid Organisation data";
                _logger.LogInformation(invalidOrganisationError);
                throw new BadRequestException(invalidOrganisationError);
            }

            _logger.LogInformation($@"Handling Create Organisation Search for UKPRN [{request.Ukprn}]");

            var command = new CreateOrganisationCommand
            {
                CharityNumber = request.CharityNumber,
                CompanyNumber = request.CompanyNumber,
                FinancialTrackRecord = request.FinancialTrackRecord,
                LegalName = request.LegalName,
                NonLevyContract = request.NonLevyContract,
                OrganisationStatusId = request.OrganisationStatusId,
                OrganisationTypeId = request.OrganisationTypeId,
                ParentCompanyGuarantee = request.ParentCompanyGuarantee,
                ProviderTypeId = request.ProviderTypeId,
                StatusDate = request.StatusDate,
                Ukprn = request.Ukprn,
                TradingName = request.TradingName,
                StartDate = request.StartDate,
                Username = request.Username
            };

            return _organisationRepository.CreateOrganisation(command);
        }

        private bool IsValidCreateOrganisation(CreateOrganisationRequest request)
        {
            return (_organisationValidator.IsValidLegalName(request.LegalName)
                    && _organisationValidator.IsValidTradingName(request.TradingName)
                    && _providerTypeValidator.IsValidProviderTypeId(request.ProviderTypeId)        
                    && _organisationValidator.IsValidOrganisationTypeId(request.OrganisationTypeId)   
                    && _organisationValidator.IsValidStatusId(request.OrganisationStatusId)  
                    && _organisationValidator.IsValidStatusDate(request.StatusDate)
                    && _organisationValidator.IsValidUKPRN(request.Ukprn)  
                    && _organisationValidator.IsValidCompanyNumber(request.CompanyNumber)  
                    && _organisationValidator.IsValidCharityNumber(request.CharityNumber)); 

        }
    }
}
