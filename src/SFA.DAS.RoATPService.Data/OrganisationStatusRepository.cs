namespace SFA.DAS.RoATPService.Data
{
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using Application.Interfaces;
    using SFA.DAS.RoATPService.Domain;
    using SFA.DAS.RoATPService.Settings;

    public class OrganisationStatusRepository : IOrganisationStatusRepository
    {
        private IWebConfiguration _configuration;

        public OrganisationStatusRepository(IWebConfiguration configuration)
        {
            _configuration = configuration;
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

                var results = await connection.QueryAsync<OrganisationStatus>(sql, new {statusId});

                return await Task.FromResult(results.FirstOrDefault());
            }
        }
    }
}
