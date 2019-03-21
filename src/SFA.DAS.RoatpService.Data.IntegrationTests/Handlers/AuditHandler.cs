using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Handlers
{
    public static class AuditHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(AuditModel audit)
        {
            var sql =
                @"INSERT INTO [Audit] ([OrganisationId],[UpdatedBy],[UpdatedAt],[FieldChanged],[PreviousValue],[NewValue]) VALUES " +
                $@"(@id,@updatedBy, @updatedAt, @updatedAt, @fieldChanged, @previousChanged, @newValue); ";

            DatabaseService.Execute(sql, audit);
        }

        public static void InsertRecords(List<AuditModel> audits)
        {
            foreach (var audit in audits)
            {
                InsertRecord(audit);
            }
        }

        public static AuditModel GetAuditFromId(int auditId)
        {
            var auditModel = DatabaseService.Get<AuditModel>($@"select top 1 * from Audit where Id = {auditId}");
            return auditModel;
        }

        public static AuditModel GetOrganisationFromOrganisationId(Guid organisationId)
        {
            var auditModel = DatabaseService.Get<AuditModel>($@"select top 1 * from Audit where OrganisationId = {organisationId}");
            return auditModel;
        }

        public static void DeleteRecord(int id)
        {
            var sql = $@"DELETE from Audit where id = {id}";
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
            var sql = $@"DELETE from Audit";
            DatabaseService.Execute(sql);
        }
    }
}
