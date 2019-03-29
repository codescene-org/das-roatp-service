namespace SFA.DAS.RoATPService.Data
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using Dapper;
    using Application.Interfaces;
    using Settings;
    using System.Threading.Tasks;

    public class UpdateOrganisationRepository : IUpdateOrganisationRepository
    {
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
    }
}
