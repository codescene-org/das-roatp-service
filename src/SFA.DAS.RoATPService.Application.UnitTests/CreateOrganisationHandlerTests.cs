using SFA.DAS.RoATPService.Application.Commands;

namespace SFA.DAS.RoATPService.Application.UnitTests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain;
    using FluentAssertions;
    using Handlers;
    using Interfaces;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using SFA.DAS.RoATPService.Api.Types.Models;
    using SFA.DAS.RoATPService.Application.Exceptions;
    using Validators;

    [TestFixture]
    public class CreateOrganisationHandlerTests
    {
        private CreateOrganisationRequest _request;
        private CreateOrganisationHandler _handler;
        private Mock<IOrganisationRepository> _repository;
        private Mock<ILogger<CreateOrganisationHandler>> _logger;
        private Mock<ILookupDataRepository> _lookupDataRepository;
        private Guid _organisationId;

        [SetUp]
        public void Before_each_test()
        {
            _organisationId = Guid.NewGuid();
            _repository = new Mock<IOrganisationRepository>();
            _repository.Setup(x => x.CreateOrganisation(It.IsAny<CreateOrganisationCommand>()))
                .ReturnsAsync(_organisationId);
            _logger = new Mock<ILogger<CreateOrganisationHandler>>();
            _lookupDataRepository = new Mock<ILookupDataRepository>();
            _handler = new CreateOrganisationHandler(_repository.Object, _logger.Object, new OrganisationValidator(_lookupDataRepository.Object), new ProviderTypeValidator());
            _request = new CreateOrganisationRequest
            {                                                                       
                LegalName = "Legal Name",
                TradingName = "TradingName",
                ProviderTypeId = 1,
                OrganisationStatusId =  1,
                OrganisationTypeId = 0,
                StatusDate = DateTime.Now,
                Ukprn = 10002000,
                CompanyNumber = "11223344",
                CharityNumber = "10000000",
                Username = "Test User"
            };
        }

        [Test]
        public void Create_organisation_handler_saves_organisation_to_database()
        {
            var result = _handler.Handle(_request, new CancellationToken()).GetAwaiter().GetResult();

            result.Should().Equals(_organisationId);
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(4)]
        public void Create_organisation_rejects_invalid_provider_type(int providerTypeId)
        {
            _request.ProviderTypeId = providerTypeId;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [TestCase(-1)]
        [TestCase(7)]
        public void Create_organisation_rejects_invalid_organisation_type(int organisationTypeId)
        {
            _request.OrganisationTypeId = organisationTypeId;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }
     
        [TestCase(-1)]
        [TestCase(3)]
        public void Create_organisation_rejects_invalid_organisation_status(int organisationStatusId)
        {
            _request.OrganisationStatusId = organisationStatusId;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public void Create_organisation_rejects_invalid_legal_name(string legalName)
        {
            _request.LegalName = legalName;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [Test]
        public void Create_organisation_rejects_legal_name_that_is_too_large()
        {
            _request.LegalName = new String('A', 201);

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [Test]
        public void Create_organisation_rejects_trading_name_that_is_too_large()
        {
            _request.TradingName = new String('A', 201);

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }
        
        [TestCase(0)]
        [TestCase(1000000)]
        [TestCase(9999999)]
        [TestCase(100000000)]
        public void Create_organisation_rejects_invalid_UKPRN(long ukprn)
        {
            _request.Ukprn = ukprn;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [TestCase("1234567")]
        [TestCase("ABC12345")]
        [TestCase("1000$!&*^%")]
        [TestCase("!£$%^&*()")]
        public void Create_organisation_rejects_invalid_company_number(string companyNumber)
        {
            _request.CompanyNumber = companyNumber;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [TestCase("1000$!&*^%")]
        [TestCase("!£$%^&*()")]
        [TestCase("010101888-1££££''''")]
        public void Create_organisation_rejects_invalid_charity_number(string charityNumber)
        {
            _request.CharityNumber = charityNumber;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }
    }
}
