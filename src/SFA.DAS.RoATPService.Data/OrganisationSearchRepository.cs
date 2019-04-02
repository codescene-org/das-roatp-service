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
    using Microsoft.Extensions.Configuration;
    using Settings;
    using SFA.DAS.RoatpService.Data.DapperTypeHandlers;
    using SFA.DAS.RoATPService.Api.Types.Models;

    public class OrganisationSearchRepository : IOrganisationSearchRepository
    {
        private IWebConfiguration _webConfiguration;
        private IConfiguration _appConfiguration;

        public OrganisationSearchRepository(IWebConfiguration webConfiguration, IConfiguration appConfiguration)
        {
            _webConfiguration = webConfiguration;
            _appConfiguration = appConfiguration;
            SqlMapper.AddTypeHandler(typeof(OrganisationData), new OrganisationDataHandler());
        }

        public async Task<OrganisationSearchResults> OrganisationSearchByUkPrn(string ukPrn)
        {
            long ukPrnValue = Convert.ToInt64(ukPrn);

            var connectionString = _webConfiguration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = $"select * from [Organisations] o " +
                          "inner join ProviderTypes pt on o.ProviderTypeId = pt.Id  " +
                          "inner join OrganisationTypes ot on o.OrganisationTypeId = ot.Id " +
                          "inner join OrganisationStatus os on o.StatusId = os.Id " +
                          "where UKPRN = @ukPrnValue";

                var organisations = await connection.QueryAsync<Organisation, ProviderType, OrganisationType,
                    OrganisationStatus, Organisation>
                     (sql, (org, providerType, type, status) => {
                        org.OrganisationType = type;
                        org.ProviderType = providerType;
                        org.OrganisationStatus = status;
                        return org;
                    },
                    new {ukPrnValue});

                var searchResults = new OrganisationSearchResults
                {
                    SearchResults = organisations,
                    TotalCount = organisations.Count()
                };
                return await Task.FromResult(searchResults);
            }
         }

        public async Task<OrganisationSearchResults> OrganisationSearchByName(string organisationName)
        {
            int rowLimit = 5;
            int.TryParse(_appConfiguration["OrganisationSearchResultsLimit"], out rowLimit);

            var connectionString = _webConfiguration.SqlConnectionString;

            var organisationNameFilter = $"%{organisationName}%";
       
            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = $"select top (@rowLimit) * from [Organisations] o " +
                            "inner join ProviderTypes pt on o.ProviderTypeId = pt.Id " +
                            "inner join OrganisationTypes ot on o.OrganisationTypeId = ot.Id " +
                            "inner join OrganisationStatus os on o.StatusId = os.Id " +
                            "where o.LegalName LIKE @organisationNameFilter " +
                            "order by legalname asc; " +
                            "select count(*) from[Organisations] " +
                            "where LegalName like @organisationNameFilter";
                var searchQuery = await connection.QueryMultipleAsync
                    (sql, new { rowLimit, organisationNameFilter });

                var results =
                    searchQuery.Read<Organisation, ProviderType, OrganisationType, OrganisationStatus, Organisation>(
                        (org, providerType, type, status) =>
                        {
                            org.OrganisationType = type;
                            org.ProviderType = providerType;
                            org.OrganisationStatus = status;
                            return org;
                        });

                var resultCount = searchQuery.ReadFirst<int>();

                var searchResult = new OrganisationSearchResults
                {
                    SearchResults = results,
                    TotalCount = resultCount
                };

                return await Task.FromResult(searchResult);
            }
        }
    }
}
