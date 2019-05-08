using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Api.Types.Models;
using SFA.DAS.RoATPService.Application.Exceptions;
using SFA.DAS.RoATPService.Application.Handlers;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Application.Services;
using SFA.DAS.RoATPService.Application.Validators;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Application.UnitTests
{

    [TestFixture]
    public class UpdateOrganisationCompanyNameHandlerTests
    {
        private Mock<ILogger<UpdateOrganisationCompanyNumberHandler>> _logger;
        private Mock<IOrganisationValidator> _validator;
        private Mock<IUpdateOrganisationRepository> _updateOrganisationRepository;
        private Mock<IOrganisationRepository> _repository;
        private UpdateOrganisationCompanyNumberHandler _handler;
        private Mock<IAuditLogService> _auditLogService;

        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<UpdateOrganisationCompanyNumberHandler>>();
            _validator = new Mock<IOrganisationValidator>();
            _repository = new Mock<IOrganisationRepository>();
            _validator.Setup(x => x.IsValidCompanyNumber(It.IsAny<string>())).Returns(true);
       
            _updateOrganisationRepository = new Mock<IUpdateOrganisationRepository>();
            _repository.Setup(x => x.GetCompanyNumber(It.IsAny<Guid>())).ReturnsAsync("11111111").Verifiable();
            _updateOrganisationRepository.Setup(x => x.UpdateCompanyNumber(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true).Verifiable();
            _updateOrganisationRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>())).ReturnsAsync(true).Verifiable();
            _auditLogService = new Mock<IAuditLogService>();
            _auditLogService.Setup(x => x.CreateAuditData(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(new AuditData { FieldChanges = new List<AuditLogEntry>() });
            _auditLogService.Setup(x => x.AuditCompanyNumber(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new AuditData { FieldChanges = new List<AuditLogEntry>() });
            _handler = new UpdateOrganisationCompanyNumberHandler(_logger.Object, _validator.Object, _updateOrganisationRepository.Object, _auditLogService.Object);
        }

        [Test]
        public void Handler_does_not_update_database_if_company_number_invalid()
        {
            _validator.Setup(x => x.IsValidCompanyNumber(It.IsAny<string>())).Returns(false);
            var request = new UpdateOrganisationCompanyNumberRequest
            {
                CompanyNumber = "1111222",
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };

            Func<Task> result = async () => await
                _handler.Handle(request, new CancellationToken());
            result.Should().Throw<BadRequestException>();

            _auditLogService.Verify(x => x.AuditCompanyNumber(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _updateOrganisationRepository.Verify(x => x.UpdateCompanyNumber(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _updateOrganisationRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
        }

        [Test]
        public void Handler_does_not_update_database_if_company_number_unchanged()
        {
            var request = new UpdateOrganisationCompanyNumberRequest
            {
                CompanyNumber = "11111111",
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };

            var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
            result.Should().BeFalse();

            _auditLogService.Verify(x => x.AuditCompanyNumber(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _updateOrganisationRepository.Verify(x => x.UpdateCompanyNumber(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _updateOrganisationRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
        }

        [Test]
        public void Handler_does_not_write_audit_log_entry_if_save_operation_fails()
        {
            _updateOrganisationRepository.Setup(x => x.UpdateCompanyNumber(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false).Verifiable();

            var request = new UpdateOrganisationCompanyNumberRequest
            {
                CompanyNumber = "11112222",
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };

            var fieldChanges = new List<AuditLogEntry>();
            fieldChanges.Add(new AuditLogEntry { FieldChanged = AuditLogField.CompanyNumber, NewValue = "1111111", PreviousValue = "22222222" });
            _auditLogService.Setup(x => x.AuditCompanyNumber(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new AuditData { FieldChanges = fieldChanges });

            var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
            result.Should().BeFalse();

            _auditLogService.Verify(x => x.AuditCompanyNumber(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _updateOrganisationRepository.Verify(x => x.UpdateCompanyNumber(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _updateOrganisationRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
        }

        [Test]
        public void Handler_writes_updated_company_number_and_audit_log_entry_to_database()
        {
            var request = new UpdateOrganisationCompanyNumberRequest
            {
                CompanyNumber = "11112222",
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };

            var fieldChanges = new List<AuditLogEntry>();
            fieldChanges.Add(new AuditLogEntry { FieldChanged = AuditLogField.CompanyNumber, NewValue = "1111111", PreviousValue = "22222222" });
            _auditLogService.Setup(x => x.AuditCompanyNumber(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new AuditData { FieldChanges = fieldChanges });

            var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
            result.Should().BeTrue();

            _auditLogService.Verify(x => x.AuditCompanyNumber(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _updateOrganisationRepository.Verify(x => x.UpdateCompanyNumber(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _updateOrganisationRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Once);
        }
    }

}
