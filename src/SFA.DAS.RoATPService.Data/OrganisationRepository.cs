using System.Collections.Generic;

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

        public async Task<OrganisationRegisterStatus> GetOrganisationRegisterStatus(string ukprn)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = "SELECT 1 FROM Organisations WHERE UKPRN = @ukprn";

                var ukPrnOnRegister = await connection.QueryAsync<bool>(sql, new {ukprn});

                if (!ukPrnOnRegister.FirstOrDefault())
                {
                    return new OrganisationRegisterStatus
                    {
                        UkprnOnRegister = false
                    };
                }

                sql = "SELECT 1 AS UkprnOnRegister, Id AS [OrganisationId], ProviderTypeId, StatusId, StatusDate, "
                    + "JSON_VALUE(OrganisationData, '$.RemovedReason.Id') AS RemovedReasonId FROM Organisations " +
                      "WHERE UKPRN = @ukprn";

                var registerStatus = await connection.QueryAsync<OrganisationRegisterStatus>(sql, new {ukprn});

                return registerStatus.FirstOrDefault();

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

        public async Task<IEnumerable<Engagement>> GetEngagements()
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))

            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = $@"select ProviderId, CreatedOn,
                                Case OrganisationStatusId
                                When 0 then 'REMOVED'
                                WHEN 1 then 'ACTIVE'
                                WHEN 2 then 'ACTIVENOSTARTS'
                                WHEN 3 then 'INITIATED'
                                else 'UNKNOWN'
                                End as Event
                                from organisationStatusEvent";
                return await connection.QueryAsync<Engagement>(sql);

            }
        }
    }
}
