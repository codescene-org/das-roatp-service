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

        public async Task<bool> CreateOrganisation(Organisation organisation, string username)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                Guid organisationId = Guid.NewGuid();
                DateTime createdAt = DateTime.Now;
                string createdBy = username;
                int providerTypeId = organisation.ProviderType.Id;
                int organisationTypeId = organisation.OrganisationType.Id;
                int statusId = organisation.OrganisationStatus.Id;

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
                        organisationId, createdAt, createdBy, statusId,
                        providerTypeId, organisationTypeId, organisation.UKPRN,
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
                int providerTypeId = organisation.ProviderType.Id;
                int organisationTypeId = organisation.OrganisationType.Id;
                Guid organisationId = organisation.Id;
                int statusId = organisation.OrganisationStatus.Id;

                string sql = $"UPDATE [Organisations] " +
                             "SET[UpdatedAt] = @updatedAt " +
                             ",[UpdatedBy] = @updatedBy " +
                             ",[StatusId] = @statusId " +
                             ",[ProviderTypeId] = @providerTypeId " +
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
                        statusId,
                        providerTypeId,
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
