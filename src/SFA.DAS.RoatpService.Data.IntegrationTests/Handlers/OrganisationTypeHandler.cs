using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Handlers
{
    public static class OrganisationTypeHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(OrganisationTypeModel organisationType)
        {
            var sql =
                @"set identity_insert [OrganisationTypes] ON; INSERT INTO [OrganisationTypes] ([id], [Type], [Description],[CreatedAt],[CreatedBy] ,[UpdatedAt],[UpdatedBy], [Status]) VALUES " +
                $@"(@id, @type, @description, @createdAt, @createdBy, @updatedAt, @updatedBy,@status); set identity_insert [OrganisationTypes] OFF; ";

            DatabaseService.Execute(sql, organisationType);
        }

        public static void InsertRecords(List<OrganisationTypeModel> organisationTypes)
        {
            foreach (var orgStatus in organisationTypes)
            {
                InsertRecord(orgStatus);
            }
        }

        public static OrganisationTypeModel GetOrganisationTypeFromId(int id)
        {
            var organisationTypeModel = DatabaseService.Get<OrganisationTypeModel>($@"select * from OrganisationTypes where Id = {id}");
            return organisationTypeModel;
        }

        public static void DeleteRecord(int id)
        {
            var sql = $@"DELETE from OrganisationTypes where id = {id}";
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
            var sql = $@"DELETE from OrganisationTypes";
            DatabaseService.Execute(sql);
        }
    }
}
