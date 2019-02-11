namespace SFA.DAS.RoATPService.Application.Handlers
{
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
                string invalidOrganisationError = $@"Invalid Organisation data";
                _logger.LogInformation(invalidOrganisationError);
                throw new BadRequestException(invalidOrganisationError);
            }

            _logger.LogInformation($@"Handling Update Organisation for UKPRN [{request.Organisation.UKPRN}]");

            Organisation existingOrganisation = await _organisationRepository.GetOrganisation(request.Organisation.Id);

            var auditLogEntries = await _auditLogFieldComparison
                .BuildListOfFieldsChanged(existingOrganisation, request.Organisation);

            if (auditLogEntries.Any())
            { 
                bool updateSuccess = await _organisationRepository.UpdateOrganisation(request.Organisation, request.Username);

                if (updateSuccess)
                {
                    return await _auditLogRepository.WriteFieldChangesToAuditLog(auditLogEntries);
                }
            }
            
            return false;
        }

        private bool IsValidUpdateOrganisation(Organisation requestOrganisation)
        {
            return (_organisationValidator.IsValidLegalName(requestOrganisation.LegalName)
                    && _organisationValidator.IsValidProviderTypeId(requestOrganisation.ProviderType.Id)
                    && _organisationValidator.IsValidStatus(requestOrganisation.OrganisationStatus.Id)
                    && _organisationValidator.IsValidStatusDate(requestOrganisation.StatusDate)
                    && _organisationValidator.IsValidUKPRN(requestOrganisation.UKPRN));
        }
    }
}
