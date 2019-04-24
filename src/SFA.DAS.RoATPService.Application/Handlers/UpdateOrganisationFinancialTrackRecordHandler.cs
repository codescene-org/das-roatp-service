using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Api.Types.Models;
using SFA.DAS.RoATPService.Application.Interfaces;

namespace SFA.DAS.RoATPService.Application.Handlers
{

    public class UpdateOrganisationFinancialTrackRecordHandler : UpdateOrganisationHandlerBase, IRequestHandler<UpdateOrganisationFinancialTrackRecordRequest, bool>
    {
        private readonly ILogger<UpdateOrganisationFinancialTrackRecordHandler> _logger;
        private readonly IUpdateOrganisationRepository _updateOrganisationRepository;

        private const string FieldChanged = "Financial Track Record";

        public UpdateOrganisationFinancialTrackRecordHandler(ILogger<UpdateOrganisationFinancialTrackRecordHandler> logger,
            IUpdateOrganisationRepository updateOrganisationRepository)
        {
            _logger = logger;
            _updateOrganisationRepository = updateOrganisationRepository;
        }

        public async Task<bool> Handle(UpdateOrganisationFinancialTrackRecordRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($@"Handling Update '{FieldChanged}' for Organisation ID [{request.OrganisationId}]");

            var previousFinancialTrackRecord = await _updateOrganisationRepository.GetFinancialTrackRecord(request.OrganisationId);

            if (previousFinancialTrackRecord == request.FinancialTrackRecord)
            {
                return await Task.FromResult(false);
            }

            bool success = await _updateOrganisationRepository.UpdateFinancialTrackRecord(request.OrganisationId, request.FinancialTrackRecord, request.UpdatedBy);

            if (!success)
            {
                return await Task.FromResult(false);
            }

            var auditRecord = CreateAuditLogEntry(request.OrganisationId, request.UpdatedBy,
                FieldChanged, previousFinancialTrackRecord.ToString(), request.FinancialTrackRecord.ToString());

            return await _updateOrganisationRepository.WriteFieldChangesToAuditLog(auditRecord);
        }
    }
}

