namespace SFA.DAS.RoATPService.Data
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using Application.Interfaces;
    using Dapper;
    using Microsoft.Extensions.Logging;
    using Settings;
    using SFA.DAS.RoATPService.Api.Types.Models;

    public class DuplicateCheckRepository : IDuplicateCheckRepository
    {
        private IWebConfiguration _configuration;
        private ILogger<DuplicateCheckRepository> _logger;

        public DuplicateCheckRepository(IWebConfiguration configuration, ILogger<DuplicateCheckRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<DuplicateCheckResponse> DuplicateUKPRNExists(Guid organisationId, long ukprn)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = "select LegalName AS DuplicateOrganisationName, " +
                          "CASE WHEN LegalName IS NOT NULL THEN 1 ELSE 0 END AS DuplicateFound, " +
                          "Id AS DuplicateOrganisationId " +
                          "FROM [Organisations] " +
                          "WHERE UKPRN = @ukprn " +
                          "AND Id != @organisationId";
                var results = await connection.QueryAsync<DuplicateCheckResponse>(sql, new { organisationId, ukprn });

                var duplicate = results.FirstOrDefault();
                if (duplicate == null)
                {
                    return new DuplicateCheckResponse {DuplicateFound = false};
                }

                return duplicate;
            }
        }

        public async Task<DuplicateCheckResponse> DuplicateCompanyNumberExists(Guid organisationId, string companyNumber)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = "select LegalName FROM [Organisations] " +
                          "WHERE JSON_VALUE(OrganisationData, '$.CompanyNumber') = @companyNumber " +
                          "AND Id != @organisationId";
                string duplicateLegalName = await connection.ExecuteScalarAsync<string>(sql, new { organisationId, companyNumber });

                DuplicateCheckResponse response = new DuplicateCheckResponse
                {
                    DuplicateFound = !String.IsNullOrWhiteSpace(duplicateLegalName),
                    DuplicateOrganisationName = duplicateLegalName
                };
                return response;
            }
        }

        public async Task<DuplicateCheckResponse> DuplicateCharityNumberExists(Guid organisationId, string charityNumber)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = "select LegalName FROM [Organisations] " +
                          "WHERE JSON_VALUE(OrganisationData, '$.CharityNumber') = @charityNumber " +
                          "AND Id != @organisationId";
                string duplicateLegalName = await connection.ExecuteScalarAsync<string>(sql, new { organisationId, charityNumber });

                DuplicateCheckResponse response = new DuplicateCheckResponse
                {
                    DuplicateFound = !String.IsNullOrWhiteSpace(duplicateLegalName),
                    DuplicateOrganisationName = duplicateLegalName
                };
                return response;
            }
        }
    }
}
