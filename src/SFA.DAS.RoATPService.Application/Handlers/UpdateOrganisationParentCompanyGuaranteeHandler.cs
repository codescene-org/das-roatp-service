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

    public class UpdateOrganisationParentCompanyGuaranteeHandler : UpdateOrganisationHandlerBase, IRequestHandler<UpdateOrganisationParentCompanyGuaranteeRequest, bool>
    {
        private readonly ILogger<UpdateOrganisationParentCompanyGuaranteeHandler> _logger;
        private readonly IOrganisationValidator _validator;
        private readonly IUpdateOrganisationRepository _updateOrganisationRepository;
        private readonly IOrganisationRepository _organisationRepository;

        private const string FieldChanged = "Parent Company Guarantee";

        public UpdateOrganisationParentCompanyGuaranteeHandler(ILogger<UpdateOrganisationParentCompanyGuaranteeHandler> logger,
            IOrganisationValidator validator, IUpdateOrganisationRepository updateOrganisationRepository, IOrganisationRepository organisationRepository)
        {
            _logger = logger;
            _validator = validator;
            _updateOrganisationRepository = updateOrganisationRepository;
            _organisationRepository = organisationRepository;
        }

        public async Task<bool> Handle(UpdateOrganisationParentCompanyGuaranteeRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($@"Handling Update '{FieldChanged}' for Organisation ID [{request.OrganisationId}]");

            var previousParentCompanyGuarantee = await _organisationRepository.GetParentCompanyGuarantee(request.OrganisationId);

            if (previousParentCompanyGuarantee == request.ParentCompanyGuarantee)
            {
                return await Task.FromResult(false);
            }

            bool success = await _updateOrganisationRepository.UpdateParentCompanyGuarantee(request.OrganisationId, request.ParentCompanyGuarantee, request.UpdatedBy);

            if (!success)
            {
                return await Task.FromResult(false);
            }

            var auditRecord = CreateAuditLogEntry(request.OrganisationId, request.UpdatedBy,
                FieldChanged, previousParentCompanyGuarantee.ToString(), request.ParentCompanyGuarantee.ToString());

            return await _updateOrganisationRepository.WriteFieldChangesToAuditLog(auditRecord);
        }
    }
}

