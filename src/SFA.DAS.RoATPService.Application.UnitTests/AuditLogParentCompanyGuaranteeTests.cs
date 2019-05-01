using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Application.Services;
using SFA.DAS.RoATPService.Settings;

namespace SFA.DAS.RoATPService.Application.UnitTests
{
    [TestFixture]
    public class AuditLogParentCompanyGuaranteeTests
    {
        private RegisterAuditLogSettings _settings;
        private Mock<IOrganisationRepository> _organisationRepository;

        [SetUp]
        public void Before_each_test()
        {
            _organisationRepository = new Mock<IOrganisationRepository>();
            _settings = new RegisterAuditLogSettings();
        }

        [TestCase(true, false, true)]
        [TestCase(false, true, true)]
        [TestCase(false, false, false)]
        [TestCase(true, true, false)]
        public void Audit_log_checks_parent_company_guarantee_audit_is_as_expected(bool currentGuarantee, bool newGuarantee, bool auditChangesMade)
        {
            _organisationRepository.Setup(x => x.GetParentCompanyGuarantee(It.IsAny<Guid>())).ReturnsAsync(currentGuarantee);
            var auditLogService = new AuditLogService(_settings, _organisationRepository.Object, null);
            var auditData = auditLogService.AuditParentCompanyGuarantee(Guid.NewGuid(), "system", newGuarantee);

            Assert.AreEqual(auditChangesMade, auditData.ChangesMade);
        }
    }
}