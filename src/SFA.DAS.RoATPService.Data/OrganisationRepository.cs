namespace SFA.DAS.RoATPService.Data
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using Application.Interfaces;
    using AssessorService.Data.DapperTypeHandlers;
    using Dapper;
    using Domain;
    using Settings;

    public class OrganisationRepository : IOrganisationRepository
    {
        private IWebConfiguration _configuration;

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
                             "inner join ApplicationRoutes ao on o.ApplicationRouteId = ao.Id  " +
                             "inner join OrganisationTypes ot on o.OrganisationTypeId = ot.Id " +
                             "where o.Id = @organisationId";

                var organisations = await connection.QueryAsync<Organisation, ApplicationRoute, OrganisationType, Organisation>(sql, (org, route, type) => {
                        org.OrganisationType = type;
                        org.ApplicationRoute = route;
                        return org;
                    },
                    new { organisationId });
                return await Task.FromResult(organisations.FirstOrDefault());
            }
        }

        public async Task<bool> CreateOrganisation(Organisation organisation, string username)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                Guid organisationId = Guid.NewGuid();
                DateTime createdAt = DateTime.Now;
                string createdBy = username;
                int applicationRouteId = organisation.ApplicationRoute.Id;
                int organisationTypeId = organisation.OrganisationType.Id;

                string sql = $"INSERT INTO [dbo].[Organisations] " +
                    " ([Id] " +
                    ",[CreatedAt] " +
                    ",[CreatedBy] " +
                    ",[Status] " +
                    ",[ApplicationRouteId] " +
                    ",[OrganisationTypeId] " +
                    ",[UKPRN] " +
                    ",[LegalName] " +
                    ",[TradingName] " +
                    ",[StatusDate] " +
                    ",[OrganisationData]) " +
               "VALUES " +
                "(@organisationId, @createdAt, @createdBy, @status, @applicationRouteId, @organisationTypeId," +
                " @ukprn, @legalName, @tradingName, @statusDate, @organisationData)";

                var organisationsCreated = await connection.ExecuteAsync(sql,
                    new
                    {
                        organisationId, createdAt, createdBy, organisation.Status,
                        applicationRouteId, organisationTypeId, organisation.UKPRN,
                        organisation.LegalName, organisation.TradingName, organisation.StatusDate,
                        organisation.OrganisationData
                    });
                return await Task.FromResult(organisationsCreated > 0);
            }
        }

        public async Task<bool> UpdateOrganisation(Organisation organisation, string username)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();


                DateTime updatedAt = DateTime.Now;
                string updatedBy = username;
                int applicationRouteId = organisation.ApplicationRoute.Id;
                int organisationTypeId = organisation.OrganisationType.Id;
                Guid organisationId = organisation.Id;

                string sql = $"UPDATE [Organisations] " +
                             "SET[UpdatedAt] = @updatedAt " +
                             ",[UpdatedBy] = @updatedBy " +
                             ",[Status] = @status " +
                             ",[ApplicationRouteId] = @applicationRouteId " +
                             ",[OrganisationTypeId] = @organisationTypeId " +
                             ",[UKPRN] = @ukprn " +
                             ",[LegalName] = @legalName " +
                             ",[TradingName] = @tradingName " +
                             ",[StatusDate] = @statusDate " +
                             ",[OrganisationData] = @organisationData " +
                             "WHERE Id = @organisationId";

                var organisationsUpdated = await connection.ExecuteAsync(sql,
                    new
                    {
                        updatedAt,
                        updatedBy,
                        organisation.Status,
                        applicationRouteId,
                        organisationTypeId,
                        organisation.UKPRN,
                        organisation.LegalName,
                        organisation.TradingName,
                        organisation.StatusDate,
                        organisation.OrganisationData,
                        organisationId
                    });
                organisation.UpdatedAt = updatedAt;
                organisation.UpdatedBy = updatedBy;

                return await Task.FromResult(organisationsUpdated > 0);
            }
        }
    }
}
