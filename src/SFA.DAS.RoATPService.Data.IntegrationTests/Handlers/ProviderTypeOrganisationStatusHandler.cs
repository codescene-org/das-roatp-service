using System.Collections.Generic;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Handlers
{
    public class ProviderTypeOrganisationStatusHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(ProviderTypeOrganisationStatusModel providerTypeOrganisationStatus)
        {
            var sql =
                @"set identity_insert [ProviderTypeOrganisationStatus] ON; INSERT INTO [ProviderTypeOrganisationStatus] ([id], [ProviderTypeId], [OrganisationStatusId], CreatedBy, CreatedAt, Status) VALUES " +
                $@"(@id, @providerTypeId, @OrganisationStatusId, 'System',getdate(),'x'); set identity_insert [ProviderTypeOrganisationStatus] OFF; ";

            DatabaseService.Execute(sql, providerTypeOrganisationStatus);
        }

        public static void InsertRecords(List<ProviderTypeOrganisationStatusModel> providerTypeOrganisationStatuses)
        {
            foreach (var ptos in providerTypeOrganisationStatuses)
            {
                InsertRecord(ptos);
            }
        }

        public static void DeleteRecord(int id)
        {
            var sql = $@"DELETE from ProviderTypeOrganisationStatus where id = {id}";
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
            var sql = $@"DELETE from ProviderTypeOrganisationStatus";
            DatabaseService.Execute(sql);
        }
    }
}
