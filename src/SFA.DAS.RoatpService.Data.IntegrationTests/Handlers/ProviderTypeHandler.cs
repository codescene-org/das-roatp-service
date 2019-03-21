using System.Collections.Generic;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Handlers
{
    public static class ProviderTypeHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(ProviderTypeModel providerType)
        {
            var sql =
                @"set identity_insert [ProviderTypes] ON; INSERT INTO [ProviderTypes] ([id], [ProviderType], [Description],[CreatedAt],[CreatedBy] ,[UpdatedAt],[UpdatedBy], [Status]) VALUES " +
                $@"(@id, @providerType, @Description, @createdAt, @createdBy, @updatedAt, @updatedBy,@status); set identity_insert [ProviderTypes] OFF; ";

            DatabaseService.Execute(sql, providerType);
        }

        public static void InsertRecords(List<ProviderTypeModel> providerTypes)
        {
            foreach (var orgStatus in providerTypes)
            {
                InsertRecord(orgStatus);
            }
        }

        public static ProviderTypeModel GetProviderTypeFromId(int id)
        {
            var ProviderTypeModel = DatabaseService.Get<ProviderTypeModel>($@"select * from ProviderTypes where Id = {id}");
            return ProviderTypeModel;
        }

        public static void DeleteRecord(int id)
        {
            var sql = $@"DELETE from ProviderTypes where id = {id}";
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
            var sql = $@"DELETE from ProviderTypes";
            DatabaseService.Execute(sql);
        }
    }
}
