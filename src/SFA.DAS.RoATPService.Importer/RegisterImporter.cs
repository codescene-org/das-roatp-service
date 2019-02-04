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
        private string ConnectionString { get; }

        public RegisterImporter(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public async Task<bool> ImportRegisterEntries(List<RegisterEntry> registerEntries)
        {
            TruncateRegisterTable();

            foreach (RegisterEntry entry in registerEntries)
            {
                try
                {
                    await ImportRegisterEntry(entry);
                }
                catch (SqlException sqlException)
                {
                    throw new RegisterImportException {UKPRN = entry.UKPRN, ImportErrorMessage = sqlException.Message};
                }
            }

            return true;
        }

        private async void TruncateRegisterTable()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                
                string sql = "DELETE FROM [dbo].[Organisations]";

                var organisationsCreated = await connection.ExecuteAsync(sql);
                
            }

        }

        private async Task<bool> ImportRegisterEntry(RegisterEntry registerEntry)
        {
            using (var connection = new SqlConnection(ConnectionString))
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
                             ",[StatusId] " +
                             ",[ProviderTypeId] " +
                             ",[OrganisationTypeId] " +
                             ",[UKPRN] " +
                             ",[LegalName] " +
                             ",[TradingName] " +
                             ",[StatusDate] " +
                             ",[OrganisationData]) " +
                             "VALUES " +
                             "(@organisationId, @createdAt, @createdBy, @statusId, @applicationRouteId, @organisationTypeId," +
                             " @ukprn, @legalName, @tradingName, @statusDate, @organisationData)";

                var statusId = registerEntry.Status;
                DateTime statusDate = DateTime.Now;
                var organisationData = "{}";

                var organisationsCreated = await connection.ExecuteAsync(sql,
                    new
                    {
                        organisationId,
                        createdAt,
                        createdBy,
                        statusId,
                        ApplicationRouteId = registerEntry.ProviderTypeId,
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
