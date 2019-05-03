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
using SFA.DAS.RoATPService.Application.Validators;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Application.UnitTests
{

        [TestFixture]
        public class UpdateOrganisationUkprnHandlerTests
        {
            private Mock<ILogger<UpdateOrganisationUkprnHandler>> _logger;
            private Mock<IOrganisationValidator> _validator;
            private Mock<IUpdateOrganisationRepository> _updateOrganisationRepository;
            private Mock<IOrganisationRepository> _repository;
            private UpdateOrganisationUkprnHandler _handler;
            private Mock<IAuditLogService> _auditLogService;

            [SetUp]
            public void Before_each_test()
            {
                _logger = new Mock<ILogger<UpdateOrganisationUkprnHandler>>();
                _validator = new Mock<IOrganisationValidator>();
                _repository = new Mock<IOrganisationRepository>();
            _validator.Setup(x => x.IsValidUKPRN(It.IsAny<long>())).Returns(true);
                _validator.Setup(x => x.DuplicateUkprnInAnotherOrganisation(11111111, It.IsAny<Guid>()))
                    .Returns(new DuplicateCheckResponse
                    {
                        DuplicateFound = true,
                        DuplicateOrganisationName = "other org"
                    });
                _validator.Setup(x => x.DuplicateUkprnInAnotherOrganisation(It.IsAny<long>(), It.IsAny<Guid>()))
                    .Returns(new DuplicateCheckResponse
                    {
                        DuplicateFound = false,
                        DuplicateOrganisationName = ""
                    });
                _updateOrganisationRepository = new Mock<IUpdateOrganisationRepository>();
                _repository.Setup(x => x.GetUkprn(It.IsAny<Guid>())).ReturnsAsync(11111111).Verifiable();
                _updateOrganisationRepository.Setup(x => x.UpdateUkprn(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(true).Verifiable();
                _updateOrganisationRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>())).ReturnsAsync(true).Verifiable();
                _auditLogService = new Mock<IAuditLogService>();
                _auditLogService.Setup(x => x.CreateAuditData(It.IsAny<Guid>(), It.IsAny<string>()))
                    .Returns(new AuditData { FieldChanges = new List<AuditLogEntry>() });
                _auditLogService.Setup(x => x.AuditUkprn(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<long>()))
                    .Returns(new AuditData { FieldChanges = new List<AuditLogEntry>() });
            _handler = new  UpdateOrganisationUkprnHandler(_logger.Object, _validator.Object, _updateOrganisationRepository.Object, _auditLogService.Object);
            }

            [Test]
            public void Handler_does_not_update_database_if_ukprn_invalid()
            {
                _validator.Setup(x => x.IsValidUKPRN(It.IsAny<long>())).Returns(false);
                var request = new UpdateOrganisationUkprnRequest
                {
                    Ukprn =1111222,
                    OrganisationId = Guid.NewGuid(),
                    UpdatedBy = "unit test"
                };

                Func<Task> result = async () => await
                    _handler.Handle(request, new CancellationToken());
                result.Should().Throw<BadRequestException>();

            _auditLogService.Verify(x => x.AuditUkprn(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<long>()), Times.Never);
            _updateOrganisationRepository.Verify(x => x.UpdateUkprn(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<string>()), Times.Never);
                _updateOrganisationRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
            }

            [Test]
            public void Handler_does_not_update_database_if_ukprn_unchanged()
            {
                var request = new UpdateOrganisationUkprnRequest
                {
                    Ukprn = 11111111,
                    OrganisationId = Guid.NewGuid(),
                    UpdatedBy = "unit test"
                };

                var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
                result.Should().BeFalse();

                _auditLogService.Verify(x => x.AuditUkprn(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<long>()), Times.Once);
                _updateOrganisationRepository.Verify(x => x.UpdateUkprn(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<string>()), Times.Never);
                _updateOrganisationRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
            }

            [Test]
            public void Handler_does_not_write_audit_log_entry_if_save_operation_fails()
            {
                _updateOrganisationRepository.Setup(x => x.UpdateUkprn(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(false).Verifiable();

                var request = new UpdateOrganisationUkprnRequest
                {
                    Ukprn = 11112222,
                    OrganisationId = Guid.NewGuid(),
                    UpdatedBy = "unit test"
                };

                var fieldChanges = new List<AuditLogEntry>();
                fieldChanges.Add(new AuditLogEntry { FieldChanged = "UKPRN", NewValue = "1111111", PreviousValue = "22222222" });
                _auditLogService.Setup(x => x.AuditUkprn(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<long>()))
                    .Returns(new AuditData { FieldChanges = fieldChanges });

            var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
                result.Should().BeFalse();

            _auditLogService.Verify(x => x.AuditUkprn(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<long>()), Times.Once); 
            _updateOrganisationRepository.Verify(x => x.UpdateUkprn(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<string>()), Times.Once);
            _updateOrganisationRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
            }

            [Test]
            public void Handler_writes_updated_legal_name_and_audit_log_entry_to_database()
            {
                var request = new UpdateOrganisationUkprnRequest
                {
                    Ukprn = 11112222,
                    OrganisationId = Guid.NewGuid(),
                    UpdatedBy = "unit test"
                };

                var fieldChanges = new List<AuditLogEntry>();
                fieldChanges.Add(new AuditLogEntry { FieldChanged = "UKPRN", NewValue = "1111111", PreviousValue = "22222222" });
                _auditLogService.Setup(x => x.AuditUkprn(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<long>()))
                    .Returns(new AuditData { FieldChanges = fieldChanges });

            var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
                result.Should().BeTrue();

            _auditLogService.Verify(x => x.AuditUkprn(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<long>()), Times.Once);
            _updateOrganisationRepository.Verify(x => x.UpdateUkprn(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<string>()), Times.Once);
            _updateOrganisationRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Once);
            }
        }
    
}
