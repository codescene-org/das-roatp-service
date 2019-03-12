using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Handlers
{
    public static class OrganisationHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(OrganisationModel organisation)
        {
            var sql =
                @"INSERT INTO [Organisations] ([Id] ,[CreatedAt],[CreatedBy],[UpdatedAt],[UpdatedBy],[StatusId],[ProviderTypeId],[OrganisationTypeId],
                    [UKPRN],[LegalName],[TradingName],[StatusDate],[OrganisationData]) VALUES " +
                $@"(@id,@createdAt, @createdBy, @updatedAt, @updatedBy, @statusId, @providerTypeId, @organisationTypeId, 
                    @ukprn, @legalName, @tradingName,@statusDate,  @organisationData); ";

            DatabaseService.Execute(sql, organisation);
        }

        public static void InsertRecords(List<OrganisationModel> organisations)
        {
            foreach (var org in organisations)
            {
                InsertRecord(org);
            }
        }

        public static OrganisationModel GetOrganisationFromId(Guid organisationId)
        {
            var organisationModel = DatabaseService.Get<OrganisationModel>($@"select * from Organisations where Id = '{organisationId}'");
            return organisationModel;
        }

        public static void DeleteRecord(Guid id)
        {
            var idToDelete = SqlStringService.ConvertStringToSqlValueString(id.ToString());
            var sql = $@"DELETE from Organisations where id = {idToDelete}";
            DatabaseService.Execute(sql);
        }

        public static void DeleteRecords(List<Guid> ids)
        {
            foreach (var id in ids)
            {
                DeleteRecord(id);
            }
        }

        public static void DeleteAllRecords()
        {
            var sql = $@"DELETE from Organisations";
            DatabaseService.Execute(sql);
        }
    }
}
