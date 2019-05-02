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

        public async Task<Guid?> CreateOrganisation(CreateOrganisationCommand command)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var startDate = command.StartDate;
                var organisationId = Guid.NewGuid();
                var createdAt = DateTime.Now;
                var createdBy = command.Username;
                var providerTypeId = command.ProviderTypeId;
                var organisationTypeId = command.OrganisationTypeId;
                var statusId = command.OrganisationStatusId;
                var organisationData = new OrganisationData
                {
                    CompanyNumber = command.CompanyNumber,
                    CharityNumber = command.CharityNumber,
                    ParentCompanyGuarantee = command.ParentCompanyGuarantee,
                    FinancialTrackRecord = command.FinancialTrackRecord,
                    NonLevyContract = command.NonLevyContract,
                    StartDate = startDate
                };

                string sql = $"INSERT INTO [dbo].[Organisations] " +
                             " ([Id] " +
                             ",[CreatedAt] " +
                             ",[CreatedBy] " +
                             ",[StatusId] " +
                             ",[ProviderTypeId] " +
                             ",[OrganisationTypeId] " +
                             ",[UKPRN] " +
                             ",[LegalName] " +
                             ",[TradingName] " +
                             ",[StatusDate] " +
                             ",[OrganisationData]) " +
                             "VALUES " +
                             "(@organisationId, @createdAt, @createdBy, @statusId, @providerTypeId, @organisationTypeId," +
                             " @ukprn, @legalName, @tradingName, @statusDate, @organisationData)";

                var organisationsCreated = await connection.ExecuteAsync(sql,
                    new
                    {
                        organisationId,
                        createdAt,
                        createdBy,
                        statusId,
                        providerTypeId,
                        organisationTypeId,
                        command.Ukprn,
                        command.LegalName,
                        command.TradingName,
                        command.StatusDate,
                        organisationData
                    });
                var success = await Task.FromResult(organisationsCreated > 0);

                if (success)
                    return organisationId;

                return null;
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

                int recordsAffected = await connection.ExecuteAsync(updateSql,
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
