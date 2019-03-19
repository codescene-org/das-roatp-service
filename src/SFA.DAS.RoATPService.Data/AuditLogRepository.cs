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
    using Newtonsoft.Json;
    using Settings;

    public class AuditLogRepository : IAuditLogRepository
    {
        public IWebConfiguration _configuration;

        public AuditLogRepository(IWebConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> WriteFieldChangesToAuditLog(AuditData auditFieldChanges)
        {
            if (!auditFieldChanges.FieldChanges.Any())
            {
                return await Task.FromResult(false);
            }

            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
            
                string sql = $"INSERT INTO Audit " +
                             "([OrganisationId], [UpdatedBy], [UpdatedAt], [AuditData]) " +
                             "VALUES(@organisationId, @updatedBy, @updatedAt, @auditData)";
                
                var updatedAt = DateTime.Now;
                var auditData = JsonConvert.SerializeObject(auditFieldChanges);
                var recordsAffected = await connection.ExecuteAsync(sql,
                    new
                    {
                        auditFieldChanges.OrganisationId,
                        auditFieldChanges.UpdatedBy,
                        updatedAt,
                        auditData
                    });
                 
                return await Task.FromResult(recordsAffected > 0);
            }
        }
    }
}
