namespace SFA.DAS.RoATPService.Application.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain;

    public interface IAuditLogRepository
    {
        Task<bool> WriteFieldChangesToAuditLog(IEnumerable<AuditLogEntry> auditLogEntries);
    }
}
