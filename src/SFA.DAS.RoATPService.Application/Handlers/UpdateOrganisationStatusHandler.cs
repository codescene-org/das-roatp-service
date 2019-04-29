using SFA.DAS.RoATPService.Application.Services;

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

    public class UpdateOrganisationStatusHandler : IRequestHandler<UpdateOrganisationStatusRequest, bool>
    {
        private readonly ILogger<UpdateOrganisationStatusHandler> _logger;
        private readonly IOrganisationValidator _validator;
        private readonly IUpdateOrganisationRepository _updateOrganisationRepository;
        private readonly ILookupDataRepository _lookupDataRepository;
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IAuditLogService _auditLogService;

        public UpdateOrganisationStatusHandler(ILogger<UpdateOrganisationStatusHandler> logger,
            IOrganisationValidator validator, IUpdateOrganisationRepository updateOrganisationRepository,
            ILookupDataRepository lookupDataRepository, IOrganisationRepository organisationRepository, IAuditLogService auditLogService)
        {
            _logger = logger;
            _validator = validator;
            _updateOrganisationRepository = updateOrganisationRepository;
            _lookupDataRepository = lookupDataRepository;
            _organisationRepository = organisationRepository;
            _auditLogService = auditLogService;
        }

        public async Task<bool> Handle(UpdateOrganisationStatusRequest request, CancellationToken cancellationToken)
        {
            ValidateUpdateStatusRequest(request);

            int existingStatusId = await _organisationRepository.GetOrganisationStatus(request.OrganisationId);
            var removedReason = await _organisationRepository.GetRemovedReason(request.OrganisationId);

            bool success = false;

            var auditData = _auditLogService.CreateAuditData(request.OrganisationId, request.UpdatedBy);

            if (existingStatusId != request.OrganisationStatusId)
            {
              _auditLogService.AddAuditEntry(auditData, "Organisation Status", StatusText(existingStatusId),
                              StatusText(request.OrganisationStatusId));                   
            }
            
            if (!request.RemovedReasonId.HasValue)
            {
                success = await _updateOrganisationRepository.UpdateOrganisationStatus(request.OrganisationId,
                                request.OrganisationStatusId,  request.UpdatedBy);
            }
            else
            {
                var reason = await _updateOrganisationRepository.UpdateStatusWithRemovedReason(
                    request.OrganisationId, request.OrganisationStatusId, 
                    request.RemovedReasonId.Value, request.UpdatedBy);

                if (removedReason == null || request.RemovedReasonId.Value != removedReason.Id)
                {
                  _auditLogService.AddAuditEntry(auditData, "Removed Reason", removedReason?.Reason ?? "Not set", reason.Reason);
                }
            }
            
            success = await _updateOrganisationRepository.WriteFieldChangesToAuditLog(auditData);

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

            if (request.OrganisationStatusId != OrganisationStatus.Removed && request.RemovedReasonId.HasValue)
            {
                var invalidRemovalReasonError = $@"Invalid Removal Reason for '{request.OrganisationStatusId}'";
                _logger.LogInformation(invalidRemovalReasonError);
                throw new BadRequestException(invalidRemovalReasonError);
            }

            if (!_validator.IsValidOrganisationStatusIdForOrganisation(request.OrganisationStatusId,
                request.OrganisationId))
            {
                var invalidStatusForOrganisationError = $@"You cannot set the organisation status {request.OrganisationStatusId} for this organisation's provider type";

                _logger.LogInformation(invalidStatusForOrganisationError);
                throw new BadRequestException(invalidStatusForOrganisationError);
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
            var organisationStatus = _lookupDataRepository.GetOrganisationStatus(statusId).Result;

            if (organisationStatus == null)
            {
                _logger.LogError($"Lookup failed for organisation status id {statusId}");
                return "(undefined)";
            }

            return organisationStatus.Status;
        }
    }
}
