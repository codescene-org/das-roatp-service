namespace SFA.DAS.RoATPService.Application.UnitTests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Domain;
    using FluentAssertions;
    using Interfaces;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using SFA.DAS.RoATPService.Application.Exceptions;
    using SFA.DAS.RoATPService.Application.Handlers;
    using Validators;

    public class GetOrganisationRegisterStatusHandlerTests
    {
        private Mock<IOrganisationRepository> _repository;

        private Mock<ILogger<GetOrganisationRegisterStatusHandler>> _logger;

        private GetOrganisationRegisterStatusHandler _handler;

        [SetUp]
        public void Before_each_test()
        {
            _repository = new Mock<IOrganisationRepository>();

            _logger =new Mock<ILogger<GetOrganisationRegisterStatusHandler>>();

            var validator = new OrganisationValidator(new Mock<IDuplicateCheckRepository>().Object,
                new Mock<ILookupDataRepository>().Object, new Mock<IOrganisationRepository>().Object);

            _handler = new GetOrganisationRegisterStatusHandler(_repository.Object, _logger.Object, validator);
        }

        [Test]
        public void Handler_returns_ukprn_not_on_register()
        {
            var organisationRegisterStatus = new OrganisationRegisterStatus
            {
                UkprnOnRegister = false
            };

            _repository.Setup(x => x.GetOrganisationRegisterStatus(It.IsAny<string>()))
                .ReturnsAsync(organisationRegisterStatus);

            var result = _handler.Handle(new GetOrganisationRegisterStatusRequest {UKPRN = "10001234"},
                new CancellationToken()).GetAwaiter().GetResult();

            result.UkprnOnRegister.Should().BeFalse();
            result.OrganisationId.Should().BeNull();
            result.ProviderTypeId.HasValue.Should().BeFalse();
            result.StatusId.HasValue.Should().BeFalse();
            result.RemovedReasonId.HasValue.Should().BeFalse();
        }

        [Test]
        public void Handler_returns_ukprn_on_register_with_active_status()
        {
            var organisationRegisterStatus = new OrganisationRegisterStatus
            {
                UkprnOnRegister = true,
                OrganisationId = Guid.NewGuid(),
                RemovedReasonId = null,
                StatusDate = new DateTime(2018, 1, 1),
                StatusId = OrganisationStatus.Active,
                ProviderTypeId = ProviderType.MainProvider
            };

            _repository.Setup(x => x.GetOrganisationRegisterStatus(It.IsAny<string>()))
                .ReturnsAsync(organisationRegisterStatus);

            var result = _handler.Handle(new GetOrganisationRegisterStatusRequest { UKPRN = "10001234" },
                new CancellationToken()).GetAwaiter().GetResult();

            result.UkprnOnRegister.Should().BeTrue();
            result.OrganisationId.Should().NotBeNull();
            result.ProviderTypeId.Should().Be(ProviderType.MainProvider);
            result.StatusId.Should().Be(OrganisationStatus.Active);
            result.RemovedReasonId.HasValue.Should().BeFalse();
        }

        [Test]
        public void Handler_returns_ukprn_removed_from_register_with_reason_and_date()
        {
            var organisationRegisterStatus = new OrganisationRegisterStatus
            {
                UkprnOnRegister = true,
                OrganisationId = Guid.NewGuid(),
                RemovedReasonId = RemovedReason.Merger,
                StatusDate = new DateTime(2018, 1, 1),
                StatusId = OrganisationStatus.Removed,
                ProviderTypeId = ProviderType.MainProvider
            };

            _repository.Setup(x => x.GetOrganisationRegisterStatus(It.IsAny<string>()))
                .ReturnsAsync(organisationRegisterStatus);

            var result = _handler.Handle(new GetOrganisationRegisterStatusRequest { UKPRN = "10001234" },
                new CancellationToken()).GetAwaiter().GetResult();

            result.UkprnOnRegister.Should().BeTrue();
            result.OrganisationId.Should().NotBeNull();
            result.ProviderTypeId.Should().Be(ProviderType.MainProvider);
            result.StatusId.Should().Be(OrganisationStatus.Removed);
            result.RemovedReasonId.Should().Be(RemovedReason.Merger);
        }

        [Test]
        public void Handler_rejects_UKPRN_containing_non_numeric_characters()
        {
            var organisationRegisterStatus = new OrganisationRegisterStatus
            {
                UkprnOnRegister = false
            };

            _repository.Setup(x => x.GetOrganisationRegisterStatus(It.IsAny<string>()))
                .ReturnsAsync(organisationRegisterStatus);

            Func<Task> result = async () => await
                _handler.Handle(new GetOrganisationRegisterStatusRequest { UKPRN = "1000123A" }, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [TestCase("999999")]
        [TestCase("100000000")]
        public void Handler_rejects_UKPRN_invalid_range(string ukprn)
        {
            var organisationRegisterStatus = new OrganisationRegisterStatus
            {
                UkprnOnRegister = false
            };

            _repository.Setup(x => x.GetOrganisationRegisterStatus(It.IsAny<string>()))
                .ReturnsAsync(organisationRegisterStatus);

            Func<Task> result = async () => await
                _handler.Handle(new GetOrganisationRegisterStatusRequest { UKPRN = "1000123A" }, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

    }
}
