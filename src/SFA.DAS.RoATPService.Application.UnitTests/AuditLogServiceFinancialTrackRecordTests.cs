using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Application.Services;
using SFA.DAS.RoATPService.Domain;
using SFA.DAS.RoATPService.Settings;

namespace SFA.DAS.RoATPService.Application.UnitTests
{
    [TestFixture]
    public class AuditLogServiceFinancialTrackRecordTests
    {
        private RegisterAuditLogSettings _settings;
        private Mock<IOrganisationRepository> _organisationRepository;

        [SetUp]
        public void Before_each_test()
        {
            _organisationRepository = new Mock<IOrganisationRepository>();
            _settings = new RegisterAuditLogSettings();
        }

        [TestCase(true,false,true)]
        [TestCase(false, true, true)]
        [TestCase(false, false, false)]
        [TestCase(true, true, false)]
        public void Audit_log_checks_financial_track_record_audit_is_as_expected(bool currentFinancialTrackRecord, bool newFinancialTrackRecord, bool auditChangesMade)
        {
            _organisationRepository.Setup(x => x.GetFinancialTrackRecord(It.IsAny<Guid>())).ReturnsAsync(currentFinancialTrackRecord);
            var auditLogService = new AuditLogService(_settings, _organisationRepository.Object);
            var auditData = auditLogService.AuditFinancialTrackRecord(Guid.NewGuid(), "system", newFinancialTrackRecord);

            Assert.AreEqual(auditChangesMade, auditData.ChangesMade);
        }
    }
}
