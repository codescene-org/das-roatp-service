using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Application.Services
{
    public class AuditLogService: IAuditLogService
    {
        public AuditData CreateAuditLogEntry(Guid organisationId, string updatedBy, string fieldName, string oldValue,
            string newValue)
        {
            return new AuditData
            {
                OrganisationId = organisationId,
                UpdatedAt = DateTime.Now,
                UpdatedBy = updatedBy,
                FieldChanges = new List<AuditLogEntry>
                {
                    new AuditLogEntry
                    {
                        FieldChanged = fieldName,
                        NewValue = newValue,
                        PreviousValue = oldValue
                    }
                }
            };
        }

        public AuditData CreateAuditData(Guid organisationId, string updatedBy)
        {
            return new AuditData
            {
                FieldChanges = new List<AuditLogEntry>(),
                OrganisationId = organisationId,
                UpdatedAt = DateTime.Now,
                UpdatedBy = updatedBy
            };
        }

        public void AddAuditEntry(AuditData auditData, string fieldChanged, string previousValue, string newValue)
        {
            var entry = new AuditLogEntry
            {
                FieldChanged = fieldChanged,
                PreviousValue = previousValue,
                NewValue = newValue
            };

            auditData.FieldChanges.Add(entry);
        }
    }
}
