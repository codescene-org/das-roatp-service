namespace SFA.DAS.RoATPService.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using Application.Interfaces;
    using Dapper;
    using Domain;
    using Settings;

    public class AuditLogRepository : IAuditLogRepository
    {
        public IWebConfiguration _configuration;

        public AuditLogRepository(IWebConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> WriteFieldChangesToAuditLog(IEnumerable<AuditLogEntry> auditLogEntries)
        {
            if (!auditLogEntries.Any())
            {
                return await Task.FromResult(false);
            }

            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                
                int auditLogsWritten = 0;

                foreach (AuditLogEntry logEntry in auditLogEntries)
                {
                    string sql = $"INSERT INTO Audit " +
                                 "([OrganisationId], [UpdatedBy], [UpdatedAt], [FieldChanged], [PreviousValue], [NewValue]) " +
                                 "VALUES(@organisationId, @updatedBy, @updatedAt, @fieldChanged, @previousValue, @newValue)";
                    
                    var updatedAt = DateTime.Now;

                    var recordsAffected = await connection.ExecuteAsync(sql,
                        new
                        {
                            logEntry.OrganisationId,
                            logEntry.UpdatedBy,
                            updatedAt,
                            logEntry.FieldChanged,
                            logEntry.PreviousValue,
                            logEntry.NewValue
                        });
                    auditLogsWritten += recordsAffected;
                }

                return await Task.FromResult(auditLogsWritten > 0);
            }
        }
    }
}
