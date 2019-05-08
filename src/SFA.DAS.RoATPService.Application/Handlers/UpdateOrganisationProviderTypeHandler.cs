namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System;
    using Services;
    using Domain;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Exceptions;
    using Interfaces;
    using Validators;

    public class UpdateOrganisationProviderTypeHandler : IRequestHandler<UpdateOrganisationProviderTypeRequest, bool>
    {
        private readonly ILogger<UpdateOrganisationProviderTypeHandler> _logger;
        private readonly IOrganisationValidator _validator;
        private readonly IUpdateOrganisationRepository _updateOrganisationRepository;
        private readonly IAuditLogService _auditLogService;

        private const string FieldChanged = "Provider Type";

        public UpdateOrganisationProviderTypeHandler(ILogger<UpdateOrganisationProviderTypeHandler> logger,
            IOrganisationValidator validator, IUpdateOrganisationRepository updateOrganisationRepository,
            IAuditLogService auditLogService)
        {
            _logger = logger;
            _validator = validator;
            _updateOrganisationRepository = updateOrganisationRepository;
            _auditLogService = auditLogService;
        }

        public async Task<bool> Handle(UpdateOrganisationProviderTypeRequest request, CancellationToken cancellationToken)
        {
            ValidateUpdateProviderTypeRequest(request);
            _logger.LogInformation($@"Handling Update '{FieldChanged}' for Organisation ID [{request.OrganisationId}]");

            var auditData = _auditLogService.AuditProviderType(request.OrganisationId, request.UpdatedBy, request.ProviderTypeId, request.OrganisationTypeId);

            var success = false;

            if (!auditData.ChangesMade)
            {
                return await Task.FromResult(false);
            }

            if (auditData.FieldChanges.Any(x => x.FieldChanged == AuditLogField.ProviderType))
            {
                success = await _updateOrganisationRepository.UpdateProviderType(request.OrganisationId,
                    request.ProviderTypeId, request.UpdatedBy);
            }

            if (success && auditData.FieldChanges.Any(x => x.FieldChanged == AuditLogField.OrganisationType))
            {
                success = await _updateOrganisationRepository.UpdateOrganisationType(request.OrganisationId,
                    request.OrganisationTypeId, request.UpdatedBy);
            }

            if (success && auditData.FieldChanges.Any(x => x.FieldChanged == AuditLogField.OrganisationStatus))
            {
                success = await _updateOrganisationRepository.UpdateOrganisationStatus(request.OrganisationId, OrganisationStatus.Active, request.UpdatedBy);
            }

            if (success && auditData.FieldChanges.Any(x => x.FieldChanged == AuditLogField.StartDate))
            {
                success = await _updateOrganisationRepository.UpdateStartDate(request.OrganisationId, DateTime.Today, request.UpdatedBy);
            }

            if (success)
                return await _updateOrganisationRepository.WriteFieldChangesToAuditLog(auditData);

            return await Task.FromResult(false);
        }

        private void ValidateUpdateProviderTypeRequest(UpdateOrganisationProviderTypeRequest request)
        {
            if (!_validator.IsValidProviderTypeId(request.ProviderTypeId))
            {
                var invalidProviderTypeError = $@"Invalid Organisation Provider Type Id '{request.ProviderTypeId}'";
                _logger.LogInformation(invalidProviderTypeError);
                throw new BadRequestException(invalidProviderTypeError);
            }

            if (!_validator.IsValidOrganisationTypeIdForProvider(request.OrganisationTypeId, request.ProviderTypeId).Result)
            {
                var invalidOrganisationTypeId = $@"Invalid Organisation Type Id '{request.OrganisationTypeId}' for Provider Type Id '{request.ProviderTypeId}'";
                _logger.LogInformation(invalidOrganisationTypeId);
                throw new BadRequestException(invalidOrganisationTypeId);
            }
        }
    }
}
