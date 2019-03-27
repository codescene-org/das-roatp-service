namespace SFA.DAS.RoATPService.Data
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using Dapper;
    using Application.Interfaces;
    using Settings;
    using System.Threading.Tasks;
    using Domain;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class UpdateOrganisationRepository : IUpdateOrganisationRepository
    {
        private const string RoatpDateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        private IWebConfiguration _configuration;

        public UpdateOrganisationRepository(IWebConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GetLegalName(Guid organisationId)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = "select LegalName FROM [Organisations] " +
                          "WHERE Id = @organisationId";
                return await connection.ExecuteScalarAsync<string>(sql, new { organisationId });
            }
        }
        
        public async Task<bool> UpdateLegalName(Guid organisationId, string legalName, string updatedBy)
        {
            var connectionString = _configuration.SqlConnectionString;
            
            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var updatedAt = DateTime.Now;

                var sql = "update [Organisations] SET LegalName = @legalName, UpdatedBy = @updatedBy, UpdatedAt = @updatedAt " +
                          "WHERE Id = @organisationId";
                int recordsAffected = await connection.ExecuteAsync(sql, new { legalName, updatedBy, updatedAt, organisationId });

                return await Task.FromResult(recordsAffected > 0);
            }
        }
        
        public async Task<int> GetStatus(Guid organisationId)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = "select StatusId FROM [Organisations] " +
                          "WHERE Id = @organisationId";
                return await connection.ExecuteScalarAsync<int>(sql, new { organisationId });
            }
        }

        public async Task<RemovedReason> GetRemovedReason(Guid organisationId)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = "select JSON_QUERY(OrganisationData, '$.RemovedReason') FROM [Organisations] " +
                          "WHERE Id = @organisationId";
                var results = await connection.QueryAsync<string>(sql, new { organisationId });
                var resultJson = results.FirstOrDefault();
                if (resultJson == null)
                {
                    RemovedReason nullResult = null;
                    return await Task.FromResult(nullResult);
                }

                return JsonConvert.DeserializeObject<RemovedReason>(resultJson);
            }
        }

        public async Task<bool> UpdateStartDate(Guid organisationId, DateTime startDate)
        {
            var connectionString = _configuration.SqlConnectionString;

            string startDateValue = startDate.ToString(RoatpDateTimeFormat);

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var updateSql =
                    "update [Organisations] set OrganisationData = JSON_MODIFY(OrganisationData, '$.StartDate', @startDateValue) " +
                    "WHERE Id = @organisationId";

                int recordsAffected = await connection.ExecuteAsync(updateSql, new { startDateValue, organisationId });

                return await Task.FromResult(recordsAffected > 0);
            }
        }

        public async Task<bool> UpdateStatus(Guid organisationId, int organisationStatusId, string updatedBy)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var updatedAt = DateTime.Now;

                var sql = "update [Organisations] SET StatusId = @organisationStatusId, " + 
                          "OrganisationData = JSON_MODIFY(OrganisationData, '$.RemovedReason', null), " +
                          "UpdatedBy = @updatedBy, UpdatedAt = @updatedAt " +
                          "WHERE Id = @organisationId";
                int recordsAffected = await connection.ExecuteAsync(sql, new { organisationStatusId, updatedBy, updatedAt, organisationId });

                return await Task.FromResult(recordsAffected > 0);
            }
        }

        public async Task<RemovedReason> UpdateStatusWithRemovedReason(Guid organisationId, int organisationStatusId, int removedReasonId, string updatedBy)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = "select Id, RemovedReason as [Reason], Status, Description, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy FROM [RemovedReasons] " +
                          "WHERE Id = @removedReasonId";
                var reason = await connection.QueryAsync<RemovedReason>(sql, new { removedReasonId });
                var removedReason = reason.FirstOrDefault();

                var reasonJson = JsonConvert.SerializeObject(removedReason,
                    new IsoDateTimeConverter() { DateTimeFormat = RoatpDateTimeFormat });

                var updatedAt = DateTime.Now;

                var updateSql =
                    "update [Organisations] set OrganisationData = JSON_MODIFY(OrganisationData, '$.RemovedReason', JSON_QUERY(@reasonJson)), " +
                    "StatusId = @organisationStatusId, UpdatedBy = @updatedBy, UpdatedAt = @updatedAt " +
                    "WHERE Id = @organisationId";

                int recordsAffected = await connection.ExecuteAsync(updateSql,
                    new { reasonJson, organisationStatusId, updatedBy, updatedAt, organisationId });

                return await Task.FromResult(removedReason);
            }
        }
    }
}
