using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Application.Services;
using SFA.DAS.RoATPService.Settings;

namespace SFA.DAS.RoATPService.Application.UnitTests
{
    [TestFixture]
    public class AuditLogUkprnTests
    {
        private RegisterAuditLogSettings _settings;
        private Mock<IOrganisationRepository> _organisationRepository;

        [SetUp]
        public void Before_each_test()
        {
            _organisationRepository = new Mock<IOrganisationRepository>();
            _settings = new RegisterAuditLogSettings();
        }

        [TestCase(11111111, 22222222, true)]
        [TestCase(22222222, 11111111, true)]
        [TestCase(11111111, 11111111, false)]
        [TestCase(22222222,22222222, false)]

        public void Audit_log_checks_trading_name_guarantee_audit_is_as_expected(long currentUkprn, long newUkprn, bool auditChangesMade)
        {
            _organisationRepository.Setup(x => x.GetUkprn(It.IsAny<Guid>())).ReturnsAsync(currentUkprn);
            var auditLogService = new AuditLogService(_settings, _organisationRepository.Object,null);
            var auditData = auditLogService.AuditUkprn(Guid.NewGuid(), "system", newUkprn);

            Assert.AreEqual(auditChangesMade, auditData.ChangesMade);
        }
    }
}
