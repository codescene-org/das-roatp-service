﻿using SFA.DAS.RoATPService.Application.Commands;
using SFA.DAS.RoATPService.Application.Mappers;

namespace SFA.DAS.RoATPService.Application.UnitTests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Handlers;
    using Interfaces;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using Api.Types.Models;
    using Exceptions;
    using Validators;

    [TestFixture]
    public class CreateOrganisationHandlerTests
    {
        private CreateOrganisationRequest _request;
        private CreateOrganisationHandler _handler;
        private Mock<ICreateOrganisationRepository> _repository;
        private Mock<IEventsRepository> _eventsRepository;
        private Mock<ILogger<CreateOrganisationHandler>> _logger;
        private Mock<IOrganisationValidator> _validator;
        private Mock<IDuplicateCheckRepository> _duplicateCheckRepository;
        private Mock<ITextSanitiser> _textSanitiser;
        private Guid _organisationId;

        private IMapCreateOrganisationRequestToCommand _mapper;
        [SetUp]
        public void Before_each_test()
        {
            _organisationId = Guid.NewGuid();
            _repository = new Mock<ICreateOrganisationRepository>();
            _eventsRepository = new Mock<IEventsRepository>();
            _repository.Setup(x => x.CreateOrganisation(It.IsAny<CreateOrganisationCommand>()))
                .ReturnsAsync(_organisationId);
            _eventsRepository.Setup(
                x => x.AddOrganisationStatusEvents(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<DateTime>())).ReturnsAsync(true);
            _duplicateCheckRepository = new Mock<IDuplicateCheckRepository>();
            _duplicateCheckRepository.Setup(x => x.DuplicateUKPRNExists(It.IsAny<Guid>(), It.IsAny<long>()))
                .ReturnsAsync(new DuplicateCheckResponse {DuplicateOrganisationName = "",DuplicateFound = false});
            _logger = new Mock<ILogger<CreateOrganisationHandler>>();
            _mapper = new MapCreateOrganisationRequestToCommand();
             _validator = new Mock<IOrganisationValidator>();
            _validator.Setup(x => x.IsValidOrganisationTypeId(It.IsAny<int>())).Returns(true);
            _validator.Setup(x => x.IsValidLegalName(It.IsAny<string>())).Returns(true);
            _validator.Setup(x => x.IsValidTradingName(It.IsAny<string>())).Returns(true);
            _validator.Setup(x => x.IsValidProviderTypeId(It.IsAny<int>())).Returns(true);
            _validator.Setup(x => x.IsValidOrganisationTypeId(It.IsAny<int>())).Returns(true);
            _validator.Setup(x => x.IsValidStatusId(It.IsAny<int>())).Returns(true);
            _validator.Setup(x => x.IsValidStatusDate(It.IsAny<DateTime>())).Returns(true);
            _validator.Setup(x => x.IsValidUKPRN(It.IsAny<long>())).Returns(true);
            _validator.Setup(x => x.IsValidCompanyNumber(It.IsAny<string>())).Returns(true);
            _validator.Setup(x => x.IsValidCharityNumber(It.IsAny<string>())).Returns(true);
            _validator.Setup(x => x.IsValidApplicationDeterminedDate(It.IsAny<DateTime?>())).Returns(true);
            _validator.Setup(x => x.DuplicateUkprnInAnotherOrganisation(It.IsAny<long>(), It.IsAny<Guid>()))
                .Returns(new DuplicateCheckResponse {DuplicateFound = false, DuplicateOrganisationName = ""});
            _validator.Setup(x => x.DuplicateCompanyNumberInAnotherOrganisation(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(new DuplicateCheckResponse {DuplicateFound = false, DuplicateOrganisationName = ""});
            _validator.Setup(x => x.DuplicateCharityNumberInAnotherOrganisation(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(new DuplicateCheckResponse { DuplicateFound = false, DuplicateOrganisationName = "" });
            _textSanitiser = new Mock<ITextSanitiser>();
            _textSanitiser.Setup(x => x.SanitiseInputText(It.IsAny<string>())).Returns<string>(x => x);

            _handler = new CreateOrganisationHandler(_repository.Object, _eventsRepository.Object,  _logger.Object, _validator.Object, new ProviderTypeValidator(), _mapper, _textSanitiser.Object);

            _request = new CreateOrganisationRequest
            {                                                                       
                LegalName = "Legal Name",
                TradingName = "TradingName",
                ProviderTypeId = 1,
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
        [TestCase(9)]
        public void Create_organisation_rejects_invalid_provider_type(int providerTypeId)
        {
            _validator.Setup(x => x.IsValidProviderTypeId(It.IsAny<int>())).Returns(false);
            _request.ProviderTypeId = providerTypeId;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [TestCase(-1)]
        [TestCase(21)]
        public void Create_organisation_rejects_invalid_organisation_type(int organisationTypeId)
        {
            _validator.Setup(x => x.IsValidOrganisationTypeId(It.IsAny<int>())).Returns(false);
            _request.OrganisationTypeId = organisationTypeId;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }
     
        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public void Create_organisation_rejects_invalid_legal_name(string legalName)
        {
            _validator.Setup(x => x.IsValidLegalName(It.IsAny<string>())).Returns(false);
            _request.LegalName = legalName;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [Test]
        public void Create_organisation_rejects_legal_name_that_is_too_large()
        {
            _validator.Setup(x => x.IsValidLegalName(It.IsAny<string>())).Returns(false);
            _request.LegalName = new String('A', 201);

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [Test]
        public void Create_organisation_rejects_trading_name_that_is_too_large()
        {
            _validator.Setup(x => x.IsValidTradingName(It.IsAny<string>())).Returns(false);
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
            _validator.Setup(x => x.IsValidUKPRN(It.IsAny<long>())).Returns(false);
            _request.Ukprn = ukprn;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [Test]
        public void Create_organisation_rejects_duplicate_UKPRN()
        {
            _duplicateCheckRepository.Setup(x => x.DuplicateUKPRNExists(It.IsAny<Guid>(), It.IsAny<long>()))
                .ReturnsAsync(new DuplicateCheckResponse {DuplicateOrganisationName = "name", DuplicateFound = true});

            _validator.Setup(x => x.DuplicateUkprnInAnotherOrganisation(It.IsAny<long>(), It.IsAny<Guid>()))
                .Returns(new DuplicateCheckResponse { DuplicateFound = true, DuplicateOrganisationName = "name" });

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [Test]
        public void Create_organisation_rejects_duplicate_company_number()
        {
            _duplicateCheckRepository.Setup(x => x.DuplicateCompanyNumberExists(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(new DuplicateCheckResponse { DuplicateOrganisationName = "name", DuplicateFound = true });

            _validator.Setup(x => x.DuplicateCompanyNumberInAnotherOrganisation(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(new DuplicateCheckResponse { DuplicateFound = true, DuplicateOrganisationName = "name" });

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [Test]
        public void Create_organisation_rejects_duplicate_charity_number()
        {
            _duplicateCheckRepository.Setup(x => x.DuplicateCharityNumberExists(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(new DuplicateCheckResponse { DuplicateOrganisationName = "name", DuplicateFound = true });

            _validator.Setup(x => x.DuplicateCharityNumberInAnotherOrganisation(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(new DuplicateCheckResponse { DuplicateFound = true, DuplicateOrganisationName = "name" });

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
            _validator.Setup(x => x.IsValidCompanyNumber(It.IsAny<string>())).Returns(false);
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
            _validator.Setup(x => x.IsValidCharityNumber(It.IsAny<string>())).Returns(false);
            _request.CharityNumber = charityNumber;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }
    }
}
