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
        private ILogger<UpdateOrganisationProviderTypeHandler> _logger;
        private IOrganisationValidator _validator;
        private IUpdateOrganisationRepository _updateOrganisationRepository;
        private IAuditLogRepository _auditLogRepository;
        private ILookupDataRepository _lookupDataRepository;

        private const string FieldChanged = "Provider Type";

        public UpdateOrganisationProviderTypeHandler(ILogger<UpdateOrganisationProviderTypeHandler> logger,
            IOrganisationValidator validator, IUpdateOrganisationRepository updateOrganisationRepository,
            IAuditLogRepository auditLogRepository, ILookupDataRepository lookupDataRepository)
        {
            _logger = logger;
            _validator = validator;
            _updateOrganisationRepository = updateOrganisationRepository;
            _auditLogRepository = auditLogRepository;
            _lookupDataRepository = lookupDataRepository;
        }

        public async Task<bool> Handle(UpdateOrganisationProviderTypeRequest request, CancellationToken cancellationToken)
        {
            ValidateUpdateProviderTypeRequest(request);

            _logger.LogInformation($@"Handling Update '{FieldChanged}' for Organisation ID [{request.OrganisationId}]");

            int previousProviderTypeId = await _updateOrganisationRepository.GetProviderType(request.OrganisationId);
            int previousOrganisationTypeId = await _updateOrganisationRepository.GetOrganisationType(request.OrganisationId);

            bool success = await _updateOrganisationRepository.UpdateProviderType(request.OrganisationId, request.ProviderTypeId,
                request.OrganisationTypeId, request.UpdatedBy);

            if (success)
            {
                var auditData = CreateAuditData(request.OrganisationId, request.UpdatedBy);

                AddAuditEntry(
                    auditData, 
                    "Provider Type", 
                    GetProviderType(previousProviderTypeId).Result, 
                    GetProviderType(request.ProviderTypeId).Result
                    );

                if (previousOrganisationTypeId != request.OrganisationTypeId)
                {
                    AddAuditEntry(
                        auditData,
                        "Organisation Type",
                        GetOrganisationType(previousOrganisationTypeId, previousProviderTypeId).Result,
                        GetOrganisationType(request.OrganisationTypeId, request.ProviderTypeId).Result
                    );
                }
                
                return await _auditLogRepository.WriteFieldChangesToAuditLog(auditData);
            }

            return await Task.FromResult(success);
        }

        private void ValidateUpdateProviderTypeRequest(UpdateOrganisationProviderTypeRequest request)
        {
            if (!_validator.IsValidProviderTypeId(request.ProviderTypeId))
            {
                string invalidProviderTypeError = $@"Invalid Organisation Provider Type Id '{request.ProviderTypeId}'";
                _logger.LogInformation(invalidProviderTypeError);
                throw new BadRequestException(invalidProviderTypeError);
            }

            if (!_validator.IsValidOrganisationTypeIdForProvider(request.OrganisationTypeId, request.ProviderTypeId).Result)
            {
                string invalidOrganisationTypeId = $@"Invalid Organisation Type Id '{request.OrganisationTypeId}'";
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
    }
}
