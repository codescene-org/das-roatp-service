using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using SFA.DAS.RoatpService.Data.IntegrationTests.Handlers;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;
using SFA.DAS.RoATPService.Data;
using SFA.DAS.RoATPService.Data.DapperDataHandlers;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Tests
{
    public class AddAuditLogTests: TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private AuditLogRepository _repository;
        private Guid _organisationId;
        private bool _updateSuccessful;
        private AuditModel _auditRecord;
        private string _updatedBy;
        private string _fieldChanged;
        private string _previousValue;
        private string _newValue;

        public AddAuditLogTests()
        {
            SqlMapper.AddTypeHandler(typeof(AuditData), new AuditDataHandler());
        }

        [OneTimeSetUp]
        public void setup_organisation_is_added()
        {
            _organisationId = Guid.NewGuid();
            _updatedBy = "test user";
            _fieldChanged = "trading name 1";
            _previousValue = "Trainer Trading Name";
            _newValue = "ANDERSON TRAINING LTD";
            _repository = new AuditLogRepository(_databaseService.WebConfiguration);
            var auditData = new AuditData
            {
                OrganisationId =_organisationId,
                UpdatedAt = DateTime.Now,
                UpdatedBy = _updatedBy,
                FieldChanges = new List<AuditLogEntry>
                {
                    new AuditLogEntry
                    {
                        FieldChanged = _fieldChanged,
                        PreviousValue = _previousValue,
                        NewValue = _newValue
                    }
                }
            };

            _updateSuccessful = _repository.WriteFieldChangesToAuditLog(auditData).Result;
            _auditRecord = AuditHandler.GetOrganisationFromOrganisationId(_organisationId);
        }


        [Test]
        public void Update_is_successful()
        {
            Assert.AreEqual(_updateSuccessful, true);
        }

        [Test]
        public void UpdatedBy_is_correct()
        {
               Assert.AreEqual(_updatedBy, _auditRecord.UpdatedBy);
        }

        [Test]
        public void FieldChanged_is_correct()
        {
            Assert.AreEqual(_fieldChanged, _auditRecord.AuditData.FieldChanges[0].FieldChanged);
        }

        [Test]
        public void PreviousValue_is_correct()
        {
            Assert.AreEqual(_previousValue, _auditRecord.AuditData.FieldChanges[0].PreviousValue);
        }

        [Test]
        public void NewValue_is_correct()
        {
            Assert.AreEqual(_newValue, _auditRecord.AuditData.FieldChanges[0].NewValue);
        }

        [OneTimeTearDown]
        public void tear_down()
        {
            AuditHandler.DeleteAllRecords();
        }
    }
}
