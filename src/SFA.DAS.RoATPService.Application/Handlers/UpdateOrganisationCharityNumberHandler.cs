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

    public class UpdateOrganisationCharityNumberHandler : IRequestHandler<UpdateOrganisationCharityNumberRequest, bool>
    {
        private readonly ILogger<UpdateOrganisationCharityNumberHandler> _logger;
        private readonly IOrganisationValidator _validator;
        private readonly IUpdateOrganisationRepository _updateOrganisationRepository;
        private readonly IAuditLogService _auditLogService;

        private const string FieldChanged = "Charity Registration Number";

        public UpdateOrganisationCharityNumberHandler(ILogger<UpdateOrganisationCharityNumberHandler> logger,
            IOrganisationValidator validator, IUpdateOrganisationRepository updateOrganisationRepository,
            IAuditLogService auditLogService)
        {
            _logger = logger;
            _validator = validator;
            _updateOrganisationRepository = updateOrganisationRepository;
            _auditLogService = auditLogService;
        }

        public async Task<bool> Handle(UpdateOrganisationCharityNumberRequest request, CancellationToken cancellationToken)
        {
            if (!_validator.IsValidCharityNumber(request.CharityNumber))
            {
                var invalidCharityNumberError = $@"Invalid Organisation Charity Registration Number '{request.CharityNumber}'";
                _logger.LogInformation(invalidCharityNumberError);
                throw new BadRequestException(invalidCharityNumberError);
            }

            var duplicateCompanyNumberDetails = _validator.DuplicateCharityNumberInAnotherOrganisation(request.CharityNumber, request.OrganisationId);

            if (duplicateCompanyNumberDetails.DuplicateFound)
            {
                var duplicateCompanyNumerMessage = $@"Charity registration number '{request.CharityNumber}' already used against organisation '{duplicateCompanyNumberDetails.DuplicateOrganisationName}'";
                _logger.LogInformation(duplicateCompanyNumerMessage);
                throw new BadRequestException(duplicateCompanyNumerMessage);
            }

            if (request?.CharityNumber?.Length < 6 || request?.CharityNumber?.Length > 14)
            {
                var wrongLengthMessage = $@"Charity registration number '{request.CharityNumber}' should be between 6 and 14 characters'";
                _logger.LogInformation(wrongLengthMessage);
                throw new BadRequestException(wrongLengthMessage);
            }


            _logger.LogInformation($@"Handling Update '{FieldChanged}' for Organisation ID [{request.OrganisationId}]");

            var auditRecord = _auditLogService.AuditCharityNumber(request.OrganisationId, request.UpdatedBy, request.CharityNumber);


            if (!auditRecord.ChangesMade)
            {
                return await Task.FromResult(false);
            }

            var success = await _updateOrganisationRepository.UpdateCharityNumber(request.OrganisationId, request.CharityNumber, request.UpdatedBy);

            if (!success)
            {
                return await Task.FromResult(false);
            }

            return await _updateOrganisationRepository.WriteFieldChangesToAuditLog(auditRecord);
        }
    }
}