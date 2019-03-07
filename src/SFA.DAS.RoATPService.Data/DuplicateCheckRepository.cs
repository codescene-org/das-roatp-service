namespace SFA.DAS.RoATPService.Data
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using Application.Interfaces;
    using Dapper;
    using Microsoft.Extensions.Logging;
    using Settings;

    public class DuplicateCheckRepository : IDuplicateCheckRepository
    {
        private IWebConfiguration _configuration;
        private ILogger<DuplicateCheckRepository> _logger;

        public DuplicateCheckRepository(IWebConfiguration configuration, ILogger<DuplicateCheckRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> DuplicateUKPRNExists(Guid organisationId, long ukprn)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Organisations] " +
                          "WHERE UKPRN = @ukprn " +
                          "AND Id != @organisationId";
                return await connection.ExecuteScalarAsync<bool>(sql, new { organisationId, ukprn });               
            }
        }
        
        public async Task<bool> DuplicateCompanyNumberExists(Guid organisationId, string companyNumber)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Organisations] " +
                          "WHERE JSON_VALUE(OrganisationData, '$.CompanyNumber') = @companyNumber " +
                          "AND Id != @organisationId";
                return await connection.ExecuteScalarAsync<bool>(sql, new { organisationId, companyNumber });
            }
        }

        public async Task<bool> DuplicateCharityNumberExists(Guid organisationId, string charityNumber)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Organisations] " +
                          "WHERE JSON_VALUE(OrganisationData, '$.CharityNumber') = @charityNumber " +
                          "AND Id != @organisationId";
                return await connection.ExecuteScalarAsync<bool>(sql, new { organisationId, charityNumber });
            }
        }
    }
}
