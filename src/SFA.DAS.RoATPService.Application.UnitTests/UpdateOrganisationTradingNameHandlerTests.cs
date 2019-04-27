using SFA.DAS.RoATPService.Api.Types.Models;
using SFA.DAS.RoATPService.Application.Services;

namespace SFA.DAS.RoATPService.Application.UnitTests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using Exceptions;
    using Handlers;
    using Interfaces;
    using Validators;
    using SFA.DAS.RoATPService.Domain;

    [TestFixture]
    public class UpdateOrganisationTradingNameHandlerTests
    {
        private Mock<ILogger<UpdateOrganisationTradingNameHandler>> _logger;
        private Mock<IOrganisationValidator> _validator;
        private Mock<IUpdateOrganisationRepository> _updateRepository;
        private Mock<IOrganisationRepository> _repository;
        private UpdateOrganisationTradingNameHandler _handler;
        private Mock<ITextSanitiser> _textSanitiser;
        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<UpdateOrganisationTradingNameHandler>>();
            _validator = new Mock<IOrganisationValidator>();
            _validator.Setup(x => x.IsValidTradingName(It.IsAny<string>())).Returns(true);
            _updateRepository = new Mock<IUpdateOrganisationRepository>();
            _repository = new Mock<IOrganisationRepository>();
            _repository.Setup(x => x.GetTradingName(It.IsAny<Guid>())).ReturnsAsync("existing trading name").Verifiable();
            _updateRepository.Setup(x => x.UpdateTradingName(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true).Verifiable();
            _updateRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>())).ReturnsAsync(true).Verifiable();
            _textSanitiser = new Mock<ITextSanitiser>();
            _textSanitiser.Setup(x => x.SanitiseInputText(It.IsAny<string>())).Returns<string>(x => x);

            _handler = new UpdateOrganisationTradingNameHandler(_logger.Object, _validator.Object, _updateRepository.Object, _repository.Object, _textSanitiser.Object);
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
            _updateRepository.Verify(x => x.UpdateTradingName(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _updateRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
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
            _updateRepository.Verify(x => x.UpdateTradingName(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _updateRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
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
            _updateRepository.Verify(x => x.UpdateTradingName(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _updateRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
        }

        [Test]
        public void Handler_does_not_write_audit_log_entry_if_save_operation_fails()
        {
            _updateRepository.Setup(x => x.UpdateTradingName(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false).Verifiable();

            var request = new UpdateOrganisationTradingNameRequest
            {
                TradingName = "new trading name",
                OrganisationId = Guid.NewGuid(),
                UpdatedBy = "unit test"
            };

            var result = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();
            result.Should().BeFalse();

            _repository.Verify(x => x.GetTradingName(It.IsAny<Guid>()), Times.Once);
            _updateRepository.Verify(x => x.UpdateTradingName(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _updateRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Never);
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
            _updateRepository.Verify(x => x.UpdateTradingName(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _updateRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()), Times.Once);
        }
    }
}
