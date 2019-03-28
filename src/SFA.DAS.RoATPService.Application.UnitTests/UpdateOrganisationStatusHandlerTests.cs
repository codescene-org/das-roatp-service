namespace SFA.DAS.RoATPService.Application.UnitTests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Domain;
    using FluentAssertions;
    using Handlers;
    using Interfaces;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using SFA.DAS.RoATPService.Application.Exceptions;
    using Validators;

    [TestFixture]
    public class UpdateOrganisationStatusHandlerTests
    {
        private UpdateOrganisationStatusRequest _request;
        private UpdateOrganisationStatusHandler _handler;
        private Mock<ILogger<UpdateOrganisationStatusHandler>> _logger;
        private Mock<OrganisationValidator> _validator;
        private Mock<IUpdateOrganisationRepository> _repository;
        private Mock<IAuditLogRepository> _auditLogRepository;
        private Mock<IOrganisationStatusRepository> _orgStatusRepository;

        [SetUp]
        public void Before_each_test()
        {
            _request = new UpdateOrganisationStatusRequest
            {
                OrganisationId = Guid.NewGuid(),
                OrganisationStatusId = 0,
                UpdatedBy = "unit test",
                RemovedReasonId = null
            };
            _logger = new Mock<ILogger<UpdateOrganisationStatusHandler>>();
            _validator = new Mock<OrganisationValidator>();
            _repository = new Mock<IUpdateOrganisationRepository>();
            _auditLogRepository = new Mock<IAuditLogRepository>();
            _orgStatusRepository = new Mock<IOrganisationStatusRepository>();

            var activeStatus = new OrganisationStatus { Id = 1, Status = "Active" };
            _orgStatusRepository.Setup(x => x.GetOrganisationStatus(1)).ReturnsAsync(activeStatus);
            var removedStatus = new OrganisationStatus { Id = 0, Status = "Removed" };
            _orgStatusRepository.Setup(x => x.GetOrganisationStatus(0)).ReturnsAsync(removedStatus);
            var notTakingOnStatus = new OrganisationStatus { Id = 2, Status = "Active - not taking on" };
            _orgStatusRepository.Setup(x => x.GetOrganisationStatus(2)).ReturnsAsync(notTakingOnStatus);

            RemovedReason nullReason = null;
            _repository.Setup(x => x.GetRemovedReason(It.IsAny<Guid>())).ReturnsAsync(nullReason);

            _handler = new UpdateOrganisationStatusHandler(_logger.Object, _validator.Object, 
                                                           _repository.Object, _auditLogRepository.Object,
                                                           _orgStatusRepository.Object);
        }

        [TestCase(-1)]
        [TestCase(3)]
        public void Handler_rejects_invalid_organisation_status(int statusId)
        {
            _request.OrganisationStatusId = statusId;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [TestCase(OrganisationStatus.Active)]
        [TestCase(OrganisationStatus.ActiveNotTakingOnApprentices)]
        public void Handler_rejects_removal_reason_if_status_not_removed(int statusId)
        {
            _request.OrganisationStatusId = statusId;
            _request.RemovedReasonId = 1;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [Test]
        public void Handler_accepts_change_from_active_to_not_taking_on_apprentices()
        {
            _repository.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(OrganisationStatus.Active);

            _request.OrganisationStatusId = OrganisationStatus.ActiveNotTakingOnApprentices;

            _repository.Setup(x =>
                    x.UpdateStatus(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _auditLogRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>())).ReturnsAsync(true);

            _repository.Setup(x => x.UpdateStartDate(It.IsAny<Guid>(), It.IsAny<DateTime>())).ReturnsAsync(true).Verifiable();

            var result = _handler.Handle(_request, new CancellationToken()).GetAwaiter().GetResult();

            result.Should().BeTrue();

            _repository.Verify(x => x.UpdateStartDate(It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Test]
        public void Handler_accepts_change_from_not_taking_on_apprentices_to_active()
        {
            _repository.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(OrganisationStatus.ActiveNotTakingOnApprentices);

            _request.OrganisationStatusId = OrganisationStatus.Active;

            _repository.Setup(x =>
                    x.UpdateStatus(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _auditLogRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>())).ReturnsAsync(true);

            _repository.Setup(x => x.UpdateStartDate(It.IsAny<Guid>(), It.IsAny<DateTime>())).ReturnsAsync(true).Verifiable();

            var result = _handler.Handle(_request, new CancellationToken()).GetAwaiter().GetResult();

            result.Should().BeTrue();

            _repository.Verify(x => x.UpdateStartDate(It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Test]
        public void Handler_accepts_change_from_removed_to_active()
        {
            _repository.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(OrganisationStatus.Removed);

            _request.OrganisationStatusId = OrganisationStatus.Active;

            _repository.Setup(x =>
                    x.UpdateStatus(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _auditLogRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>())).ReturnsAsync(true);

            _repository.Setup(x => x.UpdateStartDate(It.IsAny<Guid>(), It.IsAny<DateTime>())).ReturnsAsync(true).Verifiable();

            var result = _handler.Handle(_request, new CancellationToken()).GetAwaiter().GetResult();

            result.Should().BeTrue();

            _repository.Verify(x => x.UpdateStartDate(It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Test]
        public void Handler_accepts_change_from_removed_to_different_removed_reason()
        {
            _repository.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(OrganisationStatus.Removed);
            RemovedReason existingReason = new RemovedReason{ Id = 1, Reason = "test reason" };
            _repository.Setup(x => x.GetRemovedReason(It.IsAny<Guid>())).ReturnsAsync(existingReason);
            _request.OrganisationStatusId = OrganisationStatus.Removed;

            _repository.Setup(x =>
                    x.UpdateStatus(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _auditLogRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>())).ReturnsAsync(true).Verifiable();

            _repository.Setup(x => x.UpdateStartDate(It.IsAny<Guid>(), It.IsAny<DateTime>())).ReturnsAsync(true).Verifiable();

            var result = _handler.Handle(_request, new CancellationToken()).GetAwaiter().GetResult();

            result.Should().BeTrue();

            _repository.Verify(x => x.UpdateStartDate(It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Never);
            _auditLogRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Once);
        }

        [Test]
        public void Handler_accepts_change_from_removed_to_not_taking_on_apprentices()
        {
            _repository.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(OrganisationStatus.Removed);

            _request.OrganisationStatusId = OrganisationStatus.ActiveNotTakingOnApprentices;

            _repository.Setup(x =>
                    x.UpdateStatus(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _auditLogRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>())).ReturnsAsync(true);

            _repository.Setup(x => x.UpdateStartDate(It.IsAny<Guid>(), It.IsAny<DateTime>())).ReturnsAsync(true).Verifiable();

            var result = _handler.Handle(_request, new CancellationToken()).GetAwaiter().GetResult();

            result.Should().BeTrue();

            _repository.Verify(x => x.UpdateStartDate(It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Once);
        }

        [TestCase(OrganisationStatus.Active, 1)]
        [TestCase(OrganisationStatus.Active, 2)]
        [TestCase(OrganisationStatus.Active, 3)]
        [TestCase(OrganisationStatus.Active, 4)]
        [TestCase(OrganisationStatus.Active, 5)]
        [TestCase(OrganisationStatus.Active, 6)]
        [TestCase(OrganisationStatus.Active, 7)]
        [TestCase(OrganisationStatus.Active, 8)]
        [TestCase(OrganisationStatus.Active, 9)]
        [TestCase(OrganisationStatus.Active, 10)]
        [TestCase(OrganisationStatus.Active, 11)]
        [TestCase(OrganisationStatus.Active, 12)]
        [TestCase(OrganisationStatus.ActiveNotTakingOnApprentices, 1)]
        [TestCase(OrganisationStatus.ActiveNotTakingOnApprentices, 2)]
        [TestCase(OrganisationStatus.ActiveNotTakingOnApprentices, 3)]
        [TestCase(OrganisationStatus.ActiveNotTakingOnApprentices, 4)]
        [TestCase(OrganisationStatus.ActiveNotTakingOnApprentices, 5)]
        [TestCase(OrganisationStatus.ActiveNotTakingOnApprentices, 6)]
        [TestCase(OrganisationStatus.ActiveNotTakingOnApprentices, 7)]
        [TestCase(OrganisationStatus.ActiveNotTakingOnApprentices, 8)]
        [TestCase(OrganisationStatus.ActiveNotTakingOnApprentices, 9)]
        [TestCase(OrganisationStatus.ActiveNotTakingOnApprentices, 10)]
        [TestCase(OrganisationStatus.ActiveNotTakingOnApprentices, 11)]
        [TestCase(OrganisationStatus.ActiveNotTakingOnApprentices, 12)]
        public void Handler_accepts_change_from_active_to_removed_and_valid_removal_reasons(int statusId, int removalReasonId)
        {
            _repository.Setup(x => x.GetStatus(It.IsAny<Guid>())).ReturnsAsync(statusId);

            _request.OrganisationStatusId = OrganisationStatus.Removed;
            _request.RemovedReasonId = removalReasonId;

            var removedReason = new RemovedReason {Id = 1, Reason = "test reason"};

            _repository.Setup(x =>
                    x.UpdateStatusWithRemovedReason(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(removedReason);

            _auditLogRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>())).ReturnsAsync(true);

            _repository.Setup(x => x.UpdateStartDate(It.IsAny<Guid>(), It.IsAny<DateTime>())).ReturnsAsync(true).Verifiable();

            var result = _handler.Handle(_request, new CancellationToken()).GetAwaiter().GetResult();

            result.Should().BeTrue();

            _repository.Verify(x => x.UpdateStartDate(It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Never);
        }
    }
}
