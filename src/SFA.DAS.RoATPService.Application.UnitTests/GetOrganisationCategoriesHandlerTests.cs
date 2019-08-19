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
    public class GetOrganisationCategoriesHandlerTests
    {
        private GetOrganisationCategoriesHandler _handler;
        private Mock<ILookupDataRepository> _repository;
        private Mock<ILogger<GetOrganisationCategoriesHandler>> _logger;
        private IProviderTypeValidator _validator;

        [SetUp]
        public void Before_each_test()
        {
            _repository = new Mock<ILookupDataRepository>();
            var organisationCategories = new List<OrganisationCategory>
            {
                new OrganisationCategory
                {
                    Id = 1,
                    Category = "Educational Institute"
                },
                new OrganisationCategory
                {
                    Id = 2,
                    Category = "Public Sector Body"
                }
            };
            _repository.Setup(x => x.GetOrganisationCategories(It.IsAny<int>())).ReturnsAsync(organisationCategories);
            _logger = new Mock<ILogger<GetOrganisationCategoriesHandler>>();
            _validator = new ProviderTypeValidator();
            _handler = new GetOrganisationCategoriesHandler(_repository.Object, _logger.Object, _validator);
        }


        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Handler_retrieves_list_of_organisations_by_provider_type(int providerTypeId)
        {
            var request = new GetOrganisationCategoriesRequest { ProviderTypeId = providerTypeId };

            var result = _handler.Handle(request, new CancellationToken()).Result;

            result.Should().NotBeNullOrEmpty();
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(4)]
        public void Handler_returns_bad_request_for_invalid_provider_type(int providerTypeId)
        {
            var request = new GetOrganisationCategoriesRequest { ProviderTypeId = providerTypeId };

            Func<Task> result = async () => await
                _handler.Handle(request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [Test]
        public void Handler_returns_server_error_for_repository_exception()
        {
            var request = new GetOrganisationCategoriesRequest { ProviderTypeId = 1 };

            _repository.Setup(x => x.GetOrganisationCategories(It.IsAny<int>()))
                .Throws(new Exception("Unit test exception"));

            Func<Task> result = async () => await
                _handler.Handle(request, new CancellationToken());
            result.Should().Throw<ApplicationException>();
        }
    }
}
