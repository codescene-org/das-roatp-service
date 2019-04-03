namespace SFA.DAS.RoatpService.Data.IntegrationTests.Handlers
{
    using System.Collections.Generic;
    using Dapper;
    using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
    using SFA.DAS.RoatpService.Data.IntegrationTests.Services;
    using SFA.DAS.RoATPService.Data.DapperDataHandlers;
    using SFA.DAS.RoATPService.Domain;

    public static class RemovedReasonHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(RemovedReasonModel removedReason)
        {
            var sql =
                @"set identity_insert [RemovedReasons] ON; INSERT INTO [RemovedReasons] ([Id],[Status],[RemovedReason],[Description],[UpdatedBy],[UpdatedAt],[CreatedBy],[CreatedAt]) VALUES " +
                $@"(@id, @status, @reason, @description, @updatedBy, getdate(), @createdBy, getdate() );set identity_insert [RemovedReasons] OFF; ";

            DatabaseService.Execute(sql, removedReason);
        }

        public static void InsertRecords(List<RemovedReasonModel> removedReasons)
        {
            foreach (var reason in removedReasons)
            {
                InsertRecord(reason);
            }
        }

        public static RemovedReasonModel GetReasonFromid(int reasonId)
        {
            SqlMapper.AddTypeHandler(typeof(RemovedReason), new RemovedReasonDataHandler());
            var removedReasonModel = DatabaseService.Get<RemovedReasonModel>($@"select top 1 * from RemovedReasons where Id = {reasonId}");
            return removedReasonModel;
        }

        public static void DeleteRecord(int id)
        {
            var sql = $@"DELETE from RemovedReasons where id = {id}";
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
            var sql = $@"DELETE from RemovedReasons";
            DatabaseService.Execute(sql);
        }
    }
}
