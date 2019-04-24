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

    public class UpdateOrganisationLegalNameHandler : UpdateOrganisationHandlerBase, IRequestHandler<UpdateOrganisationLegalNameRequest, bool>
    {
        private ILogger<UpdateOrganisationLegalNameHandler> _logger;
        private readonly IOrganisationValidator _validator;
        private readonly IUpdateOrganisationRepository _updateOrganisationRepository;

        private const string FieldChanged = "Legal Name";

        public UpdateOrganisationLegalNameHandler(ILogger<UpdateOrganisationLegalNameHandler> logger,
            IOrganisationValidator validator, IUpdateOrganisationRepository updateOrganisationRepository)
        {
            _logger = logger;
            _validator = validator;
            _updateOrganisationRepository = updateOrganisationRepository;
        }

        public async Task<bool> Handle(UpdateOrganisationLegalNameRequest request, CancellationToken cancellationToken)
        {
            if (!_validator.IsValidLegalName(request.LegalName))
            {
                string invalidLegalNameError = $@"Invalid Organisation Legal Name '{request.LegalName}'";
                _logger.LogInformation(invalidLegalNameError);
                throw new BadRequestException(invalidLegalNameError);
            }

            _logger.LogInformation($@"Handling Update '{FieldChanged}' for Organisation ID [{request.OrganisationId}]");

            string previousLegalName = await _updateOrganisationRepository.GetLegalName(request.OrganisationId);

            if (previousLegalName == request.LegalName)
            {
                return await Task.FromResult(false);
            }

            request.LegalName = HtmlTagRemover.StripOutTags(request.LegalName);

            bool success = await _updateOrganisationRepository.UpdateLegalName(request.OrganisationId, request.LegalName, request.UpdatedBy);

            if (!success)
            {
                return await Task.FromResult(false);
            }

            var auditRecord = CreateAuditLogEntry(request.OrganisationId, request.UpdatedBy,
                                                  FieldChanged, previousLegalName, request.LegalName);

            return await _updateOrganisationRepository.WriteFieldChangesToAuditLog(auditRecord);
        }
    }
}
