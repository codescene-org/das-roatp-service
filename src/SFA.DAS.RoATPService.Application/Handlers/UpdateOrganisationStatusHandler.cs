using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
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
            ILookupDataRepository lookupDataRepository, IOrganisationRepository organisationRepository,
            IAuditLogService auditLogService)
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

            var auditData = _auditLogService.AuditOrganisationStatus(request.OrganisationId, request.UpdatedBy,
                request.OrganisationStatusId, request.RemovedReasonId);

            var success = false;

            if (!auditData.ChangesMade)
            {
                return await Task.FromResult(false);

            }
           
            if (auditData.FieldChanges.Any(x => x.FieldChanged == AuditLogField.RemovedReason))
            {
                success = await _updateOrganisationRepository.UpdateRemovedReason(request.OrganisationId, 
                                                                                        request.RemovedReasonId, request.UpdatedBy);
            }
           
            if (auditData.FieldChanges.Any(x => x.FieldChanged == AuditLogField.OrganisationStatus))
            {
                success = await _updateOrganisationRepository.UpdateOrganisationStatus(request.OrganisationId,
                    request.OrganisationStatusId, request.UpdatedBy);
            }
            

            if (success && auditData.FieldChanges.Any(x => x.FieldChanged == AuditLogField.StartDate))
            {
                success = await _updateOrganisationRepository.UpdateStartDate(request.OrganisationId, DateTime.Today);
            }

            if (success)
                return await _updateOrganisationRepository.WriteFieldChangesToAuditLog(auditData);

            return await Task.FromResult(false);
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
