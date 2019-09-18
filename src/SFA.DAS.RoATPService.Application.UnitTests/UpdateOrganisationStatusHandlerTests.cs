using System.Collections.Generic;
using SFA.DAS.RoATPService.Application.Services;

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
    using Exceptions;
    using Validators;

    [TestFixture]
    public class UpdateOrganisationStatusHandlerTests
    {
        private UpdateOrganisationStatusRequest _request;
        private UpdateOrganisationStatusHandler _handler;
        private Mock<ILogger<UpdateOrganisationStatusHandler>> _logger;
        private Mock<IOrganisationValidator> _validator;
        private Mock<IUpdateOrganisationRepository> _updateRepository;
        private Mock<ILookupDataRepository> _lookupDataRepository;
        private Mock<IEventsRepository> _eventsRepository;
        private Mock<IOrganisationRepository> _repository;
        private Mock<IAuditLogService> _auditLogService;

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

            _validator = new Mock<IOrganisationValidator>();
            _validator.Setup(x => x.IsValidProviderTypeId(It.IsAny<int>())).Returns(true);
            _validator.Setup(x => x.IsValidOrganisationTypeIdForProvider(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);
            _validator.Setup(x => x.IsValidStatusId(It.IsAny<int>())).Returns(true);
            _validator.Setup(x => x.IsValidOrganisationStatusIdForOrganisation(It.IsAny<int>(), It.IsAny<Guid>())).Returns(true);

            _updateRepository = new Mock<IUpdateOrganisationRepository>();
            _lookupDataRepository = new Mock<ILookupDataRepository>();
            _repository = new Mock<IOrganisationRepository>();
            _eventsRepository = new Mock<IEventsRepository>();

            var activeStatus = new OrganisationStatus { Id = 1, Status = "Active" };
            _lookupDataRepository.Setup(x => x.GetOrganisationStatus(1)).ReturnsAsync(activeStatus);
            var removedStatus = new OrganisationStatus { Id = 0, Status = "Removed" };
            _lookupDataRepository.Setup(x => x.GetOrganisationStatus(0)).ReturnsAsync(removedStatus);
            var notTakingOnStatus = new OrganisationStatus { Id = 2, Status = "Active - not taking on" };
            _lookupDataRepository.Setup(x => x.GetOrganisationStatus(2)).ReturnsAsync(notTakingOnStatus);
            var onboardingStatus = new OrganisationStatus { Id = 3, Status = "On-boarding" };
            _lookupDataRepository.Setup(x => x.GetOrganisationStatus(3)).ReturnsAsync(onboardingStatus);

            _eventsRepository
                .Setup((x => x.AddOrganisationStatusEventsFromOrganisationId(It.IsAny<Guid>(), It.IsAny<int>(),
                    It.IsAny<DateTime>()))).ReturnsAsync(true);
            RemovedReason nullReason = null;
            _repository.Setup(x => x.GetRemovedReason(It.IsAny<Guid>())).ReturnsAsync(nullReason);
            _auditLogService = new Mock<IAuditLogService>();
            _auditLogService.Setup(x => x.CreateAuditData(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(new AuditData { FieldChanges = new List<AuditLogEntry>() });
            _auditLogService.Setup(x => x.AuditOrganisationStatus(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>()))
                .Returns(new AuditData { FieldChanges = new List<AuditLogEntry>() });
            _handler = new UpdateOrganisationStatusHandler(_logger.Object, _validator.Object, 
                                                           _updateRepository.Object,
                                                           _lookupDataRepository.Object, _repository.Object,
                                                           _auditLogService.Object, _eventsRepository.Object);
        }

        [TestCase(-1)]
        [TestCase(3)]
        public void Handler_rejects_invalid_organisation_status(int statusId)
        {
            _validator.Setup(x => x.IsValidStatusId(It.IsAny<int>())).Returns(false);

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

        [TestCase(-1)]
        [TestCase(3)]
        public void Handler_rejects_organisation_status_not_appropriate_for_organisation_provider_id(int statusId)
        {
            _validator.Setup(x => x.IsValidOrganisationStatusIdForOrganisation(statusId, It.IsAny<Guid>())).Returns(false);

            _request.OrganisationStatusId = statusId;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [Test]
        public void Handler_accepts_change_from_active_to_not_taking_on_apprentices()
        {
            _repository.Setup(x => x.GetOrganisationStatus(It.IsAny<Guid>())).ReturnsAsync(OrganisationStatus.Active);

            _request.OrganisationStatusId = OrganisationStatus.ActiveNotTakingOnApprentices;

            _updateRepository.Setup(x =>
                    x.UpdateOrganisationStatus(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _updateRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>())).ReturnsAsync(true);

            _updateRepository.Setup(x => x.UpdateStartDate(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<string>())).ReturnsAsync(true).Verifiable();
            var fieldChanges = new List<AuditLogEntry>();
            fieldChanges.Add(new AuditLogEntry { FieldChanged = AuditLogField.OrganisationStatus, NewValue = "Active - not taking on", PreviousValue = "Active" });
            _auditLogService.Setup(x => x.AuditOrganisationStatus(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>()))
                .Returns(new AuditData { FieldChanges = fieldChanges });
            var result = _handler.Handle(_request, new CancellationToken()).GetAwaiter().GetResult();

            result.Should().BeTrue();

            _updateRepository.Verify(x => x.UpdateStartDate(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Never);
            _eventsRepository.Verify(x=>x.AddOrganisationStatusEventsFromOrganisationId(It.IsAny<Guid>(), It.IsAny<int>() ,It.IsAny<DateTime>()),Times.Once);
        }

        [Test]
        public void Handler_accepts_change_from_not_taking_on_apprentices_to_active()
        {
            _repository.Setup(x => x.GetOrganisationStatus(It.IsAny<Guid>())).ReturnsAsync(OrganisationStatus.ActiveNotTakingOnApprentices);

            _request.OrganisationStatusId = OrganisationStatus.Active;

            _updateRepository.Setup(x =>
                    x.UpdateOrganisationStatus(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _updateRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>())).ReturnsAsync(true);

            _updateRepository.Setup(x => x.UpdateStartDate(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<string>())).ReturnsAsync(true).Verifiable();
            var fieldChanges = new List<AuditLogEntry>();
            fieldChanges.Add(new AuditLogEntry { FieldChanged = AuditLogField.OrganisationStatus, NewValue = "Active - not taking on", PreviousValue = "Active" });
            _auditLogService.Setup(x => x.AuditOrganisationStatus(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>()))
                .Returns(new AuditData { FieldChanges = fieldChanges });
            var result = _handler.Handle(_request, new CancellationToken()).GetAwaiter().GetResult();

            result.Should().BeTrue();

            _updateRepository.Verify(x => x.UpdateStartDate(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Never);
            _eventsRepository.Verify(x => x.AddOrganisationStatusEventsFromOrganisationId(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Test]
        public void Handler_accepts_change_from_removed_to_active()
        {
            _repository.Setup(x => x.GetOrganisationStatus(It.IsAny<Guid>())).ReturnsAsync(OrganisationStatus.Removed);

            _request.OrganisationStatusId = OrganisationStatus.Active;

            _updateRepository.Setup(x =>
                    x.UpdateOrganisationStatus(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _updateRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>())).ReturnsAsync(true);

            _updateRepository.Setup(x => x.UpdateStartDate(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<string>())).ReturnsAsync(true).Verifiable();
            var fieldChanges = new List<AuditLogEntry>();
            fieldChanges.Add(new AuditLogEntry { FieldChanged = AuditLogField.OrganisationStatus, NewValue = "Removed", PreviousValue = "Active" });
            fieldChanges.Add(new AuditLogEntry { FieldChanged = AuditLogField.StartDate, NewValue = DateTime.Today.ToShortDateString(), PreviousValue = null });
            _auditLogService.Setup(x => x.AuditOrganisationStatus(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>()))
                .Returns(new AuditData { FieldChanges = fieldChanges });
            var result = _handler.Handle(_request, new CancellationToken()).GetAwaiter().GetResult();

            result.Should().BeTrue();

            _updateRepository.Verify(x => x.UpdateStartDate(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Once);
            _eventsRepository.Verify(x => x.AddOrganisationStatusEventsFromOrganisationId(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Test]
        public void Handler_accepts_change_from_removed_to_different_removed_reason()
        {
            _repository.Setup(x => x.GetOrganisationStatus(It.IsAny<Guid>())).ReturnsAsync(OrganisationStatus.Removed);
            RemovedReason existingReason = new RemovedReason{ Id = 1, Reason = "test reason" };
            _repository.Setup(x => x.GetRemovedReason(It.IsAny<Guid>())).ReturnsAsync(existingReason);
            _request.OrganisationStatusId = OrganisationStatus.Removed;

            _updateRepository.Setup(x =>
                    x.UpdateOrganisationStatus(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _updateRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>())).ReturnsAsync(true).Verifiable();

            _updateRepository.Setup(x => x.UpdateStartDate(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<string>())).ReturnsAsync(true).Verifiable();
            _updateRepository.Setup(x => x.UpdateRemovedReason(It.IsAny<Guid>(), It.IsAny<int?>(), It.IsAny<string>())).ReturnsAsync(true).Verifiable();

            var fieldChanges = new List<AuditLogEntry>();
            fieldChanges.Add(new AuditLogEntry { FieldChanged = AuditLogField.RemovedReason, NewValue = "Breach", PreviousValue = "Other"});
            _auditLogService.Setup(x => x.AuditOrganisationStatus(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>()))
                .Returns(new AuditData { FieldChanges = fieldChanges });
            var result = _handler.Handle(_request, new CancellationToken()).GetAwaiter().GetResult();

            result.Should().BeTrue();

            _updateRepository.Verify(x => x.UpdateStartDate(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Never);
            _updateRepository.Verify(x => x.UpdateRemovedReason(It.IsAny<Guid>(), It.IsAny<int?>(), It.IsAny<string>()), Times.Once);
            _updateRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Once);
            _eventsRepository.Verify(x => x.AddOrganisationStatusEventsFromOrganisationId(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<DateTime>()), Times.Never);

        }

        [Test]
        public void Handler_accepts_change_from_removed_to_not_taking_on_apprentices()
        {
            _repository.Setup(x => x.GetOrganisationStatus(It.IsAny<Guid>())).ReturnsAsync(OrganisationStatus.Removed);

            _request.OrganisationStatusId = OrganisationStatus.ActiveNotTakingOnApprentices;

            _updateRepository.Setup(x =>
                    x.UpdateOrganisationStatus(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _updateRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>())).ReturnsAsync(true);

            _updateRepository.Setup(x => x.UpdateStartDate(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<string>())).ReturnsAsync(true).Verifiable();
            var fieldChanges = new List<AuditLogEntry>();
            fieldChanges.Add(new AuditLogEntry { FieldChanged = AuditLogField.OrganisationStatus, NewValue = "Removed", PreviousValue = "Active - not taking on" });
            fieldChanges.Add(new AuditLogEntry { FieldChanged = AuditLogField.StartDate, NewValue = null, PreviousValue = DateTime.Today.ToShortDateString()});
            _auditLogService.Setup(x => x.AuditOrganisationStatus(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>()))
                .Returns(new AuditData { FieldChanges = fieldChanges });

            var result = _handler.Handle(_request, new CancellationToken()).GetAwaiter().GetResult();

            result.Should().BeTrue();

            _updateRepository.Verify(x => x.UpdateRemovedReason(It.IsAny<Guid>(), It.IsAny<int?>(), It.IsAny<string>()), Times.Never);
            _updateRepository.Verify(x => x.UpdateStartDate(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Once);
            _updateRepository.Verify(x => x.UpdateOrganisationStatus(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            _eventsRepository.Verify(x => x.AddOrganisationStatusEventsFromOrganisationId(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<DateTime>()), Times.Once);

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
            _repository.Setup(x => x.GetOrganisationStatus(It.IsAny<Guid>())).ReturnsAsync(statusId);

            _request.OrganisationStatusId = OrganisationStatus.Removed;
            _request.RemovedReasonId = removalReasonId;

            _updateRepository.Setup(x =>
                    x.UpdateRemovedReason(It.IsAny<Guid>(), It.IsAny<int?>(),  It.IsAny<string>()))
                .ReturnsAsync(true);
            _updateRepository.Setup(x =>
                    x.UpdateOrganisationStatus(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            var fieldChanges = new List<AuditLogEntry>();
            fieldChanges.Add(new AuditLogEntry { FieldChanged = AuditLogField.OrganisationStatus, NewValue = "Removed", PreviousValue = "Active - not taking on" });
            fieldChanges.Add(new AuditLogEntry { FieldChanged = AuditLogField.RemovedReason, NewValue = null, PreviousValue = "Breach" });
            _auditLogService.Setup(x => x.AuditOrganisationStatus(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>()))
                .Returns(new AuditData { FieldChanges = fieldChanges });

            _updateRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>())).ReturnsAsync(true);

            _updateRepository.Setup(x => x.UpdateStartDate(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<string>())).ReturnsAsync(true).Verifiable();

            var result = _handler.Handle(_request, new CancellationToken()).GetAwaiter().GetResult();

            result.Should().BeTrue();

            _updateRepository.Verify(x => x.UpdateStartDate(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Never);
            _updateRepository.Verify(x => x.UpdateRemovedReason(It.IsAny<Guid>(), It.IsAny<int?>(), It.IsAny<string>()), Times.Once);
            _updateRepository.Verify(x => x.UpdateOrganisationStatus(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            _eventsRepository.Verify(x => x.AddOrganisationStatusEventsFromOrganisationId(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<DateTime>()), Times.Once);


        }
    }
}
