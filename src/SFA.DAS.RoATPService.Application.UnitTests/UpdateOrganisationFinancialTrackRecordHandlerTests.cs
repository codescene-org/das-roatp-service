using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Api.Types.Models;
using SFA.DAS.RoATPService.Application.Handlers;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Application.Services;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Application.UnitTests
{
    [TestFixture]
    public class UpdateOrganisationFinancialTrackRecordHandlerTests
    {
        private Mock<ILogger<UpdateOrganisationFinancialTrackRecordHandler>> _logger;
        private Mock<IUpdateOrganisationRepository> _updateRepository;
        private Mock<IOrganisationRepository> _repository;
        private UpdateOrganisationFinancialTrackRecordHandler _handler;
        private Mock<IAuditLogService> _auditLogService;
        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<UpdateOrganisationFinancialTrackRecordHandler>>();
            _updateRepository = new Mock<IUpdateOrganisationRepository>();
            _repository = new Mock<IOrganisationRepository>();
            _repository.Setup(x => x.GetFinancialTrackRecord(It.IsAny<Guid>())).ReturnsAsync(true).Verifiable();
            _updateRepository.Setup(x => x.UpdateFinancialTrackRecord(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>())).ReturnsAsync(true).Verifiable();
            _updateRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>())).ReturnsAsync(true).Verifiable();
            _auditLogService = new Mock<IAuditLogService>();
            _auditLogService
                .Setup(x => x.AuditFinancialTrackRecord(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AuditData { FieldChanges = new List<AuditLogEntry>() });

            _auditLogService.Setup(x => x.CreateAuditData(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(new AuditData { FieldChanges = new List<AuditLogEntry>() });
            _handler = new UpdateOrganisationFinancialTrackRecordHandler(_logger.Object, _updateRepository.Object, _auditLogService.Object);
        }

        [Test]
        public void Handler_does_not_update_database_if_parent_company_guarantee_unchanged()
        {
            var request = new UpdateOrganisationFinancialTrackRecordRequest
            {
                FinancialTrackRecord = true,
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };

            var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
            result.Should().BeFalse();

            _auditLogService.Verify(x=> x.AuditFinancialTrackRecord(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>()),Times.Once);
            _updateRepository.Verify(x => x.UpdateFinancialTrackRecord(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
            _updateRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
        }

        [Test]
        public void Handler_does_not_write_audit_log_entry_if_save_operation_fails()
        {
            _updateRepository.Setup(x => x.UpdateFinancialTrackRecord(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>())).ReturnsAsync(false).Verifiable();

            var request = new UpdateOrganisationFinancialTrackRecordRequest
            {
                FinancialTrackRecord = true,
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };

            var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
            result.Should().BeFalse();

            _auditLogService.Verify(x => x.AuditFinancialTrackRecord(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
            _updateRepository.Verify(x => x.UpdateFinancialTrackRecord(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
            _updateRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
        }

        [Test]
        public void Handler_writes_updated_parent_company_guarantee_and_audit_log_entry_to_database()
        {
            var request = new UpdateOrganisationFinancialTrackRecordRequest
            {
                FinancialTrackRecord = false,
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };

            var fieldChanges = new List<AuditLogEntry>();
            fieldChanges.Add(new AuditLogEntry {FieldChanged = "Financial Track Record", NewValue = "True", PreviousValue = "False"});
            _auditLogService.Setup(x => x.AuditFinancialTrackRecord(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new AuditData { FieldChanges = fieldChanges });

            var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
            result.Should().BeTrue();

            _auditLogService.Verify(x => x.AuditFinancialTrackRecord(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
            _updateRepository.Verify(x => x.UpdateFinancialTrackRecord(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
            _updateRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Once);
        }
    }
}