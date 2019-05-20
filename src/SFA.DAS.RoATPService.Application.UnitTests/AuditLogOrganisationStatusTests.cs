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
    public class AuditLogOrganisationStatusTests

    {
        private RegisterAuditLogSettings _settings;
        private Mock<IOrganisationRepository> _organisationRepository;
        private Mock<ILookupDataRepository> _lookupRepository;

        [SetUp]
        public void Before_each_test()
        {
            _organisationRepository = new Mock<IOrganisationRepository>();
            _lookupRepository = new Mock<ILookupDataRepository>();
            _settings = new RegisterAuditLogSettings();
            _lookupRepository.Setup(x => x.GetOrganisationStatuses()).ReturnsAsync(
                new List<OrganisationStatus>
                {
                    new OrganisationStatus {Id = OrganisationStatus.Removed, Status="Removed", CreatedBy = "test", CreatedAt = DateTime.Now},
                    new OrganisationStatus {Id = OrganisationStatus.Active, Status="Active",CreatedBy = "test", CreatedAt = DateTime.Now},
                    new OrganisationStatus {Id = OrganisationStatus.ActiveNotTakingOnApprentices, Status="Active - not taking on apprentices",CreatedBy = "test", CreatedAt = DateTime.Now},
                    new OrganisationStatus {Id = OrganisationStatus.Onboarding, Status="Onboarding",CreatedBy = "test", CreatedAt = DateTime.Now}
                }
            );
        }

        [TestCase(OrganisationStatus.Active, OrganisationStatus.ActiveNotTakingOnApprentices, null,null, null, 1, true, false,false)]
        [TestCase(OrganisationStatus.ActiveNotTakingOnApprentices, OrganisationStatus.Active, null, null, null, 1, true, false,false)]
        [TestCase(OrganisationStatus.Onboarding, OrganisationStatus.Removed, null, 1, null, 2, true, true, false)]
        [TestCase(OrganisationStatus.Onboarding, OrganisationStatus.Active, null, 1, null, 3, true, true, true)]
        [TestCase(OrganisationStatus.Onboarding, OrganisationStatus.ActiveNotTakingOnApprentices, null, 1, null, 3, true, true, true)]
        [TestCase(OrganisationStatus.Removed, OrganisationStatus.Onboarding, null, null, null, 1, true, false, false)]
        [TestCase(OrganisationStatus.Active, OrganisationStatus.Onboarding, null, null, null, 1, true, false, false)]
        [TestCase(OrganisationStatus.ActiveNotTakingOnApprentices, OrganisationStatus.Onboarding, null, null, null, 1, true, false, false)]
        [TestCase(OrganisationStatus.Active,OrganisationStatus.Active, null, null, null, 0, false, false, false)]
        [TestCase(OrganisationStatus.ActiveNotTakingOnApprentices, OrganisationStatus.ActiveNotTakingOnApprentices, null, null, null, 0, false, false, false)]
        [TestCase(OrganisationStatus.Removed, OrganisationStatus.Removed, null, null, null, 0, false, false, false)]
        [TestCase(OrganisationStatus.Onboarding, OrganisationStatus.Onboarding, null, null, null, 0, false, false, false)]

        public void Audit_log_checks_organisation_status_audit_is_as_expected(int currentOrganisationStatusId, int newOrganisationStatusId, int? currentRemovedReasonId, int? newRemovedReasonId, DateTime? currentStartDate, int numberOfFieldsChanged, bool statusAudit, bool removedReasonAudit, bool startDateAudit)
        {
            if (newRemovedReasonId.HasValue)
            {
                _organisationRepository.Setup(x => x.GetRemovedReason(It.IsAny<Guid>())).ReturnsAsync(
                    new RemovedReason
                    {
                        Id = newRemovedReasonId.Value,
                        Reason = $"Reason {newRemovedReasonId.Value}",
                        Status = $"Status {newRemovedReasonId.Value}",
                        Description = $"Description {newRemovedReasonId.Value}",
                        CreatedBy = "system",
                        CreatedAt = DateTime.Now
                    });
            }
            else
            {
                _organisationRepository.Setup(x => x.GetRemovedReason(It.IsAny<Guid>()))
                    .ReturnsAsync((RemovedReason) null);
            }

            _organisationRepository.Setup(x => x.GetOrganisationStatus(It.IsAny<Guid>()))
                .ReturnsAsync(currentOrganisationStatusId);
            _organisationRepository.Setup(x => x.GetStartDate(It.IsAny<Guid>())).ReturnsAsync(currentStartDate);
            var removedReasons = new List<RemovedReason>();
            if (currentRemovedReasonId.HasValue)
                removedReasons.Add(new RemovedReason {CreatedAt = DateTime.Now,CreatedBy = "test", Description = $"desc {currentRemovedReasonId.Value}", Reason = $"Reason {currentRemovedReasonId.Value}",Id = currentRemovedReasonId.Value,Status=$"status {currentRemovedReasonId.Value}"});
            _lookupRepository.Setup(x => x.GetRemovedReasons()).ReturnsAsync(removedReasons);
            var auditLogService = new AuditLogService(_settings, _organisationRepository.Object, _lookupRepository.Object,null);
            var auditData = auditLogService.AuditOrganisationStatus(Guid.NewGuid(), "system", newOrganisationStatusId, newRemovedReasonId);

            Assert.AreEqual(numberOfFieldsChanged, auditData.FieldChanges.Count);
            Assert.AreEqual(statusAudit, auditData.FieldChanges.Any(x=>x.FieldChanged == AuditLogField.OrganisationStatus));
            Assert.AreEqual(removedReasonAudit, auditData.FieldChanges.Any(x => x.FieldChanged == AuditLogField.RemovedReason));
            Assert.AreEqual(startDateAudit, auditData.FieldChanges.Any(x => x.FieldChanged == AuditLogField.StartDate));
        }
    }
}
