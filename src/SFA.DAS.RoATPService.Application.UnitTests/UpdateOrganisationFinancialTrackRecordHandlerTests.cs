using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Api.Types.Models;
using SFA.DAS.RoATPService.Application.Handlers;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Application.UnitTests
{
    [TestFixture]
    public class UpdateOrganisationFinancialTrackRecordHandlerTests
    {
        private Mock<ILogger<UpdateOrganisationFinancialTrackRecordHandler>> _logger;
        private Mock<IUpdateOrganisationRepository> _repository;
        private Mock<IAuditLogRepository> _auditRepository;
        private UpdateOrganisationFinancialTrackRecordHandler _handler;

        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<UpdateOrganisationFinancialTrackRecordHandler>>();
            _repository = new Mock<IUpdateOrganisationRepository>();
            _repository.Setup(x => x.GetFinancialTrackRecord(It.IsAny<Guid>())).ReturnsAsync(true).Verifiable();
            _repository.Setup(x => x.UpdateFinancialTrackRecord(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>())).ReturnsAsync(true).Verifiable();
            _auditRepository = new Mock<IAuditLogRepository>();
            _auditRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>())).ReturnsAsync(true).Verifiable();


            _handler = new UpdateOrganisationFinancialTrackRecordHandler(_logger.Object, _repository.Object, _auditRepository.Object);
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

            _repository.Verify(x => x.GetFinancialTrackRecord(It.IsAny<Guid>()), Times.Once);
            _repository.Verify(x => x.UpdateFinancialTrackRecord(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
            _auditRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
        }

        [Test]
        public void Handler_does_not_write_audit_log_entry_if_save_operation_fails()
        {
            _repository.Setup(x => x.UpdateFinancialTrackRecord(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>())).ReturnsAsync(false).Verifiable();

            var request = new UpdateOrganisationFinancialTrackRecordRequest
            {
                FinancialTrackRecord = true,
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };

            var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
            result.Should().BeFalse();

            _repository.Verify(x => x.GetFinancialTrackRecord(It.IsAny<Guid>()), Times.Once);
            _repository.Verify(x => x.UpdateFinancialTrackRecord(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
            _auditRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
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

            var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
            result.Should().BeTrue();

            _repository.Verify(x => x.GetFinancialTrackRecord(It.IsAny<Guid>()), Times.Once);
            _repository.Verify(x => x.UpdateFinancialTrackRecord(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
            _auditRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Once);
        }
    }
}