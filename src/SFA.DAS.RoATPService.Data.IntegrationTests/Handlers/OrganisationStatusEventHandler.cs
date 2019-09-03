using System.Collections.Generic;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Handlers
{
    public static class OrganisationStatusEventHandler
    {

        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(OrganisationStatusEventModel organisationStatusEvent)
        {
            var sql =
                @"set identity_insert [OrganisationStatusEvent] ON;  INSERT INTO [OrganisationStatusEvent] (Id,[OrganisationStatusId],[CreatedOn],[ProviderId]) VALUES " +
                $@"(@Id,@OrganisationStatusId, @createdOn, @providerId); set identity_insert [OrganisationStatusEvent] OFF; ";

            DatabaseService.Execute(sql, organisationStatusEvent);
        }

        public static void InsertRecords(List<OrganisationStatusEventModel> organisationStatuses)
        {
            foreach (var orgStatus in organisationStatuses)
            {
                InsertRecord(orgStatus);
            }
        }

        public static OrganisationStatusEventModel GetOrganisationStatusFromId(int id)
        {
            var organisationStatusModel = DatabaseService.Get<OrganisationStatusEventModel>($@"select * from OrganisationStatusEvent where Id = {id}");
            return organisationStatusModel;
        }

        public static void DeleteRecord(int id)
        {
            var sql = $@"DELETE from OrganisationStatusEvent where id = {id}";
            DatabaseService.Execute(sql);
        }

        public static void DeleteRecords(List<int> ids)
        {
            foreach (var id in ids)
            {
                DeleteRecord(id);
            }
        }

        public static void DeleteAllRecords()
        {
            var sql = $@"DELETE from OrganisationStatusEvent";
            DatabaseService.Execute(sql);
        }
    }
}
