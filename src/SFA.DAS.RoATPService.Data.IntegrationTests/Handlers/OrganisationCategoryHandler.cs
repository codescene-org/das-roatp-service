using System.Collections.Generic;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Handlers
{
    public static class OrganisationCategoryHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(OrganisationCategoryModel organisationCategory)
        {
            var sql =
                @"set identity_insert [OrganisationCategory] ON; INSERT INTO [OrganisationCategory] (Id,[Category]
           ,[CreatedAt],[CreatedBy],[UpdatedAt],[UpdatedBy],[Status]) VALUES " +
                $@"(@id, @category, @createdAt, @createdBy, @updatedAt, @updatedBy,@status); set identity_insert [OrganisationCategory] OFF; ";

            DatabaseService.Execute(sql, organisationCategory);
        }

        public static void InsertRecords(List<OrganisationCategoryModel> organisationCategories)
        {
            foreach (var orgCategory in organisationCategories)
            {
                InsertRecord(orgCategory);
            }
        }

        public static OrganisationCategoryModel GetOrganisationCategoryFromId(int id)
        {
            var organisationCategoryModel = DatabaseService.Get<OrganisationCategoryModel>($@"select * from OrganisationCategory where Id = {id}");
            return organisationCategoryModel;
        }

        public static void DeleteRecord(int id)
        {
            var sql = $@"DELETE from OrganisationCategory where id = {id}";
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
            var sql = $@"DELETE from OrganisationCategory";
            DatabaseService.Execute(sql);
        }
    }
}

