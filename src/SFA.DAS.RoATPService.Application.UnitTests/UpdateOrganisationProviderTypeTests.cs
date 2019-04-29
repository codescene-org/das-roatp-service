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
    using SFA.DAS.RoATPService.Application.Exceptions;
    using SFA.DAS.RoATPService.Domain;
    using Validators;

    [TestFixture]
    public class UpdateOrganisationProviderTypeTests
    {
        private Mock<ILogger<UpdateOrganisationProviderTypeHandler>> _logger;
        private Mock<IOrganisationValidator> _validator;
        private Mock<IUpdateOrganisationRepository> _updateOrganisationRepository;
        private Mock<IOrganisationRepository> _repository;
        private Mock<ILookupDataRepository> _lookupDataRepository;
        private UpdateOrganisationProviderTypeHandler _handler;
        private UpdateOrganisationProviderTypeRequest _request;
        private Mock<IAuditLogService> _auditLogService;
        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<UpdateOrganisationProviderTypeHandler>>();
            _validator = new Mock<IOrganisationValidator>();
            _validator.Setup(x => x.IsValidProviderTypeId(It.IsAny<int>())).Returns(true);
            _validator.Setup(x => x.IsValidOrganisationTypeIdForProvider(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);
            _updateOrganisationRepository = new Mock<IUpdateOrganisationRepository>();
            _repository = new Mock<IOrganisationRepository>();
            _lookupDataRepository = new Mock<ILookupDataRepository>();
            _auditLogService = new Mock<IAuditLogService>();
            _auditLogService.Setup(x => x.CreateAuditData(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(new AuditData{FieldChanges = new List<AuditLogEntry>()});
            _handler = new UpdateOrganisationProviderTypeHandler(_logger.Object, _validator.Object, 
                _updateOrganisationRepository.Object, _lookupDataRepository.Object, new OrganisationStatusManager(), _repository.Object, _auditLogService.Object);
            _request = new UpdateOrganisationProviderTypeRequest
            {
                OrganisationId = Guid.NewGuid(),
                OrganisationTypeId = 1,
                ProviderTypeId = 2,
                UpdatedBy = "test"
            };
        }

        [Test]
        public void Handler_rejects_request_with_invalid_provider_type()
        {
            _validator.Setup(x => x.IsValidProviderTypeId(It.IsAny<int>())).Returns(false);

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [Test]
        public void Handler_rejects_request_with_invalid_organisation_type_for_provider()
        {
            _validator.Setup(x => x.IsValidOrganisationTypeIdForProvider(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(false);

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [Test]
        public void Handler_updates_provider_type_and_organisation_type_and_records_audit_history()
        {
            _repository.Setup(x => x.GetProviderType(It.IsAny<Guid>())).ReturnsAsync(1);
            _repository.Setup(x => x.GetOrganisationType(It.IsAny<Guid>())).ReturnsAsync(2);

            _updateOrganisationRepository.Setup(x =>
                    x.UpdateProviderTypeAndOrganisationType(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(),
                        It.IsAny<string>()))
                .ReturnsAsync(true).Verifiable();

            _updateOrganisationRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()))
                .ReturnsAsync(true).Verifiable();

            var result = _handler.Handle(_request, new CancellationToken()).Result;

            result.Should().BeTrue();
            _updateOrganisationRepository.VerifyAll();
        }

        [Test]
        public void Handler_does_not_update_audit_history_if_provider_type_not_changed()
        {
            _request = new UpdateOrganisationProviderTypeRequest
            {
                OrganisationId = Guid.NewGuid(),
                OrganisationTypeId = 3,
                ProviderTypeId = ProviderType.MainProvider,
                UpdatedBy = "test"
            };

            _repository.Setup(x => x.GetProviderType(It.IsAny<Guid>())).ReturnsAsync(1);
            _repository.Setup(x => x.GetOrganisationType(It.IsAny<Guid>())).ReturnsAsync(3);

            _updateOrganisationRepository.Setup(x =>
                    x.UpdateProviderTypeAndOrganisationType(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(true).Verifiable();

            _updateOrganisationRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()))
                .ReturnsAsync(true).Verifiable();

            var result = _handler.Handle(_request, new CancellationToken()).Result;

            result.Should().BeFalse();
            _updateOrganisationRepository.Verify(x => x.UpdateProviderTypeAndOrganisationType(It.IsAny<Guid>(), It.IsAny<int>(),
                It.IsAny<int>(), It.IsAny<string>()), Times.Never());
            _updateOrganisationRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
        }

    }
}
