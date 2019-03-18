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

        [SetUp]
        public void Before_each_test()
        {
            _repository = new Mock<IOrganisationRepository>();
            _repository.Setup(x => x.CreateOrganisation(It.IsAny<Organisation>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            _logger = new Mock<ILogger<CreateOrganisationHandler>>();
            _handler = new CreateOrganisationHandler(_repository.Object, _logger.Object, new OrganisationValidator());
            _request = new CreateOrganisationRequest
            {
                Organisation = new Organisation
                {
                    LegalName = "Legal Name",
                    TradingName = "Trading Name",
                    ProviderType = new ProviderType
                    {
                        Id = 1,
                        Type = "Main provider"
                    },
                    OrganisationStatus = new OrganisationStatus
                    {
                        Id = 1,
                        Status = "Active"
                    },
                    OrganisationType = new OrganisationType
                    {
                        Id = 0,
                        Type = "Unassigned"
                    },
                    Status = "Live",
                    StatusDate = DateTime.Now,
                    UKPRN = 10002000,
                    OrganisationData = new OrganisationData
                    {
                        CompanyNumber = "11223344",
                        CharityNumber = "10000000",
                        FinancialTrackRecord = true,
                        NonLevyContract = false,
                        ParentCompanyGuarantee = false
                    }
                },
                Username = "Test User"
            };
        }

        [Test]
        public void Create_organisation_handler_saves_organisation_to_database()
        {
            var result = _handler.Handle(_request, new CancellationToken()).GetAwaiter().GetResult();

            result.Should().BeTrue();
        }

        [Test]
        public void Create_organisation_handler_rejects_null_provider_type()
        {
            _request.Organisation.ProviderType = null;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(4)]
        public void Create_organisation_rejects_invalid_provider_type(int providerTypeId)
        {
            _request.Organisation.ProviderType.Id = providerTypeId;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [Test]
        public void Create_organisation_rejects_null_organisation_type()
        {
            _request.Organisation.OrganisationType = null;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [TestCase(-1)]
        [TestCase(7)]
        public void Create_organisation_rejects_invalid_organisation_type(int organisationTypeId)
        {
            _request.Organisation.OrganisationType.Id = organisationTypeId;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [Test]
        public void Create_organisation_rejects_null_organisation_status()
        {
            _request.Organisation.OrganisationStatus = null;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }
        
        [TestCase(-1)]
        [TestCase(3)]
        public void Create_organisation_rejects_invalid_organisation_status(int organisationStatusId)
        {
            _request.Organisation.OrganisationStatus.Id = organisationStatusId;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public void Create_organisation_rejects_invalid_legal_name(string legalName)
        {
            _request.Organisation.LegalName = legalName;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [Test]
        public void Create_organisation_rejects_legal_name_that_is_too_large()
        {
            _request.Organisation.LegalName = new String('A', 201);

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [Test]
        public void Create_organisation_rejects_trading_name_that_is_too_large()
        {
            _request.Organisation.TradingName = new String('A', 201);

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
            _request.Organisation.UKPRN = ukprn;

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
            _request.Organisation.OrganisationData.CompanyNumber = companyNumber;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [TestCase("1000$!&*^%")]
        [TestCase("!£$%^&*()")]
        [TestCase("010101888-1££££''''")]
        public void Create_organisation_rejects_invalid_charity_number(string charityNumber)
        {
            _request.Organisation.OrganisationData.CharityNumber = charityNumber;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }
    }
}
