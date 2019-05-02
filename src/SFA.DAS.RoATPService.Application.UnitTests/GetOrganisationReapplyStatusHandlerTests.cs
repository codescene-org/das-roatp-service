namespace SFA.DAS.RoATPService.Application.UnitTests
{
    using System;
    using System.Threading;
    using Api.Types.Models;
    using Domain;
    using FluentAssertions;
    using Interfaces;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using SFA.DAS.RoATPService.Application.Handlers;

    public class GetOrganisationReapplyStatusHandlerTests
    {
        private Mock<IOrganisationRepository> _repository;

        private Mock<ILogger<GetOrganisationReapplyStatusHandler>> _logger;

        private GetOrganisationReapplyStatusHandler _handler;

        [SetUp]
        public void Before_each_test()
        {
            _repository = new Mock<IOrganisationRepository>();

            _logger =new Mock<ILogger<GetOrganisationReapplyStatusHandler>>();

            _handler = new GetOrganisationReapplyStatusHandler(_repository.Object, _logger.Object);
        }

        [TestCase(ProviderType.MainProvider, OrganisationStatus.Removed)]
        [TestCase(ProviderType.MainProvider, OrganisationStatus.Active)]
        [TestCase(ProviderType.MainProvider, OrganisationStatus.ActiveNotTakingOnApprentices)]
        [TestCase(ProviderType.MainProvider, OrganisationStatus.Onboarding)]
        [TestCase(ProviderType.EmployerProvider, OrganisationStatus.Removed)]
        [TestCase(ProviderType.EmployerProvider, OrganisationStatus.Active)]
        [TestCase(ProviderType.EmployerProvider, OrganisationStatus.ActiveNotTakingOnApprentices)]
        [TestCase(ProviderType.EmployerProvider, OrganisationStatus.Onboarding)]
        [TestCase(ProviderType.SupportingProvider, OrganisationStatus.Removed)]
        [TestCase(ProviderType.SupportingProvider, OrganisationStatus.Active)]
        [TestCase(ProviderType.SupportingProvider, OrganisationStatus.ActiveNotTakingOnApprentices)]
        public void Handler_returns_all_applicable_provider_types_and_statuses(int providerTypeId, int statusId)
        {
            var reapplyStatus = new OrganisationReapplyStatus
            {
                ProviderTypeId = providerTypeId,
                StatusId = statusId
            };

            

            _repository.Setup(x => x.GetOrganisationReapplyStatus(It.IsAny<Guid>())).ReturnsAsync(reapplyStatus);

            var request = new GetOrganisationReapplyStatusRequest {OrganisationId = Guid.NewGuid()};
            var response = _handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();

            response.ProviderTypeId.Should().Be(providerTypeId);
            response.StatusId.Should().Be(statusId);
        }
    }
}
