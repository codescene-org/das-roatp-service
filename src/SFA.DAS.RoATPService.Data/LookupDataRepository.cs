using System;
using System.Linq;

namespace SFA.DAS.RoATPService.Data
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using Application.Interfaces;
    using Dapper;
    using Microsoft.Extensions.Logging;
    using Settings;
    using SFA.DAS.RoATPService.Domain;

    public class LookupDataRepository : ILookupDataRepository
    {
        private readonly IWebConfiguration _configuration;

        private ILogger<LookupDataRepository> _logger;

        public LookupDataRepository(ILogger<LookupDataRepository> logger, IWebConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IEnumerable<ProviderType>> GetProviderTypes()
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = $"select Id, ProviderType AS [Type], Description, " +
                          "CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, Status " +
                          "from [ProviderTypes] " +
                          "order by Id";

                var providerTypes = await connection.QueryAsync<ProviderType>(sql);
                return await Task.FromResult(providerTypes);
            }
        }

        public async Task<ProviderType> GetProviderType(int providerTypeId)
        {
            var types = await GetProviderTypes();
            return types.FirstOrDefault(x => x.Id == providerTypeId);
        }

        public async Task<IEnumerable<OrganisationType>> GetOrganisationTypes()
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = $"select ot.Id, ot.Type, ot.Description, ot.CreatedBy, ot.CreatedAt, ot.UpdatedBy, ot.UpdatedAt, ot.Status "
                          + "from [OrganisationTypes] ot " +
                          "order by Id";

                var organisationTypes = await connection.QueryAsync<OrganisationType>(sql);
                return await Task.FromResult(organisationTypes);
            }
        }

        public async Task<OrganisationType> GetOrganisationType(int organisationTypeId)
        {
            var types = await GetOrganisationTypes();
            return types.FirstOrDefault(x => x.Id == organisationTypeId);
        }


        public async Task<IEnumerable<OrganisationType>> GetOrganisationTypesForProviderTypeId(int? providerTypeId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = $"SELECT [Id], [Type], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [Status] " +
                          "FROM [dbo].[OrganisationTypes] ORDER BY Id";

                if (providerTypeId != null)
                {
                    sql =
                        $"select ot.Id, ot.Type, ot.Description, ot.CreatedBy, ot.CreatedAt, ot.UpdatedBy, ot.UpdatedAt, ot.Status "
                        + "from [OrganisationTypes] ot " +
                        "inner join [ProviderTypeOrganisationTypes] ptot " +
                        "on ptot.OrganisationTypeId = ot.Id " +
                        "and ptot.ProviderTypeId = @providerTypeId " +
                        "order by ot.Id";
                }

                var organisationTypes = await connection.QueryAsync<OrganisationType>(sql, new { providerTypeId });
                return await Task.FromResult(organisationTypes);
            }
        }

       

       

       
        
        public async Task<IEnumerable<OrganisationStatus>> GetOrganisationStatusesForProviderTypeId(int? providerTypeId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = $"SELECT [Id], [Status], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy] " +
                              "FROM [dbo].[OrganisationStatus] ORDER BY Id";

                if (providerTypeId != null)
                {
                    sql =
                        $@"SELECT os.[Id], os.[Status], os.[CreatedAt], os.[CreatedBy], os.[UpdatedAt], os.[UpdatedBy] 
                            FROM [dbo].[OrganisationStatus] os
                            inner join [ProviderTypeOrganisationStatus] ptos on os.Id = ptos.organisationStatusId
                            where ptos.providerTypeId = @providerTypeId
                            ORDER BY os.Id";
                }

                var organisationStatuses = await connection.QueryAsync<OrganisationStatus>(sql, new { providerTypeId });
                return await Task.FromResult(organisationStatuses);
            }
        }

        public async Task<IEnumerable<RemovedReason>> GetRemovedReasons()
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = "select Id, RemovedReason as [Reason], Status, Description, CreatedAt, " +
                          "CreatedBy, UpdatedAt, UpdatedBy FROM [RemovedReasons] " +
                          "ORDER BY Id";

                var removedReasons = await connection.QueryAsync<RemovedReason>(sql);
                return await Task.FromResult(removedReasons);
            }
        }

        public async Task<OrganisationStatus> GetOrganisationStatus(int statusId)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = "select * FROM [OrganisationStatus] " +
                          "WHERE Id = @statusId";

                var results = await connection.QueryAsync<OrganisationStatus>(sql, new { statusId });

                return await Task.FromResult(results.FirstOrDefault());
            }
        }

        public async Task<bool> IsOrganisationTypeValidForOrganisation(int organisationTypeId, Guid organisationId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = $@"SELECT case count(*)
                            when 0 then 0 else 1 end
                            FROM [ProviderTypeOrganisationTypes] ptot
                                 inner join organisations o on ptot.ProviderTypeId = o.ProviderTypeId
                            where o.Id = @organisationId
                            and ptot.OrganisationTypeId = @organisationTypeId";

                var organisationTypeValid = await connection.ExecuteScalarAsync<bool>(sql, new { organisationId, organisationTypeId });
                return await Task.FromResult(organisationTypeValid);
            }
        }

        public async Task<bool> IsOrganisationStatusValidForOrganisation(int organisationStatusId, Guid organisationId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = $@"SELECT case count(*)
                            when 0 then 0 else 1 end
                            FROM [ProviderTypeOrganisationStatus] ptos
                                 inner join organisations o on ptos.ProviderTypeId = o.ProviderTypeId
                            where o.Id = @organisationId
                            and ptos.OrganisationStatusId = @organisationStatusId";

                var organisationStatusValid = await connection.ExecuteScalarAsync<bool>(sql, new { organisationId, organisationStatusId });
                return await Task.FromResult(organisationStatusValid);
            }
        }
    }
}
