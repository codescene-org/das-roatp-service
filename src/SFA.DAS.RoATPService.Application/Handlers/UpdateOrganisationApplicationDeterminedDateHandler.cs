namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Interfaces;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Exceptions;
    using Validators;

    public class UpdateOrganisationApplicationDeterminedDateHandler : IRequestHandler<UpdateOrganisationApplicationDeterminedDateRequest, bool>
    {
        private readonly ILogger<UpdateOrganisationApplicationDeterminedDateHandler> _logger;
        private readonly IOrganisationValidator _validator;
        private readonly IUpdateOrganisationRepository _updateOrganisationRepository;
        private readonly ITextSanitiser _textSanitiser;
        private readonly IAuditLogService _auditLogService;

        private const string FieldChanged = "Legal Name";

        public UpdateOrganisationApplicationDeterminedDateHandler(ILogger<UpdateOrganisationApplicationDeterminedDateHandler> logger,
            IOrganisationValidator validator, IUpdateOrganisationRepository updateOrganisationRepository,
            IAuditLogService auditLogService)
        {
            _logger = logger;
            _validator = validator;
            _updateOrganisationRepository = updateOrganisationRepository;
            _auditLogService = auditLogService;
        }

        public async Task<bool> Handle(UpdateOrganisationApplicationDeterminedDateRequest request, CancellationToken cancellationToken)
        {
            if (!_validator.IsValidApplicationDeterminedDate(request.ApplicationDeterminedDate))
            {
                string invalidApplicationDeterminedDate = $@"Invalid Application Determined Date '{request.ApplicationDeterminedDate}'";
                _logger.LogInformation(invalidApplicationDeterminedDate);
                throw new BadRequestException(invalidApplicationDeterminedDate);
              }


            //var auditRecord = _auditLogService.AuditLegalName(request.OrganisationId, request.UpdatedBy, legalName);

            //if (!auditRecord.ChangesMade)
            //{
            //    return await Task.FromResult(false);
            //}


            //MFCMFC update application determined date
            var success = await _updateOrganisationRepository.UpdateApplicationDeterminedDate(request.OrganisationId, request.ApplicationDeterminedDate, request.UpdatedBy);

            if (!success)
            {
                return await Task.FromResult(false);
            }

            // return await _updateOrganisationRepository.WriteFieldChangesToAuditLog(auditRecord);

            return await Task.FromResult(true);
        }
    }
}