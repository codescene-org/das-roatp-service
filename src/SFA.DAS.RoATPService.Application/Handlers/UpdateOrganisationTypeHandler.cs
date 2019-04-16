namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Domain;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Exceptions;
    using Interfaces;
    using Validators;

    public class UpdateOrganisationTypeHandler : UpdateOrganisationHandlerBase, IRequestHandler<UpdateOrganisationTypeRequest, bool>
    {
        private ILogger<UpdateOrganisationTypeHandler> _logger;
        private IOrganisationValidator _validator;
        private IUpdateOrganisationRepository _updateOrganisationRepository;
        private IAuditLogRepository _auditLogRepository;
        private ILookupDataRepository _lookupRepository;

        public UpdateOrganisationTypeHandler(ILogger<UpdateOrganisationTypeHandler> logger,
            IOrganisationValidator validator, IUpdateOrganisationRepository updateOrganisationRepository,
            IAuditLogRepository auditLogRepository, ILookupDataRepository lookupRepository)
        {
            _logger = logger;
            _validator = validator;
            _updateOrganisationRepository = updateOrganisationRepository;
            _auditLogRepository = auditLogRepository;
            _lookupRepository = lookupRepository;
        }

        public async Task<bool> Handle(UpdateOrganisationTypeRequest request, CancellationToken cancellationToken)
        {

            ValidateUpdateTypeRequest(request);

            var existingTypeId = await _updateOrganisationRepository.GetOrganisationType(request.OrganisationId);

            var success = false;

            var auditData = CreateAuditData(request.OrganisationId, request.UpdatedBy);

            if (existingTypeId != request.OrganisationTypeId)
            {
                AddAuditEntry(auditData, "Organisation Type", TypeText(existingTypeId),
                    TypeText(request.OrganisationTypeId));
            }

            success = await _updateOrganisationRepository.UpdateType(request.OrganisationId,
                request.OrganisationTypeId, request.UpdatedBy);

            if (success)
                success = await _auditLogRepository.WriteFieldChangesToAuditLog(auditData);

         
            return await Task.FromResult(success);
        }

        private void ValidateUpdateTypeRequest(UpdateOrganisationTypeRequest request)
        {
            if (!_validator.IsValidOrganisationTypeId(request.OrganisationTypeId))
            {
                var invalidOrganisationType = $@"Invalid Organisation Type '{request.OrganisationTypeId}'";
                _logger.LogInformation(invalidOrganisationType);
                throw new BadRequestException(invalidOrganisationType);
            }

            var organisationTypeIdUnassigned = 0;

            if (request.OrganisationTypeId == organisationTypeIdUnassigned)
            {
                string organisationTypeUnassignedIsNotAllowed = $@"You cannot set the organisation type to unassigned";
                _logger.LogInformation(organisationTypeUnassignedIsNotAllowed);
                throw new BadRequestException(organisationTypeUnassignedIsNotAllowed);
            }


            if (!_validator.IsValidOrganisationTypeIdForOrganisationProvider(request.OrganisationTypeId, request.OrganisationId))
            {
                var organisationTypeIsNotAllowed = $@"You cannot set the organisation type {request.OrganisationTypeId} for this organisation's provider type";
                _logger.LogInformation(organisationTypeIsNotAllowed);
                throw new BadRequestException(organisationTypeIsNotAllowed);
            }
        }


        private string TypeText(int typeId)
        {
            var organisationType = _lookupRepository.GetOrganisationType(typeId).Result;

            if (organisationType == null)
            {
                _logger.LogError($"Lookup failed for organisation type id {typeId}");
                return "(undefined)";
            }

            return organisationType.Type;
        }
    }
}
