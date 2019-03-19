using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Handlers
{
    public static class OrganisationStatusHandler
    {

        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(OrganisationStatusModel organisationStatus)
        {
            var sql =
                @"set identity_insert [OrganisationStatus] ON; INSERT INTO [OrganisationStatus] ([id], [Status],[CreatedAt],[CreatedBy] ,[UpdatedAt],[UpdatedBy]) VALUES " +
                $@"(@id,@status, @createdAt, @createdBy, @updatedAt, @updatedBy); set identity_insert [OrganisationStatus] OFF; ";

            DatabaseService.Execute(sql, organisationStatus);
        }

        public static void InsertRecords(List<OrganisationStatusModel> organisationStatuses)
        {
            foreach (var orgStatus in organisationStatuses)
            {
                InsertRecord(orgStatus);
            }
        }

        public static OrganisationStatusModel GetOrganisationStatusFromId(int id)
        {
            var organisationStatusModel = DatabaseService.Get<OrganisationStatusModel>($@"select * from OrganisationStatus where Id = {id}");
            return organisationStatusModel;
        }

        public static void DeleteRecord(int id)
        {
            var sql = $@"DELETE from OrganisationStatus where id = {id}";
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
            var sql = $@"DELETE from OrganisationStatus";
            DatabaseService.Execute(sql);
        }
    }
}
