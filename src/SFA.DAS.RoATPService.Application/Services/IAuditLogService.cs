using System;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Application.Services
{
    public interface IAuditLogService
    {
        AuditData CreateAuditLogEntry(Guid organisationId, string updatedBy, string fieldName,
            string oldValue, string newValue);

        AuditData CreateAuditData(Guid organisationId, string updatedBy);

        void AddAuditEntry(AuditData auditData, string fieldChanged, string previousValue, string newValue);
    }
}