using System.Collections.Generic;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Handlers
{
    public class ProviderTypeOrganisationTypeHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(ProviderTypeOrganisationTypeModel providerTypeOrganisationType)
        {
            var sql =
                @"set identity_insert [ProviderTypeOrganisationTypes] ON; INSERT INTO [ProviderTypeOrganisationTypes] ([id], [ProviderTypeId], [OrganisationTypeId], CreatedBy, CreatedAt, Status) VALUES " +
                $@"(@id, @providerTypeId, @OrganisationTypeId, 'System',getdate(),'x'); set identity_insert [ProviderTypeOrganisationTypes] OFF; ";

            DatabaseService.Execute(sql, providerTypeOrganisationType);
        }

        public static void InsertRecords(List<ProviderTypeOrganisationTypeModel> providerTypeOrganisationTypes)
        {
            foreach (var ptot in providerTypeOrganisationTypes)
            {
                InsertRecord(ptot);
            }
        }

        public static void DeleteRecord(int id)
        {
            var sql = $@"DELETE from ProviderTypeOrganisationTypes where id = {id}";
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
            var sql = $@"DELETE from ProviderTypeOrganisationTypes";
            DatabaseService.Execute(sql);
        }
    }
}
