namespace SFA.DAS.RoATPService.Application.UnitTests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using SFA.DAS.RoATPService.Api.Types.Models.UpdateOrganisation;
    using SFA.DAS.RoATPService.Application.Exceptions;
    using SFA.DAS.RoATPService.Application.Handlers;
    using SFA.DAS.RoATPService.Application.Interfaces;
    using SFA.DAS.RoATPService.Application.Validators;
    using SFA.DAS.RoATPService.Domain;

    [TestFixture]
    public class UpdateOrganisationTradingNameHandlerTests
    {
        private Mock<ILogger<UpdateOrganisationTradingNameHandler>> _logger;
        private Mock<IOrganisationValidator> _validator;
        private Mock<IUpdateOrganisationRepository> _repository;
        private Mock<IAuditLogRepository> _auditRepository;
        private UpdateOrganisationTradingNameHandler _handler;

        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<UpdateOrganisationTradingNameHandler>>();
            _validator = new Mock<IOrganisationValidator>();
            _validator.Setup(x => x.IsValidTradingName(It.IsAny<string>())).Returns(true);
            _repository = new Mock<IUpdateOrganisationRepository>();
            _repository.Setup(x => x.GetTradingName(It.IsAny<Guid>())).ReturnsAsync("existing trading name").Verifiable();
            _repository.Setup(x => x.UpdateTradingName(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true).Verifiable();
            _auditRepository = new Mock<IAuditLogRepository>();
            _auditRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>())).ReturnsAsync(true).Verifiable();

            _handler = new UpdateOrganisationTradingNameHandler(_logger.Object, _validator.Object, _repository.Object, _auditRepository.Object);
        }

        [Test]
        public void Handler_does_not_update_database_if_trading_name_invalid()
        {
            _validator.Setup(x => x.IsValidTradingName(It.IsAny<string>())).Returns(false);

            var request = new UpdateOrganisationTradingNameRequest
            {
                TradingName = "trading name x%%%%",
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };

            Func<Task> result = async () => await
                _handler.Handle(request, new CancellationToken());
            result.Should().Throw<BadRequestException>();

            _repository.Verify(x => x.GetTradingName(It.IsAny<Guid>()), Times.Never);
            _repository.Verify(x => x.UpdateTradingName(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _auditRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
        }

        [Test]
        public void Handler_does_not_update_database_if_Trading_name_unchanged()
        {
            var request = new UpdateOrganisationTradingNameRequest
            {
                TradingName = "existing trading name",
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };
            
            var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
            result.Should().BeFalse();

            _repository.Verify(x => x.GetTradingName(It.IsAny<Guid>()), Times.Once);
            _repository.Verify(x => x.UpdateTradingName(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _auditRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
        }

        [TestCase("", null)]
        [TestCase("", " ")]
        [TestCase(null, null)]
        [TestCase(null, "")]
        [TestCase(" ", null)]
        public void Handler_does_not_update_database_if_both_existing_and_new_trading_names_are_whitespace(string existingTradingName, string updateTradingName)
        {
            var request = new UpdateOrganisationTradingNameRequest
            {
                TradingName = updateTradingName,
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };
            
            _repository.Setup(x => x.GetTradingName(It.IsAny<Guid>())).ReturnsAsync(existingTradingName).Verifiable();

            var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
            result.Should().BeFalse();

            _repository.Verify(x => x.GetTradingName(It.IsAny<Guid>()), Times.Once);
            _repository.Verify(x => x.UpdateTradingName(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _auditRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
        }

        [Test]
        public void Handler_does_not_write_audit_log_entry_if_save_operation_fails()
        {
            _repository.Setup(x => x.UpdateTradingName(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false).Verifiable();

            var request = new UpdateOrganisationTradingNameRequest
            {
                TradingName = "new trading name",
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };

            var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
            result.Should().BeFalse();

            _repository.Verify(x => x.GetTradingName(It.IsAny<Guid>()), Times.Once);
            _repository.Verify(x => x.UpdateTradingName(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _auditRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
        }

        [Test]
        public void Handler_writes_updated_Trading_name_and_audit_log_entry_to_database()
        {
            var request = new UpdateOrganisationTradingNameRequest
            {
                TradingName = "new trading name",
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };

            var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
            result.Should().BeTrue();

            _repository.Verify(x => x.GetTradingName(It.IsAny<Guid>()), Times.Once);
            _repository.Verify(x => x.UpdateTradingName(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _auditRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Once);
        }
    }
}
