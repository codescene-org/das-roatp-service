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

    public class UpdateOrganisationParentCompanyGuaranteeHandler : IRequestHandler<UpdateOrganisationParentCompanyGuaranteeRequest, bool>
    {
        private readonly ILogger<UpdateOrganisationParentCompanyGuaranteeHandler> _logger;
        private readonly IUpdateOrganisationRepository _updateOrganisationRepository;
        private readonly IAuditLogService _auditLogService;

        private const string FieldChanged = "Parent Company Guarantee";

        public UpdateOrganisationParentCompanyGuaranteeHandler(ILogger<UpdateOrganisationParentCompanyGuaranteeHandler> logger,
            IUpdateOrganisationRepository updateOrganisationRepository,  IAuditLogService auditLogService)
        {
            _logger = logger;
            _updateOrganisationRepository = updateOrganisationRepository;
            _auditLogService = auditLogService;
        }

        public async Task<bool> Handle(UpdateOrganisationParentCompanyGuaranteeRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($@"Handling Update '{FieldChanged}' for Organisation ID [{request.OrganisationId}]");

            var auditRecord = _auditLogService.AuditParentCompanyGuarantee(request.OrganisationId, request.UpdatedBy, request.ParentCompanyGuarantee);

            if (!auditRecord.ChangesMade)
            {
                return await Task.FromResult(false);
            }

            var success = await _updateOrganisationRepository.UpdateParentCompanyGuarantee(request.OrganisationId, request.ParentCompanyGuarantee, request.UpdatedBy);

            if (!success)
            {
                return await Task.FromResult(false);
            }

            return await _updateOrganisationRepository.WriteFieldChangesToAuditLog(auditRecord);
        }
    }
}

