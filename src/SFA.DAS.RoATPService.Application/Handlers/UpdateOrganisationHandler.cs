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
            if (!IsValidUpdateOrganisation(request.Organisation))
            {
                string invalidSearchTermError = $@"Invalid Organisation data";
                _logger.LogInformation(invalidSearchTermError);
                throw new BadRequestException(invalidSearchTermError);
            }

            _logger.LogInformation($@"Handling Update Organisation for UKPRN [{request.Organisation.UKPRN}]");

            UpdateOrganisationResult updateResult = await _organisationRepository.UpdateOrganisation(request.Organisation, request.Username);

            if (updateResult.Success)
            {
                var auditLogEntries = await
                    _auditLogFieldComparison.BuildListOfFieldsChanged(updateResult.OriginalOrganisation,
                        updateResult.UpdatedOrganisation);

                if (auditLogEntries.Any())
                {
                    return await _auditLogRepository.WriteFieldChangesToAuditLog(auditLogEntries);
                }
            }

            return false;
        }

        private bool IsValidUpdateOrganisation(Organisation requestOrganisation)
        {
            return (_organisationValidator.IsValidLegalName(requestOrganisation.LegalName)
                    && _organisationValidator.IsValidTradingName(requestOrganisation.TradingName)
                    && _organisationValidator.IsValidApplicationRouteId(requestOrganisation.ApplicationRoute.Id)
                    && _organisationValidator.IsValidStatus(requestOrganisation.Status)
                    && _organisationValidator.IsValidStatusDate(requestOrganisation.StatusDate)
                    && _organisationValidator.IsValidUKPRN(requestOrganisation.UKPRN));
        }
    }
}
