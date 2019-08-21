using System.Collections.Generic;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Handlers
{
    public static class OrganisationCategoryOrgTypeProviderTypeHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(OrganisationCategoryOrgTypeProviderTypeModel orgCategoryOrgTypeProviderType)
        {
            var sql =
                @"set identity_insert [OrganisationCategoryOrgTypeProviderType] ON; INSERT INTO [OrganisationCategoryOrgTypeProviderType] ([id], [OrganisationTypeId]
                ,[OrganisationCategoryId],[ProviderTypeId],[CreatedAt],[CreatedBy],[UpdatedAt],[UpdatedBy],[Status]) VALUES " +
                $@"(@id, @organisationTypeId, @OrganisationCategoryId, @providerTypeId, @createdAt, @createdBy, @updatedAt, @updatedBy,@status); 
                set identity_insert [OrganisationCategoryOrgTypeProviderType] OFF; ";

            DatabaseService.Execute(sql, orgCategoryOrgTypeProviderType);
        }

        public static void InsertRecords(List<OrganisationCategoryOrgTypeProviderTypeModel> orgCategoryOrgTypeProviderTypes)
        {
            foreach (var orgCategoryOrgTypeProviderType in orgCategoryOrgTypeProviderTypes)
            {
                InsertRecord(orgCategoryOrgTypeProviderType);
            }
        }

        public static void DeleteRecord(int id)
        {
            var sql = $@"DELETE from OrganisationCategoryOrgTypeProviderType where id = {id}";
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
            var sql = $@"DELETE from OrganisationCategoryOrgTypeProviderType";
            DatabaseService.Execute(sql);
        }
    }
}
