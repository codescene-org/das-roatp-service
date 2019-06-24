using System.Collections.Generic;
using SFA.DAS.RoATPService.Application.Services;

namespace SFA.DAS.RoATPService.Application.UnitTests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using FluentAssertions;
    using Handlers;
    using Interfaces;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using Exceptions;
    using Domain;
    using Validators;

    [TestFixture]
    public class UpdateOrganisationApplicationDeterminedDateHandlerTests
    {
        private Mock<ILogger<UpdateOrganisationApplicationDeterminedDateHandler>> _logger;
        private Mock<IOrganisationValidator> _validator;
        private Mock<IUpdateOrganisationRepository> _updateOrganisationRepository;
        private UpdateOrganisationApplicationDeterminedDateHandler _handler;
        private UpdateOrganisationApplicationDeterminedDateRequest _request;
        private Mock<IAuditLogService> _auditLogService;
        private Mock<IOrganisationRepository> _repository;

        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<UpdateOrganisationApplicationDeterminedDateHandler>>();
            _validator = new Mock<IOrganisationValidator>();
            _validator.Setup(x => x.IsValidApplicationDeterminedDate(It.IsAny<DateTime?>())).Returns(true);
            _repository = new Mock<IOrganisationRepository>();
            _repository.Setup(x => x.GetApplicationDeterminedDate(It.IsAny<Guid>())).ReturnsAsync(DateTime.Today).Verifiable();

            _updateOrganisationRepository = new Mock<IUpdateOrganisationRepository>();
            _updateOrganisationRepository.Setup(x => x.UpdateApplicationDeterminedDate(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<string>())).ReturnsAsync(true).Verifiable();
            _updateOrganisationRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>())).ReturnsAsync(true).Verifiable();
            _auditLogService = new Mock<IAuditLogService>();
            _auditLogService.Setup(x => x.CreateAuditData(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(new AuditData { FieldChanges = new List<AuditLogEntry>() });
            _auditLogService.Setup(x => x.AuditApplicationDeterminedDate(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(new AuditData { FieldChanges = new List<AuditLogEntry>() });

            _handler = new UpdateOrganisationApplicationDeterminedDateHandler(_logger.Object, _validator.Object,
                _updateOrganisationRepository.Object, _auditLogService.Object);
            _request = new UpdateOrganisationApplicationDeterminedDateRequest
            {
                OrganisationId = Guid.NewGuid(),
                ApplicationDeterminedDate = DateTime.Today,
                UpdatedBy = "test"
            };
        }

        [Test]
        public void Handler_does_not_update_database_if_application_determined_date_invalid()
        {
            _validator.Setup(x => x.IsValidApplicationDeterminedDate(It.IsAny<DateTime?>())).Returns(false);
            var request = new UpdateOrganisationApplicationDeterminedDateRequest
            {
                ApplicationDeterminedDate = DateTime.Today.AddDays(1),
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };

            Func<Task> result = async () => await
                _handler.Handle(request, new CancellationToken());
            result.Should().Throw<BadRequestException>();

            _auditLogService.Verify(x => x.AuditApplicationDeterminedDate(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
            _updateOrganisationRepository.Verify(x => x.UpdateApplicationDeterminedDate(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Never);
            _updateOrganisationRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
        }


        [Test]
        public void Handler_does_not_update_database_if_company_number_unchanged()
        {
            var request = new UpdateOrganisationApplicationDeterminedDateRequest
            {
                ApplicationDeterminedDate = DateTime.Today,
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };
           
            var result = _handler.Handle(request, new CancellationToken()).Result;
            result.Should().BeFalse();

            _auditLogService.Verify(x => x.AuditApplicationDeterminedDate(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
            _updateOrganisationRepository.Verify(x => x.UpdateApplicationDeterminedDate(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Never);
            _updateOrganisationRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
        }


        [Test]
        public void Handler_does_not_write_audit_log_entry_if_save_operation_fails()
        {
            _updateOrganisationRepository.Setup(x => x.UpdateApplicationDeterminedDate(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<string>())).ReturnsAsync(false).Verifiable();

            var request = new UpdateOrganisationApplicationDeterminedDateRequest
            {
                ApplicationDeterminedDate = DateTime.Today.AddDays(-2),
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };

            var fieldChanges = new List<AuditLogEntry>();
            fieldChanges.Add(new AuditLogEntry { FieldChanged = AuditLogField.ApplicationDeterminedDate, NewValue = DateTime.Today.AddDays(-2).ToShortDateString(), PreviousValue = DateTime.Today.ToShortDateString() });
            _auditLogService.Setup(x => x.AuditApplicationDeterminedDate(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(new AuditData { FieldChanges = fieldChanges });

            var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
            result.Should().BeFalse();

            _auditLogService.Verify(x => x.AuditApplicationDeterminedDate(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
            _updateOrganisationRepository.Verify(x => x.UpdateApplicationDeterminedDate(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Once);
            _updateOrganisationRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
        }

        [Test]
        public void Handler_writes_updated_application_determined_date_and_audit_log_entry_to_database()
        {
            var request = new UpdateOrganisationApplicationDeterminedDateRequest
            {
                ApplicationDeterminedDate = DateTime.Today.AddDays(-2),
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };


            var fieldChanges = new List<AuditLogEntry>();
            fieldChanges.Add(new AuditLogEntry { FieldChanged = AuditLogField.ApplicationDeterminedDate, NewValue = DateTime.Today.AddDays(-2).ToShortDateString(), PreviousValue = DateTime.Today.ToShortDateString() });
            _auditLogService.Setup(x => x.AuditApplicationDeterminedDate(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(new AuditData { FieldChanges = fieldChanges });

            var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
            result.Should().BeTrue();

            _auditLogService.Verify(x => x.AuditApplicationDeterminedDate(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
            _updateOrganisationRepository.Verify(x => x.UpdateApplicationDeterminedDate(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Once);
            _updateOrganisationRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Once);
        }
    }
}

