namespace SFA.DAS.RoATPService.Application.UnitTests
{
    using Interfaces;
    using Moq;
    using NUnit.Framework;
    using System;
    using System.Threading;
    using Api.Types.Models;
    using Castle.Core.Logging;
    using FluentAssertions;
    using Handlers;
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;
    using System.Xml.XPath;

    [TestFixture]
    public class DuplicateCheckHandlerTests
    {
        private Mock<IDuplicateCheckRepository> _repository;

        [SetUp]
        public void Before_each_test()
        {
            _repository = new Mock<IDuplicateCheckRepository>();
        }

        [Test]
        public void Duplicate_UKPRN_check_returns_match()
        {
            var response = new DuplicateCheckResponse
            {
                DuplicateFound = true, DuplicateOrganisationName = "Legal Name", DuplicateOrganisationId = Guid.NewGuid()
            };

            _repository.Setup(x => x.DuplicateUKPRNExists(It.IsAny<Guid>(), It.IsAny<long>())).ReturnsAsync(response);

            var logger = new Mock<ILogger<DuplicateUKPRNCheckHandler>>();
            var handler = new DuplicateUKPRNCheckHandler(logger.Object, _repository.Object);

            var request = new DuplicateUKPRNCheckRequest
            {
                UKPRN = 10001000,
                OrganisationId = Guid.NewGuid()
            };
            var result = handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();

            result.DuplicateFound.Should().BeTrue();
            result.DuplicateOrganisationId.Should().Be(response.DuplicateOrganisationId);
            result.DuplicateOrganisationName.Should().Be(response.DuplicateOrganisationName);
        }

        [Test]
        public void Duplicate_UKPRN_check_returns_no_match()
        {
            var response = new DuplicateCheckResponse
            {
                DuplicateFound = false,
                DuplicateOrganisationName = null
            };

            _repository.Setup(x => x.DuplicateUKPRNExists(It.IsAny<Guid>(), It.IsAny<long>())).ReturnsAsync(response);

            var logger = new Mock<ILogger<DuplicateUKPRNCheckHandler>>();
            var handler = new DuplicateUKPRNCheckHandler(logger.Object, _repository.Object);

            var request = new DuplicateUKPRNCheckRequest
            {
                UKPRN = 99999999,
                OrganisationId = Guid.NewGuid()
            };
            var result = handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();

            result.DuplicateFound.Should().BeFalse();
        }

        [Test]
        public void Duplicate_UKPRN_check_throws_exception()
        {
            _repository.Setup(x => x.DuplicateUKPRNExists(It.IsAny<Guid>(), It.IsAny<long>()))
                .Throws(new Exception("Unit test exception"));

            var logger = new Mock<ILogger<DuplicateUKPRNCheckHandler>>();
            var handler = new DuplicateUKPRNCheckHandler(logger.Object, _repository.Object);

            var request = new DuplicateUKPRNCheckRequest
            {
                UKPRN = 10001000,
                OrganisationId = Guid.NewGuid()
            };
            
            Func<Task> result = async () => await
                handler.Handle(request, new CancellationToken());
            result.Should().Throw<ApplicationException>();
        }

        [Test]
        public void Duplicate_company_number_check_returns_match()
        {
            var response = new DuplicateCheckResponse
            {
                DuplicateFound = true,
                DuplicateOrganisationName = "Legal Name"
            };

            _repository.Setup(x => x.DuplicateCompanyNumberExists(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(response);

            var logger = new Mock<ILogger<DuplicateCompanyNumberCheckHandler>>();
            var handler = new DuplicateCompanyNumberCheckHandler(logger.Object, _repository.Object);

            var request = new DuplicateCompanyNumberCheckRequest
            {
                CompanyNumber = "10001000",
                OrganisationId = Guid.NewGuid()
            };
            var result = handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();

            result.DuplicateFound.Should().BeTrue();
        }

        [Test]
        public void Duplicate_company_number_check_returns_no_match()
        {
            var response = new DuplicateCheckResponse
            {
                DuplicateFound = false,
                DuplicateOrganisationName = null
            };
            _repository.Setup(x => x.DuplicateCompanyNumberExists(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(response);

            var logger = new Mock<ILogger<DuplicateCompanyNumberCheckHandler>>();
            var handler = new DuplicateCompanyNumberCheckHandler(logger.Object, _repository.Object);

            var request = new DuplicateCompanyNumberCheckRequest
            {
                CompanyNumber = "10001000",
                OrganisationId = Guid.NewGuid()
            };
            var result = handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();

            result.DuplicateFound.Should().BeFalse();
        }

        [Test]
        public void Duplicate_company_number_check_throws_exception()
        {
            _repository.Setup(x => x.DuplicateCompanyNumberExists(It.IsAny<Guid>(), It.IsAny<string>()))
                .Throws(new Exception("Unit test exception"));

            var logger = new Mock<ILogger<DuplicateCompanyNumberCheckHandler>>();
            var handler = new DuplicateCompanyNumberCheckHandler(logger.Object, _repository.Object);

            var request = new DuplicateCompanyNumberCheckRequest
            {
                CompanyNumber = "10001000",
                OrganisationId = Guid.NewGuid()
            };
            
            Func<Task> result = async () => await
                handler.Handle(request, new CancellationToken());
            result.Should().Throw<ApplicationException>();
        }

        [Test]
        public void Duplicate_charity_number_check_returns_match()
        {
            var response = new DuplicateCheckResponse
            {
                DuplicateFound = true,
                DuplicateOrganisationName = "Legal Name"
            };

            _repository.Setup(x => x.DuplicateCharityNumberExists(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(response);

            var logger = new Mock<ILogger<DuplicateCharityNumberCheckHandler>>();
            var handler = new DuplicateCharityNumberCheckHandler(logger.Object, _repository.Object);

            var request = new DuplicateCharityNumberCheckRequest()
            {
                CharityNumber = "10001000",
                OrganisationId = Guid.NewGuid()
            };
            var result = handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();

            result.DuplicateFound.Should().BeTrue();
        }

        [Test]
        public void Duplicate_charity_number_check_returns_no_match()
        {
            var response = new DuplicateCheckResponse
            {
                DuplicateFound = false,
                DuplicateOrganisationName = null
            };

            _repository.Setup(x => x.DuplicateCharityNumberExists(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(response);

            var logger = new Mock<ILogger<DuplicateCharityNumberCheckHandler>>();
            var handler = new DuplicateCharityNumberCheckHandler(logger.Object, _repository.Object);

            var request = new DuplicateCharityNumberCheckRequest()
            {
                CharityNumber = "10001000",
                OrganisationId = Guid.NewGuid()
            };
            var result = handler.Handle(request, new CancellationToken()).GetAwaiter().GetResult();

            result.DuplicateFound.Should().BeFalse();
        }

        [Test]
        public void Duplicate_charity_number_check_throws_exception()
        {
            _repository.Setup(x => x.DuplicateCharityNumberExists(It.IsAny<Guid>(), It.IsAny<string>()))
                .Throws(new Exception("Unit test exception"));

            var logger = new Mock<ILogger<DuplicateCharityNumberCheckHandler>>();
            var handler = new DuplicateCharityNumberCheckHandler(logger.Object, _repository.Object);

            var request = new DuplicateCharityNumberCheckRequest()
            {
                CharityNumber = "10001000",
                OrganisationId = Guid.NewGuid()
            };

            Func<Task> result = async () => await
                handler.Handle(request, new CancellationToken());
            result.Should().Throw<ApplicationException>();
        }
    }
}
