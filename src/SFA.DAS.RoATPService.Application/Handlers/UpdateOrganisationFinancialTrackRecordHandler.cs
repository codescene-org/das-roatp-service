using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Api.Types.Models;
using SFA.DAS.RoATPService.Application.Interfaces;

namespace SFA.DAS.RoATPService.Application.Handlers
{

    public class UpdateOrganisationFinancialTrackRecordHandler : IRequestHandler<UpdateOrganisationFinancialTrackRecordRequest, bool>
    {
        private readonly ILogger<UpdateOrganisationFinancialTrackRecordHandler> _logger;
        private readonly IUpdateOrganisationRepository _updateOrganisationRepository;
        private readonly IAuditLogService _auditLogService;
        private const string FieldChanged = "Financial Track Record";

        public UpdateOrganisationFinancialTrackRecordHandler(ILogger<UpdateOrganisationFinancialTrackRecordHandler> logger,
            IUpdateOrganisationRepository updateOrganisationRepository, IAuditLogService auditLogService)
        {
            _logger = logger;
            _updateOrganisationRepository = updateOrganisationRepository;
            _auditLogService = auditLogService;
        }

        public async Task<bool> Handle(UpdateOrganisationFinancialTrackRecordRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($@"Handling Update '{FieldChanged}' for Organisation ID [{request.OrganisationId}]");
     
            var auditRecord = _auditLogService.AuditFinancialTrackRecord(request.OrganisationId, request.UpdatedBy, request.FinancialTrackRecord);

            if (!auditRecord.ChangesMade)
            {
                return await Task.FromResult(false);
            }

            var success = await _updateOrganisationRepository.UpdateFinancialTrackRecord(request.OrganisationId, request.FinancialTrackRecord, request.UpdatedBy);

            if (!success)
            {
                return await Task.FromResult(false);
            }

            return await _updateOrganisationRepository.WriteFieldChangesToAuditLog(auditRecord);
        }
    }
}

