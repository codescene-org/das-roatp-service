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
    public class GetOrganisationTypesByCategoryHandlerTests
    {

        private GetOrganisationTypesByCategoryHandler _handler;
        private Mock<ILookupDataRepository> _repository;
        private Mock<ILogger<GetOrganisationTypesByCategoryHandler>> _logger;
        private IProviderTypeValidator _providerTypeValidator;
        private IOrganisationCategoryValidator _categoryValidator;

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
            _repository.Setup(x => x.GetOrganisationTypesForProviderTypeIdCategoryId(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(organisationTypes);
            _logger = new Mock<ILogger<GetOrganisationTypesByCategoryHandler>>();
            _providerTypeValidator = new ProviderTypeValidator();
            _categoryValidator = new OrganisationCategoryValidator(_repository.Object);
            _handler = new GetOrganisationTypesByCategoryHandler(_repository.Object, _logger.Object, _providerTypeValidator, _categoryValidator);
        }


        [TestCase(1,1)]
        [TestCase(2,1)]
        [TestCase(3,1)]
        public void Handler_retrieves_list_of_organisations_by_provider_type(int providerTypeId, int categoryId)
        {
            _repository.Setup((x => x.GetValidOrganisationCategoryIds())).ReturnsAsync(new List<int>{categoryId});
            var request = new GetOrganisationTypesByCategoryRequest { ProviderTypeId = providerTypeId, CategoryId = categoryId};

            var result = _handler.Handle(request, new CancellationToken()).Result;

            result.Should().NotBeNullOrEmpty();
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(4)]
        public void Handler_returns_bad_request_for_invalid_provider_type(int providerTypeId)
        {
            var request = new GetOrganisationTypesByCategoryRequest { ProviderTypeId = providerTypeId, CategoryId = 1};

            Func<Task> result = async () => await
                _handler.Handle(request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [Test]
        public void Handler_returns_server_error_for_repository_exception()
        {
            var request = new GetOrganisationTypesByCategoryRequest { ProviderTypeId = 1 };

            _repository.Setup(x => x.GetOrganisationTypesForProviderTypeIdCategoryId(It.IsAny<int>(), It.IsAny<int>()))
                .Throws(new Exception("Unit test exception"));

            Func<Task> result = async () => await
                _handler.Handle(request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }
    }
}
