using SFA.DAS.RoATPService.Application.Services;

namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Domain;
    using Exceptions;
    using Interfaces;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Validators;

    [Obsolete("Use handlers invoked from UpdateOrganisationController instead")]
    public class UpdateOrganisationHandler : IRequestHandler<UpdateOrganisationRequest, bool>
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IAuditLogFieldComparison _auditLogFieldComparison;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ILogger<UpdateOrganisationHandler> _logger;
        private readonly IOrganisationValidator _organisationValidator;

        public UpdateOrganisationHandler(IOrganisationRepository repository, ILogger<UpdateOrganisationHandler> logger, 
                                         IOrganisationValidator organisationValidator,
                                         IAuditLogFieldComparison auditLogFieldComparison,
                                         IAuditLogRepository auditLogRepository)
        {
            _organisationRepository = repository;
            _logger = logger;
            _organisationValidator = organisationValidator;
            _auditLogFieldComparison = auditLogFieldComparison;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<bool> Handle(UpdateOrganisationRequest request, CancellationToken cancellationToken)
        {
            request.Organisation.LegalName = HtmlTagRemover.StripOutTags(request.Organisation?.LegalName);
            request.Organisation.TradingName = HtmlTagRemover.StripOutTags(request.Organisation?.TradingName);


            if (!IsValidUpdateOrganisation(request.Organisation))
            {
                string invalidOrganisationError = $@"Invalid Organisation data";
                _logger.LogInformation(invalidOrganisationError);
                throw new BadRequestException(invalidOrganisationError);
            }

            _logger.LogInformation($@"Handling Update Organisation for UKPRN [{request.Organisation.UKPRN}]");

            Organisation existingOrganisation = await _organisationRepository.GetOrganisation(request.Organisation.Id);

            var auditData = await _auditLogFieldComparison
                .BuildListOfFieldsChanged(existingOrganisation, request.Organisation);

            if (auditData.FieldChanges.Any())
            { 
                bool updateSuccess = await _organisationRepository.UpdateOrganisation(request.Organisation, request.Username);

                if (updateSuccess)
                {
                    return await _auditLogRepository.WriteFieldChangesToAuditLog(auditData);
                }
            }
            
            return false;
        }

        private bool IsValidUpdateOrganisation(Organisation requestOrganisation)
        {
            return (_organisationValidator.IsValidLegalName(requestOrganisation.LegalName)
                    && _organisationValidator.IsValidTradingName(requestOrganisation.TradingName)
                    && _organisationValidator.IsValidProviderType(requestOrganisation.ProviderType)
                    && _organisationValidator.IsValidOrganisationType(requestOrganisation.OrganisationType)
                    && _organisationValidator.IsValidStatus(requestOrganisation.OrganisationStatus)
                    && _organisationValidator.IsValidStatusDate(requestOrganisation.StatusDate)
                    && _organisationValidator.IsValidUKPRN(requestOrganisation.UKPRN)
                    && requestOrganisation.OrganisationData != null
                    && _organisationValidator.IsValidCompanyNumber(requestOrganisation.OrganisationData.CompanyNumber)
                    && _organisationValidator.IsValidCharityNumber(requestOrganisation.OrganisationData.CharityNumber));
        }
    }
}
