namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System;
    using System.Collections.Generic;
    using Domain;

    public class UpdateOrganisationHandlerBase
    {
        protected AuditData CreateAuditLogEntry(Guid organisationId, string updatedBy, string fieldName, 
                                                string oldValue, string newValue)
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
    }
}
