namespace SFA.DAS.RoATPService.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using Application.Interfaces;
    using AssessorService.Data.DapperTypeHandlers;
    using Dapper;
    using Domain;
    using Settings;

    public class OrganisationSearchRepository : IOrganisationSearchRepository
    {
        private IWebConfiguration _configuration;

        public OrganisationSearchRepository(IWebConfiguration configuration)
        {
            _configuration = configuration;
            SqlMapper.AddTypeHandler(typeof(OrganisationData), new OrganisationDataHandler());
        }

        public async Task<IEnumerable<Organisation>> OrganisationSearchByUkPrn(string ukPrn)
        {
            long ukPrnValue = Convert.ToInt64(ukPrn);

            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var organisations = await connection.QueryAsync<Organisation>($"select * from [Organisations] where UKPRN = {ukPrnValue}");
                return await Task.FromResult(organisations);
            }
         }

        public async Task<IEnumerable<Organisation>> OrganisationSearchByName(string organisationName)
        {
            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var organisations = await connection.QueryAsync<Organisation>($"select * from [Organisations] where LegalName LIKE '%{organisationName}%' OR TradingName LIKE '%{organisationName}%'");
                return await Task.FromResult(organisations);
            }
        }
    }
}
