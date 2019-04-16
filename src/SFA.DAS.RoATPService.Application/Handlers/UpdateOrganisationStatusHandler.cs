namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Domain;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.RoATPService.Application.Exceptions;
    using SFA.DAS.RoATPService.Application.Interfaces;
    using Validators;

    public class UpdateOrganisationStatusHandler : UpdateOrganisationHandlerBase, IRequestHandler<UpdateOrganisationStatusRequest, bool>
    {
        private ILogger<UpdateOrganisationStatusHandler> _logger;
        private IOrganisationValidator _validator;
        private IUpdateOrganisationRepository _updateOrganisationRepository;
        private IAuditLogRepository _auditLogRepository;
        private IOrganisationStatusRepository _organisationStatusRepository;

        public UpdateOrganisationStatusHandler(ILogger<UpdateOrganisationStatusHandler> logger,
            IOrganisationValidator validator, IUpdateOrganisationRepository updateOrganisationRepository,
            IAuditLogRepository auditLogRepository, IOrganisationStatusRepository organisationStatusRepository)
        {
            _logger = logger;
            _validator = validator;
            _updateOrganisationRepository = updateOrganisationRepository;
            _auditLogRepository = auditLogRepository;
            _organisationStatusRepository = organisationStatusRepository;
        }

        public async Task<bool> Handle(UpdateOrganisationStatusRequest request, CancellationToken cancellationToken)
        {
            ValidateUpdateStatusRequest(request);

            int existingStatusId = await _updateOrganisationRepository.GetStatus(request.OrganisationId);
            var removedReason = await _updateOrganisationRepository.GetRemovedReason(request.OrganisationId);

            bool success = false;

            var auditData = CreateAuditData(request.OrganisationId, request.UpdatedBy);

            if (existingStatusId != request.OrganisationStatusId)
            {
                AddAuditEntry(auditData, "Organisation Status", StatusText(existingStatusId),
                              StatusText(request.OrganisationStatusId));                   
            }
            
            if (!request.RemovedReasonId.HasValue)
            {
                success = await _updateOrganisationRepository.UpdateStatus(request.OrganisationId,
                                request.OrganisationStatusId,  request.UpdatedBy);
            }
            else
            {
                var reason = await _updateOrganisationRepository.UpdateStatusWithRemovedReason(
                    request.OrganisationId, request.OrganisationStatusId, 
                    request.RemovedReasonId.Value, request.UpdatedBy);

                if (removedReason == null || request.RemovedReasonId.Value != removedReason.Id)
                {
                    AddAuditEntry(auditData, "Removed Reason", removedReason?.Reason ?? "Not set", reason.Reason);
                }
            }
            
            success = await _auditLogRepository.WriteFieldChangesToAuditLog(auditData);

            if (success && UpdateStartDateRequired(existingStatusId, request.OrganisationStatusId))
            {
                success = await _updateOrganisationRepository.UpdateStartDate(request.OrganisationId, DateTime.Today);
            }

            return await Task.FromResult(success);
        }

        private void ValidateUpdateStatusRequest(UpdateOrganisationStatusRequest request)
        {
            if (!_validator.IsValidStatusId(request.OrganisationStatusId))
            {
                string invalidStatusError = $@"Invalid Organisation Status '{request.OrganisationStatusId}'";
                _logger.LogInformation(invalidStatusError);
                throw new BadRequestException(invalidStatusError);
            }

            if (request.OrganisationStatusId != 0 && request.RemovedReasonId.HasValue)
            {
                string invalidRemovalReasonError = $@"Invalid Removal Reason for '{request.OrganisationStatusId}'";
                _logger.LogInformation(invalidRemovalReasonError);
                throw new BadRequestException(invalidRemovalReasonError);
            }
        }

        private bool UpdateStartDateRequired(int oldStatusId, int newStatusId)
        {
            if ((oldStatusId == OrganisationStatus.Removed || oldStatusId == OrganisationStatus.Onboarding) &&
                (newStatusId == OrganisationStatus.Active || newStatusId == OrganisationStatus.ActiveNotTakingOnApprentices))
            {
                return true;              
            }

            return false;
        }

        private string StatusText(int statusId)
        {
            var organisationStatus = _organisationStatusRepository.GetOrganisationStatus(statusId).Result;

            if (organisationStatus == null)
            {
                _logger.LogError($"Lookup failed for organisation status id {statusId}");
                return "(undefined)";
            }

            return organisationStatus.Status;
        }
    }
}
