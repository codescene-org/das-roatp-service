namespace SFA.DAS.RoATPService.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Application.Interfaces;
    using AssessorService.Data.DapperTypeHandlers;
    using Dapper;
    using Domain;
    using KellermanSoftware.CompareNetObjects;
    using Settings;

    public class OrganisationRepository : IOrganisationRepository
    {
        private IWebConfiguration _configuration;

        public OrganisationRepository(IWebConfiguration configuration)
        {
            _configuration = configuration;
            SqlMapper.AddTypeHandler(typeof(OrganisationData), new OrganisationDataHandler());
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
            Organisation existingOrganisation = await GetOrganisation(organisation.Id);
            bool updateSuccess;

            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                updateSuccess = await UpdateOrganisationTable(organisation, username, connection);

                if (updateSuccess)
                {
                    updateSuccess = await UpdateAuditLog(existingOrganisation, organisation, connection);
                }
            }

            return await Task.FromResult(updateSuccess);
        }
        
        private static async Task<bool> UpdateOrganisationTable(Organisation organisation, string username, SqlConnection connection)
        {
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
                new {
                    updatedAt, updatedBy, organisation.Status, applicationRouteId, organisationTypeId,
                    organisation.UKPRN, organisation.LegalName, organisation.TradingName, organisation.StatusDate,
                    organisation.OrganisationData, organisationId
                });
            organisation.UpdatedAt = updatedAt;
            organisation.UpdatedBy = updatedBy;

            return await Task.FromResult(organisationsUpdated > 0);
        }
        
        private async Task<bool> UpdateAuditLog(Organisation originalOrganisation, Organisation updatedOrganisation, SqlConnection connection)
        {
            var auditLogEntries = await BuildAuditLogEntries(originalOrganisation, updatedOrganisation);

            int auditLogsWritten = 0;

            foreach (AuditLogEntry logEntry in auditLogEntries)
            {
                string sql = $"INSERT INTO Audit " +
                              "([OrganisationId], [UpdatedBy], [UpdatedAt], [FieldChanged], [PreviousValue], [NewValue]) " +
                              "VALUES(@organisationId, @updatedBy, @updatedAt, @fieldChanged, @previousValue, @newValue)";

                var recordsAffected = await connection.ExecuteAsync(sql,
                    new
                    {
                        logEntry.OrganisationId,
                        logEntry.UpdatedBy,
                        logEntry.UpdatedAt,
                        logEntry.FieldChanged,
                        logEntry.PreviousValue,
                        logEntry.NewValue
                    });
                auditLogsWritten += recordsAffected;
            }

            return await Task.FromResult(auditLogsWritten > 0);
        }

        private async Task<List<AuditLogEntry>> BuildAuditLogEntries(Organisation originalOrganisation, Organisation updatedOrganisation)
        {
            List<AuditLogEntry> auditLogEntries = new List<AuditLogEntry>();

            IEnumerable<AuditLogEntry> organisationLogEntries = await BuildListOfFieldsChanged<Organisation>(
                updatedOrganisation.Id, updatedOrganisation.UpdatedAt.Value, updatedOrganisation.UpdatedBy,
                originalOrganisation, updatedOrganisation);

            if (organisationLogEntries.Any())
            {
                auditLogEntries.AddRange(organisationLogEntries.ToList());
            }

            IEnumerable<AuditLogEntry> applicationRouteLogEntries = await BuildListOfFieldsChanged<ApplicationRoute>(
                updatedOrganisation.Id, updatedOrganisation.UpdatedAt.Value, updatedOrganisation.UpdatedBy,
                originalOrganisation.ApplicationRoute, updatedOrganisation.ApplicationRoute);

            if (applicationRouteLogEntries.Any())
            {
                auditLogEntries.AddRange(applicationRouteLogEntries.ToList());
            }

            IEnumerable<AuditLogEntry> organisationTypeLogEntries = await BuildListOfFieldsChanged<OrganisationType>(
                updatedOrganisation.Id, updatedOrganisation.UpdatedAt.Value, updatedOrganisation.UpdatedBy,
                originalOrganisation.OrganisationType, updatedOrganisation.OrganisationType);

            if (organisationTypeLogEntries.Any())
            {
                auditLogEntries.AddRange(organisationTypeLogEntries.ToList());
            }

            return auditLogEntries;
        }

        private async Task<IEnumerable<AuditLogEntry>> BuildListOfFieldsChanged<T>(Guid id, DateTime updatedAt, string updatedBy, T original, T updated)
        {
            CompareLogic organisationComparison = new CompareLogic(new ComparisonConfig
                {
                    CompareChildren = false,
                    MaxDifferences = byte.MaxValue
                }
            );
            ComparisonResult comparisonResult = organisationComparison.Compare(original, updated);
            List<AuditLogEntry> auditLogEntries = new List<AuditLogEntry>();
            foreach (var difference in comparisonResult.Differences)
            {
                PropertyInfo property = updated.GetType().GetProperty(difference.PropertyName);
                bool excluded = Attribute.IsDefined(property, typeof(ExcludeFromAuditLog));

                if (excluded)
                {
                    continue;
                }

                string propertyName = difference.PropertyName;
                if (Attribute.IsDefined(property, typeof(DisplayNameAttribute)))
                {
                    DisplayNameAttribute attribute = (DisplayNameAttribute)Attribute.GetCustomAttribute(property, typeof(DisplayNameAttribute));
                    if (!String.IsNullOrWhiteSpace(attribute.DisplayName))
                    {
                        propertyName = attribute.DisplayName;
                    }
                }

                AuditLogEntry entry = new AuditLogEntry
                {
                    OrganisationId = id,
                    FieldChanged = propertyName,
                    PreviousValue = difference.Object1Value,
                    NewValue = difference.Object2Value,
                    UpdatedAt = updatedAt,
                    UpdatedBy = updatedBy
                };
                auditLogEntries.Add(entry);
            }

            return await Task.FromResult(auditLogEntries);
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
    }
}
