using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Application.Services;
using SFA.DAS.RoATPService.Settings;

namespace SFA.DAS.RoATPService.Application.UnitTests
{
    [TestFixture]
    public class AuditLogLegalNameTests
    {
        private RegisterAuditLogSettings _settings;
        private Mock<IOrganisationRepository> _organisationRepository;

        [SetUp]
        public void Before_each_test()
        {
            _organisationRepository = new Mock<IOrganisationRepository>();
            _settings = new RegisterAuditLogSettings();
        }

        [TestCase("first name", "second name", true)]
        [TestCase("second name", "first name", true)]
        [TestCase("first name", "first name", false)]
        [TestCase("second name", "second name", false)]
        public void Audit_log_checks_legal_name_audit_is_as_expected(string currentName, string newName, bool auditChangesMade)
        {
            _organisationRepository.Setup(x => x.GetLegalName(It.IsAny<Guid>())).ReturnsAsync(currentName);
            var auditLogService = new AuditLogService(_settings, _organisationRepository.Object, null,null);
            var auditData = auditLogService.AuditLegalName(Guid.NewGuid(), "system", newName);

            Assert.AreEqual(auditChangesMade, auditData.ChangesMade);
        }
    }
}