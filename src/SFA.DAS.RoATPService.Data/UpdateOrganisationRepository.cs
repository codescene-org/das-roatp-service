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

        private readonly IWebConfiguration _configuration;

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

        public async Task<string> GetTradingName(Guid organisationId)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                const string sql = "select TradingName FROM [Organisations] " +
                                   "WHERE Id = @organisationId";
                return await connection.ExecuteScalarAsync<string>(sql, new { organisationId });
            }
        }

        public async Task<bool> UpdateTradingName(Guid organisationId, string tradingName, string updatedBy)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var updatedAt = DateTime.Now;

                const string sql = "update [Organisations] SET TradingName = @tradingName, UpdatedBy = @updatedBy, UpdatedAt = @updatedAt " +
                                   "WHERE Id = @organisationId";
                var recordsAffected = await connection.ExecuteAsync(sql, new { tradingName, updatedBy, updatedAt, organisationId });

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
                          "UpdatedBy = @updatedBy, UpdatedAt = @updatedAt, StatusDate = @updatedAt " +
                          "WHERE Id = @organisationId";
                int recordsAffected = await connection.ExecuteAsync(sql, new { organisationStatusId, updatedBy, updatedAt, organisationId });

                return await Task.FromResult(recordsAffected > 0);
            }
        }

        public async Task<bool> UpdateType(Guid organisationId, int organisationTypeId, string updatedBy)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var updatedAt = DateTime.Now;

                var sql = "update [Organisations] SET OrganisationTypeId = @organisationTypeId, " +
                          "UpdatedBy = @updatedBy, UpdatedAt = @updatedAt, StatusDate = @updatedAt " +
                          "WHERE Id = @organisationId";
                int recordsAffected = await connection.ExecuteAsync(sql, new { organisationTypeId, updatedBy, updatedAt, organisationId });

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
                    "StatusId = @organisationStatusId, UpdatedBy = @updatedBy, UpdatedAt = @updatedAt, StatusDate = @updatedAt " +
                    "WHERE Id = @organisationId";

                int recordsAffected = await connection.ExecuteAsync(updateSql,
                    new { reasonJson, organisationStatusId, updatedBy, updatedAt, organisationId });

                return await Task.FromResult(removedReason);
            }
        }

        public async Task<bool> GetParentCompanyGuarantee(Guid organisationId)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = $@"select CASE WHEN isnull(JSON_Value(OrganisationData,'$.ParentCompanyGuarantee'),'false') = 'false'
                                    THEN 0
                                    ELSE 1
                                    END
                                    FROM[Organisations] " +
                                   "WHERE Id = @organisationId";
                return await connection.ExecuteScalarAsync<bool>(sql, new { organisationId });
            }
        }

        public async Task<bool> UpdateParentCompanyGuarantee(Guid organisationId, bool parentCompanyGuarantee, string updatedBy)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var updatedAt = DateTime.Now;

                var sql = "update [Organisations] SET OrganisationData = JSON_MODIFY(OrganisationData,'$.ParentCompanyGuarantee',@parentCompanyGuarantee), UpdatedBy = @updatedBy, UpdatedAt = @updatedAt " +
                          "WHERE Id = @organisationId";
                int recordsAffected = await connection.ExecuteAsync(sql, new { parentCompanyGuarantee, updatedBy, updatedAt, organisationId });

                return await Task.FromResult(recordsAffected > 0);
            }
        }

        public async Task<bool> GetFinancialTrackRecord(Guid organisationId)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = $@"select CASE WHEN isnull(JSON_Value(OrganisationData,'$.FinancialTrackRecord'),'false') = 'false'
                                    THEN 0
                                    ELSE 1
                                    END
                                    FROM[Organisations] " +
                          "WHERE Id = @organisationId";
                return await connection.ExecuteScalarAsync<bool>(sql, new { organisationId });
            }
        }

        public async Task<bool> UpdateFinancialTrackRecord(Guid organisationId, bool financialTrackRecord, string updatedBy)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var updatedAt = DateTime.Now;

                const string sql = "update [Organisations] SET OrganisationData = JSON_MODIFY(OrganisationData,'$.FinancialTrackRecord',@financialTrackRecord), UpdatedBy = @updatedBy, UpdatedAt = @updatedAt " +
                                   "WHERE Id = @organisationId";
                var recordsAffected = await connection.ExecuteAsync(sql, new { financialTrackRecord, updatedBy, updatedAt, organisationId });

                return await Task.FromResult(recordsAffected > 0);
            }
        }

        public async Task<int> GetProviderType(Guid organisationId)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                const string sql = "SELECT ProviderTypeId FROM [Organisations] WHERE Id = @organisationId";
                return await connection.ExecuteScalarAsync<int>(sql, new { organisationId });
            }
        }
        
                public async Task<bool> UpdateUkprn(Guid organisationId, long ukprn, string updatedBy) 
		{
			var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
				var updatedAt = DateTime.Now;

                var sql = "update [Organisations] SET UKPRN = @ukprn, UpdatedBy = @updatedBy, UpdatedAt = @updatedAt " +
                          "WHERE Id = @organisationId";
                int recordsAffected = await connection.ExecuteAsync(sql, new { ukprn, updatedBy, updatedAt, organisationId });

                return await Task.FromResult(recordsAffected > 0);
			}
		}
		
        
                public async Task<DateTime?> GetStartDate(Guid organisationId)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                const string sql = "SELECT Json_value(organisationData,'$.StartDate') FROM [Organisations] WHERE Id = @organisationId";
                return await connection.ExecuteScalarAsync<DateTime?>(sql, new { organisationId });
            }
        }
        
		

        public async Task<int> GetOrganisationType(Guid organisationId)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                const string sql = "SELECT OrganisationTypeId FROM [Organisations] WHERE Id = @organisationId";
                return await connection.ExecuteScalarAsync<int>(sql, new { organisationId });
            }
        }

        public async Task<bool> UpdateProviderTypeAndOrganisationType(Guid organisationId, int providerTypeId, int organisationTypeId, string updatedBy)
		{
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();


                var updatedAt = DateTime.Now;

                const string sql = "update [Organisations] SET ProviderTypeId = @providerTypeId, " +
                          "OrganisationTypeId = @organisationTypeId, " +
                          "UpdatedBy = @updatedBy, UpdatedAt = @updatedAt " +
                          "WHERE Id = @organisationId";
                int recordsAffected = await connection.ExecuteAsync(sql, new { providerTypeId, organisationTypeId, updatedBy, updatedAt, organisationId });

                return await Task.FromResult(recordsAffected > 0);
            }
        }

        public async Task<long> GetUkprn(Guid organisationId)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                
                var sql = "select ukprn FROM [Organisations] " +
                          "WHERE Id = @organisationId";
						  
                return await connection.ExecuteScalarAsync<long>(sql, new { organisationId });
            }
        }
    }
}
