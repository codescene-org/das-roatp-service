namespace SFA.DAS.RoATPService.Importer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using Dapper;
    using Loggers;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.RoATPService.Importer.Exceptions;
    using SFA.DAS.RoATPService.Importer.Models;

    public class RegisterImporter : IRegisterImporter
    {
        private string ConnectionString { get; set; }

        private ILogger<RegisterImporter> Logger { get; }

        public RegisterImporter(ILogger<RegisterImporter> logger)
        {
            Logger = logger;
        }

        public async Task<bool> ImportRegisterEntries(string connectionString, List<RegisterEntry> registerEntries)
        {
            ConnectionString = connectionString;

            using (var connection = await TruncateRegisterTable())
            {
                foreach (RegisterEntry entry in registerEntries)
                {
                    try
                    {
                        await ImportRegisterEntry(connection, entry);
                    }
                    catch (SqlException sqlException)
                    {
                        string databaseErrorMessage =
                            $"Unable to import register data for UKPRN {entry.UKPRN} : SQL operation failed";
                        Logger.LogError(sqlException, databaseErrorMessage);
                        throw new RegisterImportException("Unable to import register data", sqlException)
                        {
                            UKPRN = entry.UKPRN,
                            ImportErrorMessage = "SQL operation failed"
                        };
                    }
                }
            }
            return true;
        }

        private async Task<IDbConnection> TruncateRegisterTable()
        {
            var connection = new SqlConnection(ConnectionString);
            
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();
            
            string sql = "DELETE FROM [dbo].[Organisations]";

            await connection.ExecuteAsync(sql);

            RegisterImportLogger.Instance.LogStatement(sql);

            return connection;
        }

        private async Task<bool> ImportRegisterEntry(IDbConnection connection, RegisterEntry registerEntry)
        {
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
                         "(@organisationId, @createdAt, @createdBy, @statusId, @providerTypeId, @organisationTypeId," +
                         " @ukprn, @legalName, @tradingName, @statusDate, @organisationData)";

            var statusId = registerEntry.Status;
            var organisationData = "{}";

            var organisationsCreated = await connection.ExecuteAsync(sql,
                new
                {
                    organisationId,
                    createdAt,
                    createdBy,
                    statusId,
                    registerEntry.ProviderTypeId,
                    registerEntry.OrganisationTypeId,
                    registerEntry.UKPRN,
                    registerEntry.LegalName,
                    registerEntry.TradingName,
                    registerEntry.StatusDate,
                    organisationData
                });

            RegisterImportLogger.Instance.LogInsertStatement(sql, registerEntry, organisationId, createdAt, 
                                                             createdBy, statusId, organisationData);

            return await Task.FromResult(organisationsCreated > 0);
        }
    }
}
