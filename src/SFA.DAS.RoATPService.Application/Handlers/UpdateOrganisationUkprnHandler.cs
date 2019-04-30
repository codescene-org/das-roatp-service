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

    public class UpdateOrganisationUkprnHandler
        : IRequestHandler<UpdateOrganisationUkprnRequest, bool>
    {
        private readonly ILogger<UpdateOrganisationUkprnHandler> _logger;
        private readonly IOrganisationValidator _validator;
        private readonly IUpdateOrganisationRepository _updateOrganisationRepository;
        private readonly IAuditLogService _auditLogService;

        private const string FieldChanged = "UKPRN";

        public UpdateOrganisationUkprnHandler(ILogger<UpdateOrganisationUkprnHandler> logger,
            IOrganisationValidator validator, IUpdateOrganisationRepository updateOrganisationRepository, 
            IAuditLogService auditLogService)
        {
            _logger = logger;
            _validator = validator;
            _updateOrganisationRepository = updateOrganisationRepository;
            _auditLogService = auditLogService;
        }

        public async Task<bool> Handle(UpdateOrganisationUkprnRequest request, CancellationToken cancellationToken)
        {
            if (!_validator.IsValidUKPRN(request.Ukprn))
            {
                var invalidUkprnError = $@"Invalid Organisation Ukprn '{request.Ukprn}'";
                _logger.LogInformation(invalidUkprnError);
                throw new BadRequestException(invalidUkprnError);
            }

            var duplicateUkprnDetails = _validator.DuplicateUkprnInAnotherOrganisation(request.Ukprn, request.OrganisationId);
            
            if (duplicateUkprnDetails.DuplicateFound)
            {
                var invalidUkprnError = $@"Ukprn '{request.Ukprn}' already used against organisation '{duplicateUkprnDetails.DuplicateOrganisationName}'";
                _logger.LogInformation(invalidUkprnError);
                throw new BadRequestException(invalidUkprnError);
            }

            _logger.LogInformation($@"Handling Update '{FieldChanged}' for Organisation ID [{request.OrganisationId}]");

            var auditRecord = _auditLogService.AuditUkprn(request.OrganisationId, request.UpdatedBy, request.Ukprn);


            if (!auditRecord.ChangesMade)
            {
                return await Task.FromResult(false);
            }

            var success = await _updateOrganisationRepository.UpdateUkprn(request.OrganisationId, request.Ukprn, request.UpdatedBy);

            if (!success)
            {
                return await Task.FromResult(false);
            }

            return await _updateOrganisationRepository.WriteFieldChangesToAuditLog(auditRecord);
        }
    }
}
