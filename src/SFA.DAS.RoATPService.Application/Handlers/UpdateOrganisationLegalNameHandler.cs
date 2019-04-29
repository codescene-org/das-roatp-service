using SFA.DAS.RoATPService.Application.Services;

namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Interfaces;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.RoATPService.Application.Exceptions;
    using Validators;
  
    public class UpdateOrganisationLegalNameHandler : IRequestHandler<UpdateOrganisationLegalNameRequest, bool>
    {
        private readonly ILogger<UpdateOrganisationLegalNameHandler> _logger;
        private readonly IOrganisationValidator _validator;
        private readonly IUpdateOrganisationRepository _updateOrganisationRepository;
        private readonly IOrganisationRepository _organisationRepository;
        private readonly ITextSanitiser _textSanitiser;
        private readonly IAuditLogService _auditLogService;

        private const string FieldChanged = "Legal Name";

        public UpdateOrganisationLegalNameHandler(ILogger<UpdateOrganisationLegalNameHandler> logger,
            IOrganisationValidator validator, IUpdateOrganisationRepository updateOrganisationRepository, 
            IOrganisationRepository organisationRepository, ITextSanitiser textSanitiser, IAuditLogService auditLogService)
        {
            _logger = logger;
            _validator = validator;
            _updateOrganisationRepository = updateOrganisationRepository;
            _organisationRepository = organisationRepository;
            _textSanitiser = textSanitiser;
            _auditLogService = auditLogService;
        }

        public async Task<bool> Handle(UpdateOrganisationLegalNameRequest request, CancellationToken cancellationToken)
        {
            request.LegalName = _textSanitiser.SanitiseInputText(request.LegalName);

            if (!_validator.IsValidLegalName(request.LegalName))
            {
                string invalidLegalNameError = $@"Invalid Organisation Legal Name '{request.LegalName}'";
                _logger.LogInformation(invalidLegalNameError);
                throw new BadRequestException(invalidLegalNameError);
            }

            _logger.LogInformation($@"Handling Update '{FieldChanged}' for Organisation ID [{request.OrganisationId}]");

            var previousLegalName = await _organisationRepository.GetLegalName(request.OrganisationId);

            if (previousLegalName == request.LegalName)
            {
                return await Task.FromResult(false);
            }

       
            var success = await _updateOrganisationRepository.UpdateLegalName(request.OrganisationId, request.LegalName, request.UpdatedBy);

            if (!success)
            {
                return await Task.FromResult(false);
            }

            var auditRecord = _auditLogService.CreateAuditLogEntry(request.OrganisationId, request.UpdatedBy,
                                                  FieldChanged, previousLegalName, request.LegalName);

            return await _updateOrganisationRepository.WriteFieldChangesToAuditLog(auditRecord);
        }
    }
}
