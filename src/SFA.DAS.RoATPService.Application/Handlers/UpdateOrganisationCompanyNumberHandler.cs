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

    public class UpdateOrganisationCompanyNumberHandler : IRequestHandler<UpdateOrganisationCompanyNumberRequest, bool>
    {
        private readonly ILogger<UpdateOrganisationCompanyNumberHandler> _logger;
        private readonly IOrganisationValidator _validator;
        private readonly IUpdateOrganisationRepository _updateOrganisationRepository;
        private readonly IAuditLogService _auditLogService;

        private const string FieldChanged = "Company Number";

        public UpdateOrganisationCompanyNumberHandler(ILogger<UpdateOrganisationCompanyNumberHandler> logger,
            IOrganisationValidator validator, IUpdateOrganisationRepository updateOrganisationRepository,
            IAuditLogService auditLogService)
        {
            _logger = logger;
            _validator = validator;
            _updateOrganisationRepository = updateOrganisationRepository;
            _auditLogService = auditLogService;
        }

        public async Task<bool> Handle(UpdateOrganisationCompanyNumberRequest request, CancellationToken cancellationToken)
        {
            if (!_validator.IsValidCompanyNumber(request.CompanyNumber))
            {
                var invalidCompanyNumberError = $@"Invalid Organisation Company Number '{request.CompanyNumber}'";
                _logger.LogInformation(invalidCompanyNumberError);
                throw new BadRequestException(invalidCompanyNumberError);
            }


            var duplicateCompanyNumberDetails = _validator.DuplicateCompanyNumberInAnotherOrganisation(request.CompanyNumber, request.OrganisationId);

            if (duplicateCompanyNumberDetails.DuplicateFound)
            {
                var duplicateCompanyNumerMessage = $@"Company number '{request.CompanyNumber}' already used against organisation '{duplicateCompanyNumberDetails.DuplicateOrganisationName}'";
                _logger.LogInformation(duplicateCompanyNumerMessage);
                throw new BadRequestException(duplicateCompanyNumerMessage);
            }

            _logger.LogInformation($@"Handling Update '{FieldChanged}' for Organisation ID [{request.OrganisationId}]");

            var auditRecord = _auditLogService.AuditCompanyNumber(request.OrganisationId, request.UpdatedBy, request.CompanyNumber);


            if (!auditRecord.ChangesMade)
            {
                return await Task.FromResult(false);
            }

            var success = await _updateOrganisationRepository.UpdateCompanyNumber(request.OrganisationId, request.CompanyNumber?.ToUpper(), request.UpdatedBy);

            if (!success)
            {
                return await Task.FromResult(false);
            }

            return await _updateOrganisationRepository.WriteFieldChangesToAuditLog(auditRecord);
        }
    }
}
