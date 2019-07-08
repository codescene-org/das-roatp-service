namespace SFA.DAS.RoATPService.Data
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using Application.Interfaces;
    using Dapper;
    using Domain;
    using Settings;
    using RoatpService.Data.DapperTypeHandlers;
    using Newtonsoft.Json;

    public class OrganisationRepository : IOrganisationRepository
    {
        private readonly IWebConfiguration _configuration;

        public OrganisationRepository(IWebConfiguration configuration)
        {
            _configuration = configuration;
            SqlMapper.AddTypeHandler(typeof(OrganisationData), new OrganisationDataHandler());
        }

        public async Task<Organisation> GetOrganisation(Guid organisationId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                string sql = $"select * from [Organisations] o " +
                             "inner join ProviderTypes pt on o.ProviderTypeId = pt.Id  " +
                             "inner join OrganisationTypes ot on o.OrganisationTypeId = ot.Id " +
                             "inner join OrganisationStatus os on o.StatusId = os.Id " +
                             "where o.Id = @organisationId";

                var organisations = await connection.QueryAsync<Organisation, ProviderType, OrganisationType, 
                                                                OrganisationStatus, Organisation>
                    (sql, (org, providerType, type, status) => {
                        org.OrganisationType = type;
                        org.ProviderType = providerType;
                        org.OrganisationStatus = status;
                        return org;
                    },
                    new { organisationId });
                return await Task.FromResult(organisations.FirstOrDefault());
            }
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

        public async Task<int> GetOrganisationStatus(Guid organisationId)
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

        public async Task<string> GetCompanyNumber(Guid organisationId)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                const string sql = "SELECT Json_value(organisationData,'$.CompanyNumber') FROM [Organisations] WHERE Id = @organisationId";
                return await connection.ExecuteScalarAsync<string>(sql, new { organisationId });
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

        public async Task<string> GetCharityNumber(Guid organisationId)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                const string sql = "SELECT Json_value(organisationData,'$.CharityNumber') FROM [Organisations] WHERE Id = @organisationId";
                return await connection.ExecuteScalarAsync<string>(sql, new { organisationId });
            }
        }


        public async Task<OrganisationReapplyStatus> GetOrganisationReapplyStatus(Guid organisationId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = "SELECT ProviderTypeId, StatusId FROM Organisations " +
                          "WHERE Id = @organisationId";

                var reapplyStatus = await connection.QueryAsync<OrganisationReapplyStatus>(sql, new {organisationId});

                return reapplyStatus.FirstOrDefault();

             }
        }
                
        public async Task<DateTime?> GetApplicationDeterminedDate(Guid organisationId)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))

            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                const string sql = "SELECT Json_value(organisationData,'$.ApplicationDeterminedDate') FROM [Organisations] WHERE Id = @organisationId";
                return await connection.ExecuteScalarAsync<DateTime?>(sql, new { organisationId });

            }
        }
    }
}
