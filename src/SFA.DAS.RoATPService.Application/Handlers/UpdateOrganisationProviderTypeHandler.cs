using System;
using System.Globalization;
using SFA.DAS.RoATPService.Application.Services;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.RoATPService.Application.Exceptions;
    using SFA.DAS.RoATPService.Application.Interfaces;
    using SFA.DAS.RoATPService.Application.Validators;

    public class UpdateOrganisationProviderTypeHandler : UpdateOrganisationHandlerBase, IRequestHandler<UpdateOrganisationProviderTypeRequest, bool>
    {
        private readonly ILogger<UpdateOrganisationProviderTypeHandler> _logger;
        private readonly IOrganisationValidator _validator;
        private readonly IUpdateOrganisationRepository _updateOrganisationRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ILookupDataRepository _lookupDataRepository;
        private readonly IOrganisationStatusManager _organisationStatusManager;

        private const string FieldChanged = "Provider Type";

        public UpdateOrganisationProviderTypeHandler(ILogger<UpdateOrganisationProviderTypeHandler> logger,
            IOrganisationValidator validator, IUpdateOrganisationRepository updateOrganisationRepository,
            IAuditLogRepository auditLogRepository, ILookupDataRepository lookupDataRepository, 
            IOrganisationStatusManager organisationStatusManager)
        {
            _logger = logger;
            _validator = validator;
            _updateOrganisationRepository = updateOrganisationRepository;
            _auditLogRepository = auditLogRepository;
            _lookupDataRepository = lookupDataRepository;
            _organisationStatusManager = organisationStatusManager;
        }

        public async Task<bool> Handle(UpdateOrganisationProviderTypeRequest request, CancellationToken cancellationToken)
        {
            ValidateUpdateProviderTypeRequest(request);
            _logger.LogInformation($@"Handling Update '{FieldChanged}' for Organisation ID [{request.OrganisationId}]");

            var previousProviderTypeId = await _updateOrganisationRepository.GetProviderType(request.OrganisationId);
            var previousOrganisationStatusId = await _updateOrganisationRepository.GetStatus(request.OrganisationId);
            var previousStartDate = await _updateOrganisationRepository.GetStartDate(request.OrganisationId);

            if (previousProviderTypeId == request.ProviderTypeId)
            {
                return await Task.FromResult(false);
            }
               var success = await _updateOrganisationRepository.UpdateProviderTypeAndOrganisationType(request.OrganisationId, request.ProviderTypeId, request.OrganisationTypeId, request.UpdatedBy);
            if (!success) return await Task.FromResult(false);

            var auditData = CreateAuditData(request.OrganisationId, request.UpdatedBy);

            auditData.FieldChanges.Add(AuditProviderType(request.ProviderTypeId, previousProviderTypeId));
            var previousOrganisationTypeId = await _updateOrganisationRepository.GetOrganisationType(request.OrganisationId);

            var auditLog = AuditOrganisationType(request.OrganisationTypeId, previousOrganisationTypeId, request.ProviderTypeId, previousProviderTypeId);
            if (auditLog.IsValid)
                auditData.FieldChanges.Add(auditLog);

            success = await ProcessOrganisationsDetailsAndUpdateAuditStatusAndStartDate(request.OrganisationId, request.UpdatedBy, request.ProviderTypeId, 
                                                                            previousProviderTypeId, previousOrganisationStatusId, previousStartDate, auditData);
            if (!success) return await Task.FromResult(false);

            success = await _auditLogRepository.WriteFieldChangesToAuditLog(auditData);
            return await Task.FromResult(success);
        }

        private async Task<bool> ProcessOrganisationsDetailsAndUpdateAuditStatusAndStartDate(Guid organisationId, string updatedBy, int providerTypeId, int previousProviderTypeId, 
            int previousOrganisationStatusId, DateTime? previousStartDate, AuditData auditData)
        {
            var changeStatusToActiveAndSetStartDate = 
                _organisationStatusManager.ShouldChangeStatustoActiveAndSetStartDateToToday(providerTypeId, previousProviderTypeId, previousOrganisationStatusId);

          
            if (changeStatusToActiveAndSetStartDate)
            {
                if (previousOrganisationStatusId != OrganisationStatus.Active)
                {
                    var success = await _updateOrganisationRepository.UpdateStatus(organisationId,
                        OrganisationStatus.Active, updatedBy);

                    if (!success)
                    {
                        return false;
                    }

                    AddAuditEntry(
                        auditData,
                        "Organisation Status",
                        GetOrganisationStatus(previousOrganisationStatusId).Result,
                        GetOrganisationStatus(OrganisationStatus.Active).Result
                    );
                }
           
                if (previousStartDate == null || previousStartDate.Value.Date != DateTime.Today.Date)
                {
                    var success = await _updateOrganisationRepository.UpdateStartDate(organisationId, DateTime.Today);

                    if (!success) { return false;}

                    AddAuditEntry(
                        auditData,
                        "Start Date",
                        previousStartDate?.ToString(),
                        DateTime.Today.ToString(CultureInfo.InvariantCulture)
                    );
                }
            }

            var changeStatusToOnboarding =  _organisationStatusManager.ShouldChangeStatusToOnboarding(providerTypeId, previousProviderTypeId, previousOrganisationStatusId);

            if (changeStatusToOnboarding && _organisationStatusManager.IsOrganisationStatusActive(previousOrganisationStatusId))
                {
                    var success = await _updateOrganisationRepository.UpdateStatus(organisationId,
                        OrganisationStatus.Onboarding, updatedBy);

                    if (!success) return await Task.FromResult(false);

                    AddAuditEntry(
                        auditData,
                        "Organisation Status",
                        GetOrganisationStatus(previousOrganisationStatusId).Result,
                        GetOrganisationStatus(OrganisationStatus.Onboarding).Result
                    );
                }
            
            return true;
        }

        private AuditLogEntry AuditProviderType(int providerTypeId, int previousProviderTypeId)
        {;
            var entry = new AuditLogEntry
            {
                FieldChanged = "Provider Type",
                PreviousValue = GetProviderType(previousProviderTypeId).Result,
                NewValue = GetProviderType(providerTypeId).Result
            };

            return entry;
        }


        private AuditLogEntry AuditOrganisationType(int organisationTypeId, int previousOrganisationTypeId, int providerTypeId, int previousProviderTypeId)
        {
            var entry = new AuditLogEntry();

            if (previousOrganisationTypeId != organisationTypeId)
            {
                entry = new AuditLogEntry
                {
                    FieldChanged = "Organisation Type",
                    PreviousValue = GetOrganisationType(previousOrganisationTypeId, previousProviderTypeId).Result,
                    NewValue = GetOrganisationType(organisationTypeId, providerTypeId).Result
                };
            }
            return entry;
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
                var invalidOrganisationTypeId = $@"Invalid Organisation Type Id '{request.OrganisationTypeId}'";
                _logger.LogInformation(invalidOrganisationTypeId);
                throw new BadRequestException(invalidOrganisationTypeId);
            }
        }

        private async Task<string> GetProviderType(int providerTypeId)
        {
            var providerTypes = await _lookupDataRepository.GetProviderTypes();

            var providerType = providerTypes.FirstOrDefault(x => x.Id == providerTypeId);
            if (providerType != null)
            {
                return providerType.Type;
            }

            return string.Empty;
        }

        private async Task<string> GetOrganisationType(int organisationTypeId, int providerTypeId)
        {
            var organisationTypes = await _lookupDataRepository.GetOrganisationTypes(providerTypeId);

            var organisationType = organisationTypes.FirstOrDefault(x => x.Id == organisationTypeId);
            if (organisationType != null)
            {
                return organisationType.Type;
            }

            return string.Empty;
        }

        private async Task<string> GetOrganisationStatus(int organisationStatusId)
        {
            var organisationStatuses = await _lookupDataRepository.GetOrganisationStatuses(null);

            var organisationType = organisationStatuses.FirstOrDefault(x => x.Id == organisationStatusId);
            if (organisationType != null)
            {
                return organisationType.Status;
            }

            return string.Empty;
        }
    }
}
