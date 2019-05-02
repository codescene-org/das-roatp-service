using SFA.DAS.RoATPService.Application.Services;

namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Domain;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Exceptions;
    using Interfaces;
    using Validators;

    public class UpdateOrganisationTypeHandler : IRequestHandler<UpdateOrganisationTypeRequest, bool>
    {
        private readonly ILogger<UpdateOrganisationTypeHandler> _logger;
        private readonly IOrganisationValidator _validator;
        private readonly IUpdateOrganisationRepository _updateOrganisationRepository;
        private readonly IAuditLogService _auditLogService;

        public UpdateOrganisationTypeHandler(ILogger<UpdateOrganisationTypeHandler> logger,
            IOrganisationValidator validator, IUpdateOrganisationRepository updateOrganisationRepository,
            IAuditLogService auditLogService)
        {
            _logger = logger;
            _validator = validator;
            _updateOrganisationRepository = updateOrganisationRepository;
            _auditLogService = auditLogService;
        }

        public async Task<bool> Handle(UpdateOrganisationTypeRequest request, CancellationToken cancellationToken)
        {

            ValidateUpdateTypeRequest(request);

            var auditRecord = _auditLogService.AuditOrganisationType(request.OrganisationId, request.UpdatedBy, request.OrganisationTypeId);

            var success = false;

            if (auditRecord.ChangesMade && request.OrganisationTypeId!= OrganisationType.Unassigned)
            {
                success = await _updateOrganisationRepository.UpdateOrganisationType(request.OrganisationId,
                    request.OrganisationTypeId, request.UpdatedBy);    
            }
            if (!success)
            {
                return await Task.FromResult(false);
            }

            return await _updateOrganisationRepository.WriteFieldChangesToAuditLog(auditRecord);
        }

        private void ValidateUpdateTypeRequest(UpdateOrganisationTypeRequest request)
        {
            if (!_validator.IsValidOrganisationTypeId(request.OrganisationTypeId))
            {
                var invalidOrganisationType = $@"Invalid Organisation Type '{request.OrganisationTypeId}'";
                _logger.LogInformation(invalidOrganisationType);
                throw new BadRequestException(invalidOrganisationType);
            }

            if (request.OrganisationTypeId == OrganisationType.Unassigned)
            {
                string organisationTypeUnassignedIsNotAllowed = $@"You cannot set the organisation type to unassigned";
                _logger.LogInformation(organisationTypeUnassignedIsNotAllowed);
                throw new BadRequestException(organisationTypeUnassignedIsNotAllowed);
            }

            if (!_validator.IsValidOrganisationTypeIdForOrganisation(request.OrganisationTypeId, request.OrganisationId))
            {
                var organisationTypeIsNotAllowed = $@"You cannot set the organisation type Id [{request.OrganisationTypeId}] for this organisation's provider type";
                _logger.LogInformation(organisationTypeIsNotAllowed);
                throw new BadRequestException(organisationTypeIsNotAllowed);
            }
        }
    }
}
