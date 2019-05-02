using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Application.Services;
using SFA.DAS.RoATPService.Domain;
using SFA.DAS.RoATPService.Settings;

namespace SFA.DAS.RoATPService.Application.UnitTests
{
    [TestFixture]
    public class AuditLogOrganisationTypeTests
    {
        private RegisterAuditLogSettings _settings;
        private Mock<IOrganisationRepository> _organisationRepository;
        private Mock<ILookupDataRepository> _lookupDataRepository;

        [SetUp]
        public void Before_each_test()
        {
            _organisationRepository = new Mock<IOrganisationRepository>();
            _settings = new RegisterAuditLogSettings();
            _lookupDataRepository = new Mock<ILookupDataRepository>();
            _lookupDataRepository.Setup(x => x.GetOrganisationTypes()).ReturnsAsync(
                new List<OrganisationType>
                {
                    new OrganisationType {Id = 0, Type="Unassigned", Status="Live", CreatedBy = "test", CreatedAt = DateTime.Now},
                    new OrganisationType {Id = 1, Type="School", Status="Live", CreatedBy = "test", CreatedAt = DateTime.Now},
                    new OrganisationType {Id = 2, Type="GFE College", Status="Live", CreatedBy = "test", CreatedAt = DateTime.Now},
                  }
            );
        }

        [TestCase(1, 2, true)]
        [TestCase(2, 1, true)]
        [TestCase(1, 1, false)]
        [TestCase(2, 2, false)]
        public void Audit_log_checks_organisation_type_audit_is_as_expected(int currentOrganisationTypeId, int newOrganisationTypeId, bool auditChangesMade)
        {
            _organisationRepository.Setup(x => x.GetOrganisationType(It.IsAny<Guid>())).ReturnsAsync(currentOrganisationTypeId);
            var auditLogService = new AuditLogService(_settings, _organisationRepository.Object, _lookupDataRepository.Object);
            var auditData = auditLogService.AuditOrganisationType(Guid.NewGuid(), "system", newOrganisationTypeId);

            Assert.AreEqual(auditChangesMade, auditData.ChangesMade);
        }
    }
}
