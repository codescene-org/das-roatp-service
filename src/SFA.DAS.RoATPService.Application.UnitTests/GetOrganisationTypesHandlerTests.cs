namespace SFA.DAS.RoATPService.Application.UnitTests
{
    using System;
    using System.Collections.Generic;
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
    using SFA.DAS.RoATPService.Application.Exceptions;
    using SFA.DAS.RoATPService.Application.Validators;

    [TestFixture]
    public class GetOrganisationTypesHandlerTests
    {
        private GetOrganisationTypesHandler _handler;
        private Mock<ILookupDataRepository> _repository;
        private Mock<ILogger<GetOrganisationTypesHandler>> _logger;
        private IProviderTypeValidator _validator;

        [SetUp]
        public void Before_each_test()
        {
            _repository = new Mock<ILookupDataRepository>();
            var organisationTypes = new List<OrganisationType>
            {
                new OrganisationType
                {
                    Id = 1,
                    Type = "College"
                },
                new OrganisationType
                {
                    Id = 2,
                    Type = "University"
                }
            };
            _repository.Setup(x => x.GetOrganisationTypes(It.IsAny<int>())).ReturnsAsync(organisationTypes);
            _logger = new Mock<ILogger<GetOrganisationTypesHandler>>();
            _validator = new ProviderTypeValidator();
            _handler = new GetOrganisationTypesHandler(_repository.Object, _logger.Object, _validator);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Handler_retrieves_list_of_organisations_by_provider_type(int providerTypeId)
        {
            var request = new GetOrganisationTypesRequest {ProviderTypeId = providerTypeId};

            var result = _handler.Handle(request, new CancellationToken()).Result;

            result.Should().NotBeNullOrEmpty();
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(4)]
        public void Handler_returns_bad_request_for_invalid_provider_type(int providerTypeId)
        {
            var request = new GetOrganisationTypesRequest { ProviderTypeId = providerTypeId };

            Func<Task> result = async () => await
                _handler.Handle(request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [Test]
        public void Handler_returns_server_error_for_repository_exception()
        {
            var request = new GetOrganisationTypesRequest { ProviderTypeId = 1 };

            _repository.Setup(x => x.GetOrganisationTypes(It.IsAny<int>()))
                .Throws(new Exception("Unit test exception"));

            Func<Task> result = async () => await
                _handler.Handle(request, new CancellationToken());
            result.Should().Throw<ApplicationException>();
        }
    }
}
