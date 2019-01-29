namespace SFA.DAS.RoATPService.Importer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using Dapper;

    public class RegisterImporter
    {
        public async Task<bool> ImportRegisterEntries(List<RegisterEntry> registerEntries)
        {
            TruncateRegisterTable();

            foreach (RegisterEntry entry in registerEntries)
            {
                await ImportRegisterEntry(entry);
            }

            return true;
        }

        private async void TruncateRegisterTable()
        {
            using (var connection =
                new SqlConnection(
                    "Data Source=localhost\\SQLEXPRESS;Initial Catalog=SFA.DAS.RoATPService.Database;Integrated Security=True;MultipleActiveResultSets=True;")
            )
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                
                string sql = "DELETE FROM [dbo].[Organisations]";

                var organisationsCreated = await connection.ExecuteAsync(sql);
                
            }

        }

        private async Task<bool> ImportRegisterEntry(RegisterEntry registerEntry)
        {
            using (var connection = new SqlConnection("Data Source=localhost\\SQLEXPRESS;Initial Catalog=SFA.DAS.RoATPService.Database;Integrated Security=True;MultipleActiveResultSets=True;"))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                Guid organisationId = Guid.NewGuid();
                DateTime createdAt = DateTime.Now;
                string createdBy = "Register Import";

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

                var status = registerEntry.StatusLongText;
                DateTime statusDate = DateTime.Now;
                var organisationData = "{}";

                var organisationsCreated = await connection.ExecuteAsync(sql,
                    new
                    {
                        organisationId,
                        createdAt,
                        createdBy,
                        status,
                        registerEntry.ApplicationRouteId,
                        registerEntry.OrganisationTypeId,
                        registerEntry.UKPRN,
                        registerEntry.LegalName,
                        registerEntry.TradingName,
                        statusDate,
                        organisationData
                    });
                return await Task.FromResult(organisationsCreated > 0);
            }
        }
    }
}
