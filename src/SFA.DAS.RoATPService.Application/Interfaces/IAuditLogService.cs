using System;

namespace SFA.DAS.RoATPService.Application.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain;

    public interface IAuditLogService
    {
        Task<AuditData> BuildListOfFieldsChanged(Organisation originalOrganisation, Organisation updatedOrganisation);

        AuditData CreateAuditLogEntry(Guid organisationId, string updatedBy, string fieldName,
            string oldValue, string newValue);

        AuditData CreateAuditData(Guid organisationId, string updatedBy);

        void AddAuditEntry(AuditData auditData, string fieldChanged, string previousValue, string newValue);
        AuditData AuditFinancialTrackRecord(Guid organisationId, string updatedBy, bool newFinancialTrackRecord);
    }
}
