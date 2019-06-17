using SFA.DAS.RoatpService.Data.DapperTypeHandlers;
using SFA.DAS.RoATPService.Application.Commands;

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
            SqlMapper.AddTypeHandler(typeof(OrganisationData), new OrganisationDataHandler());
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

        public async Task<bool> UpdateStartDate(Guid organisationId, DateTime startDate, string updatedBy)
        {
            var connectionString = _configuration.SqlConnectionString;

            var startDateValue = startDate.ToString(RoatpDateTimeFormat);

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var updatedAt = DateTime.Now;

                var updateSql =
                    "update [Organisations] set OrganisationData = JSON_MODIFY(OrganisationData, '$.StartDate', @startDateValue), UpdatedBy = @updatedBy, UpdatedAt = @updatedAt " +
                    "WHERE Id = @organisationId";

                int recordsAffected = await connection.ExecuteAsync(updateSql, new { startDateValue, organisationId, updatedBy, updatedAt });

                return await Task.FromResult(recordsAffected > 0);
            }
        }

        public async Task<bool> UpdateOrganisationStatus(Guid organisationId, int organisationStatusId, string updatedBy)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var updatedAt = DateTime.Now;

                var sql = "update [Organisations] SET StatusId = @organisationStatusId, " +    
                          "UpdatedBy = @updatedBy, UpdatedAt = @updatedAt, StatusDate = @updatedAt " +
                          "WHERE Id = @organisationId";
                int recordsAffected = await connection.ExecuteAsync(sql, new { organisationStatusId, updatedBy, updatedAt, organisationId });

                return await Task.FromResult(recordsAffected > 0);
            }
        }

        public async Task<bool> UpdateProviderType(Guid organisationId, int providerTypeId, string updatedBy)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var updatedAt = DateTime.Now;

                var sql = "update [Organisations] SET ProviderTypeId = @providerTypeId, " +
                          "UpdatedBy = @updatedBy, UpdatedAt = @updatedAt, StatusDate = @updatedAt " +
                          "WHERE Id = @organisationId";
                int recordsAffected = await connection.ExecuteAsync(sql, new { providerTypeId, updatedBy, updatedAt, organisationId });

                return await Task.FromResult(recordsAffected > 0);
            }
        }

        public async Task<bool> UpdateOrganisationType(Guid organisationId, int organisationTypeId, string updatedBy)
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

                var recordsAffected = await connection.ExecuteAsync(updateSql,
                    new { reasonJson, organisationStatusId, updatedBy, updatedAt, organisationId });

                return await Task.FromResult(removedReason);
            }
        }

        public async Task<bool> UpdateRemovedReason(Guid organisationId, int? removedReasonId, string updatedBy)
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

                if (removedReason == null)
                    reasonJson = null;
                var updatedAt = DateTime.Now;

                var updateSql =
                    "update [Organisations] set OrganisationData = JSON_MODIFY(OrganisationData, '$.RemovedReason', JSON_QUERY(@reasonJson)), " +
                    " UpdatedBy = @updatedBy, UpdatedAt = @updatedAt, StatusDate = @updatedAt " +
                    "WHERE Id = @organisationId";

                var recordsAffected = await connection.ExecuteAsync(updateSql,
                    new { reasonJson, updatedBy, updatedAt, organisationId });

                return await Task.FromResult(recordsAffected > 0);
            }
        }

        public async Task<bool> UpdateCompanyNumber(Guid organisationId, string companyNumber, string updatedBy)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var updatedAt = DateTime.Now;

                var sql = "update [Organisations] SET OrganisationData = JSON_MODIFY(OrganisationData,'$.CompanyNumber',@companyNumber), UpdatedBy = @updatedBy, UpdatedAt = @updatedAt " +
                          "WHERE Id = @organisationId";
                int recordsAffected = await connection.ExecuteAsync(sql, new { companyNumber, updatedBy, updatedAt, organisationId });

                return await Task.FromResult(recordsAffected > 0);
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

        public async Task<bool> UpdateCharityNumber(Guid organisationId, string charityNumber, string updatedBy)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var updatedAt = DateTime.Now;

                var sql = "update [Organisations] SET OrganisationData = JSON_MODIFY(OrganisationData,'$.CharityNumber',@CharityNumber), UpdatedBy = @updatedBy, UpdatedAt = @updatedAt " +
                          "WHERE Id = @organisationId";
                int recordsAffected = await connection.ExecuteAsync(sql, new { charityNumber, updatedBy, updatedAt, organisationId });

                return await Task.FromResult(recordsAffected > 0);
            }
        }

        public async Task<bool> UpdateApplicationDeterminedDate(Guid organisationId, DateTime applicationDeterminedDate, string updatedBy)
        {
            var connectionString = _configuration.SqlConnectionString;

            var applicationDeterminedDateValue = applicationDeterminedDate.ToString(RoatpDateTimeFormat);

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var updatedAt = DateTime.Now;

                var updateSql =
                    "update [Organisations] set OrganisationData = JSON_MODIFY(OrganisationData, '$.applicationDeterminedDate', @applicationDeterminedDateValue), UpdatedBy = @updatedBy, UpdatedAt = @updatedAt " +
                    "WHERE Id = @organisationId";

                int recordsAffected = await connection.ExecuteAsync(updateSql, new { applicationDeterminedDateValue = applicationDeterminedDateValue, organisationId, updatedBy, updatedAt });

                return await Task.FromResult(recordsAffected > 0);
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
