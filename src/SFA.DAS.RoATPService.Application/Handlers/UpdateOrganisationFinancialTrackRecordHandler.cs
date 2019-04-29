using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Api.Types.Models;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Application.Services;

namespace SFA.DAS.RoATPService.Application.Handlers
{

    public class UpdateOrganisationFinancialTrackRecordHandler : IRequestHandler<UpdateOrganisationFinancialTrackRecordRequest, bool>
    {
        private readonly ILogger<UpdateOrganisationFinancialTrackRecordHandler> _logger;
        private readonly IUpdateOrganisationRepository _updateOrganisationRepository;
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IAuditLogService _auditLogService;
        private const string FieldChanged = "Financial Track Record";

        public UpdateOrganisationFinancialTrackRecordHandler(ILogger<UpdateOrganisationFinancialTrackRecordHandler> logger,
            IUpdateOrganisationRepository updateOrganisationRepository, IOrganisationRepository organisationRepository, IAuditLogService auditLogService)
        {
            _logger = logger;
            _updateOrganisationRepository = updateOrganisationRepository;
            _organisationRepository = organisationRepository;
            _auditLogService = auditLogService;
        }

        public async Task<bool> Handle(UpdateOrganisationFinancialTrackRecordRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($@"Handling Update '{FieldChanged}' for Organisation ID [{request.OrganisationId}]");

            var previousFinancialTrackRecord = await _organisationRepository.GetFinancialTrackRecord(request.OrganisationId);

            if (previousFinancialTrackRecord == request.FinancialTrackRecord)
            {
                return await Task.FromResult(false);
            }

            bool success = await _updateOrganisationRepository.UpdateFinancialTrackRecord(request.OrganisationId, request.FinancialTrackRecord, request.UpdatedBy);

            if (!success)
            {
                return await Task.FromResult(false);
            }

            var auditRecord = _auditLogService.CreateAuditLogEntry(request.OrganisationId, request.UpdatedBy,
                FieldChanged, previousFinancialTrackRecord.ToString(), request.FinancialTrackRecord.ToString());

            return await _updateOrganisationRepository.WriteFieldChangesToAuditLog(auditRecord);
        }
    }
}

