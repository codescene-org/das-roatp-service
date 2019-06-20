namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Interfaces;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Exceptions;
    using Validators;

    public class UpdateOrganisationApplicationDeterminedDateHandler : IRequestHandler<UpdateOrganisationApplicationDeterminedDateRequest, bool>
    {
        private readonly ILogger<UpdateOrganisationApplicationDeterminedDateHandler> _logger;
        private readonly IOrganisationValidator _validator;
        private readonly IUpdateOrganisationRepository _updateOrganisationRepository;
        private readonly IAuditLogService _auditLogService;

        private const string FieldChanged = "Application Determined Date";

        public UpdateOrganisationApplicationDeterminedDateHandler(ILogger<UpdateOrganisationApplicationDeterminedDateHandler> logger,
            IOrganisationValidator validator, IUpdateOrganisationRepository updateOrganisationRepository, IAuditLogService auditLogService)
        {
            _logger = logger;
            _validator = validator;
            _updateOrganisationRepository = updateOrganisationRepository;
            _auditLogService = auditLogService;
        }

        public async Task<bool> Handle(UpdateOrganisationApplicationDeterminedDateRequest request, CancellationToken cancellationToken)
        {
            if (!_validator.IsValidApplicationDeterminedDate(request.ApplicationDeterminedDate))
            {
                var invalidApplicationDeterminedDate = $@"Invalid Application Determined Date '{request.ApplicationDeterminedDate}'";
                _logger.LogInformation(invalidApplicationDeterminedDate);
                throw new BadRequestException(invalidApplicationDeterminedDate);
              }

            _logger.LogInformation($@"Handling Update '{FieldChanged}' for Organisation ID [{request.OrganisationId}]");

            var auditRecord = _auditLogService.AuditApplicationDeterminedDate(request.OrganisationId, request.UpdatedBy, request.ApplicationDeterminedDate);

            if (!auditRecord.ChangesMade)
            {
                return await Task.FromResult(false);
            }

            var success = await _updateOrganisationRepository.UpdateApplicationDeterminedDate(request.OrganisationId, request.ApplicationDeterminedDate, request.UpdatedBy);

            if (!success)
            {
                return await Task.FromResult(false);
            }

            return await _updateOrganisationRepository.WriteFieldChangesToAuditLog(auditRecord);
        }
    }
}