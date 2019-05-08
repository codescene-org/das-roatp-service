namespace SFA.DAS.RoATPService.Application.UnitTests
{
    using FluentAssertions;
    using Handlers;
    using Interfaces;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using RoATPService.Domain;
    using System.Collections.Generic;
    using System.Threading;
    using Api.Types.Models;
    using System;
    using System.Threading.Tasks;

    [TestFixture]
    public class GetOrganisationStatusesHandlerTests
    {
        private GetOrganisationStatusesHandler _handler;
        private Mock<ILookupDataRepository> _repository;
        private Mock<ILogger<GetOrganisationStatusesHandler>> _logger;

        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<GetOrganisationStatusesHandler>>();
            _repository = new Mock<ILookupDataRepository>();
            var organisationStatuses = new List<OrganisationStatus>
            {
                new OrganisationStatus { Id = 0, Status =  "Removed"},
                new OrganisationStatus { Id = 1, Status = "Active" },
                new OrganisationStatus { Id = 2, Status = "Active -not taking on new apprentices"}
            };
            _repository.Setup(x => x.GetOrganisationStatusesForProviderTypeId(It.IsAny<int?>())).ReturnsAsync(organisationStatuses);
            _handler = new GetOrganisationStatusesHandler(_repository.Object, _logger.Object);
        }

        [Test]
        public void Handler_returns_list_of_organisation_statuses()
        {
            var organisationStatuses = _handler.Handle(new GetOrganisationStatusesRequest(), new CancellationToken()).Result;

            organisationStatuses.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Handler_returns_exception_from_repository()
        {
            _repository.Setup(x => x.GetOrganisationStatusesForProviderTypeId(It.IsAny<int?>()))
                .Throws(new Exception("Unit test exception"));

            Func<Task> result = async () => await
                _handler.Handle(new GetOrganisationStatusesRequest(), new CancellationToken());
            result.Should().Throw<ApplicationException>();
        }
    }
}
