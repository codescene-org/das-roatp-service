using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Application.Services;
using SFA.DAS.RoATPService.Domain;
using SFA.DAS.RoATPService.Settings;

namespace SFA.DAS.RoATPService.Application.UnitTests
{

    [TestFixture]
    public class AuditLogProviderTypeTests
    {
        private RegisterAuditLogSettings _settings;
        private Mock<IOrganisationRepository> _organisationRepository;
        private Mock<ILookupDataRepository> _lookupDataRepository;
        private const int OrgTypeIdUnassigned = 0;
        private const int OrgTypeIdSchool = 1;
         private const int OrgTypeIdGfeCollege = 2;
        [SetUp]
        public void Before_each_test()
        {
            _organisationRepository = new Mock<IOrganisationRepository>();
            _settings = new RegisterAuditLogSettings();
         
            _lookupDataRepository = new Mock<ILookupDataRepository>();
            _lookupDataRepository.Setup(x => x.GetOrganisationTypes()).ReturnsAsync(
                new List<OrganisationType>
                {
                    new OrganisationType {Id = OrgTypeIdUnassigned, Type="Unassigned", Status="Live", CreatedBy = "test", CreatedAt = DateTime.Now},
                    new OrganisationType {Id = OrgTypeIdSchool, Type="School", Status="Live", CreatedBy = "test", CreatedAt = DateTime.Now},
                    new OrganisationType {Id = OrgTypeIdGfeCollege, Type="GFE College", Status="Live", CreatedBy = "test", CreatedAt = DateTime.Now},
                }
            );
        }

        [TestCase(OrgTypeIdSchool, OrgTypeIdSchool, ProviderType.MainProvider, ProviderType.MainProvider, OrganisationStatus.Active, "today", 0, false, false, false, false)]
        [TestCase(OrgTypeIdSchool, OrgTypeIdSchool, ProviderType.MainProvider, ProviderType.MainProvider, OrganisationStatus.Removed, "today", 0, false, false, false, false)]
        [TestCase(OrgTypeIdSchool, OrgTypeIdSchool, ProviderType.MainProvider, ProviderType.MainProvider, OrganisationStatus.Active, "yesterday", 0, false, false, false, false)]
        [TestCase(OrgTypeIdSchool, OrgTypeIdGfeCollege, ProviderType.EmployerProvider, ProviderType.EmployerProvider, OrganisationStatus.Active, "today", 1, false, true, false, false)]
        [TestCase(OrgTypeIdSchool, OrgTypeIdSchool, ProviderType.MainProvider, ProviderType.EmployerProvider, OrganisationStatus.Active, "today", 1, true, false, false, false)]
        [TestCase(OrgTypeIdSchool, OrgTypeIdGfeCollege, ProviderType.MainProvider, ProviderType.EmployerProvider, OrganisationStatus.Active, "today", 2, true, true, false, false)]
        [TestCase(OrgTypeIdSchool, OrgTypeIdGfeCollege, ProviderType.MainProvider, ProviderType.EmployerProvider, OrganisationStatus.Removed, "today", 2, true, true, false, false)]
        [TestCase(OrgTypeIdSchool, OrgTypeIdGfeCollege, ProviderType.MainProvider, ProviderType.SupportingProvider, OrganisationStatus.Removed, "today", 2, true, true, false, false)]
        [TestCase(OrgTypeIdSchool, OrgTypeIdGfeCollege, ProviderType.MainProvider, ProviderType.SupportingProvider, OrganisationStatus.Onboarding, "today", 3, true, true, true, false)]
        [TestCase(OrgTypeIdSchool, OrgTypeIdGfeCollege, ProviderType.EmployerProvider, ProviderType.SupportingProvider, OrganisationStatus.Onboarding, "today", 3, true, true, true, false)]
        [TestCase(OrgTypeIdSchool, OrgTypeIdGfeCollege, ProviderType.MainProvider, ProviderType.SupportingProvider, OrganisationStatus.Onboarding, "yesterday", 4, true, true, true, true)]
        [TestCase(OrgTypeIdSchool, OrgTypeIdGfeCollege, ProviderType.EmployerProvider, ProviderType.SupportingProvider, OrganisationStatus.Onboarding, "yesterday", 4, true, true, true, true)]
        [TestCase(OrgTypeIdSchool, OrgTypeIdSchool, ProviderType.EmployerProvider, ProviderType.SupportingProvider, OrganisationStatus.Onboarding, "yesterday", 3, true, false, true, true)]
        [TestCase(OrgTypeIdSchool, OrgTypeIdSchool, ProviderType.EmployerProvider, ProviderType.SupportingProvider, OrganisationStatus.Onboarding, "today", 2, true, false, true, false)]
        [TestCase(OrgTypeIdSchool, OrgTypeIdGfeCollege, ProviderType.EmployerProvider, ProviderType.SupportingProvider, OrganisationStatus.Onboarding, "today", 3, true, true, true, false)]
        public void Audit_log_checks_provider_type_audit_is_as_expected(int currentOrganisationTypeId, int newOrganisationTypeId, 
                                                                                int currentProviderTypeId, int newProviderType,
                                                                                int currentOrganisationStatus, string currentStartDate, 
                                                                                int numberOfFieldsChanged, bool providerTypeAudit, bool organisationTypeAudit, bool organisationStatusAudit, bool startDateAudit)
        {
            var startDate = DateTime.MinValue;
            if (currentStartDate == "yesterday")
                startDate = DateTime.Today.AddDays(-1);
            if (currentStartDate == "today")
                startDate = DateTime.Today;

            _organisationRepository.Setup(x => x.GetOrganisationType(It.IsAny<Guid>())).ReturnsAsync(currentOrganisationTypeId);
            _organisationRepository.Setup(x => x.GetProviderType(It.IsAny<Guid>())).ReturnsAsync(currentProviderTypeId);
            _organisationRepository.Setup(x => x.GetOrganisationStatus(It.IsAny<Guid>())).ReturnsAsync(currentOrganisationStatus);
            _organisationRepository.Setup(x => x.GetStartDate(It.IsAny<Guid>())).ReturnsAsync(startDate);
            var auditLogService = new AuditLogService(_settings, _organisationRepository.Object, _lookupDataRepository.Object, new OrganisationStatusManager());
            var auditData = auditLogService.AuditProviderType(Guid.NewGuid(), "system", newProviderType, newOrganisationTypeId);

            Assert.AreEqual(numberOfFieldsChanged, auditData.FieldChanges.Count);
            Assert.AreEqual(providerTypeAudit, auditData.FieldChanges.Any(x => x.FieldChanged == AuditLogField.ProviderType));
            Assert.AreEqual(organisationTypeAudit, auditData.FieldChanges.Any(x => x.FieldChanged == AuditLogField.OrganisationType));
            Assert.AreEqual(organisationStatusAudit, auditData.FieldChanges.Any(x => x.FieldChanged == AuditLogField.OrganisationStatus));
            Assert.AreEqual(startDateAudit, auditData.FieldChanges.Any(x => x.FieldChanged == AuditLogField.StartDate));

        }
    }
}
