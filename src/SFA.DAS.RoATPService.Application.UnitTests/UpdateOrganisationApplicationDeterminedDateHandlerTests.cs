using System.Collections.Generic;
using SFA.DAS.RoATPService.Application.Services;

namespace SFA.DAS.RoATPService.Application.UnitTests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using FluentAssertions;
    using Handlers;
    using Interfaces;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using Exceptions;
    using Domain;
    using Validators;

    [TestFixture]
    public class UpdateOrganisationApplicationDeterminedDateHandlerTests
    {
        private Mock<ILogger<UpdateOrganisationApplicationDeterminedDateHandler>> _logger;
        private Mock<IOrganisationValidator> _validator;
        private Mock<IUpdateOrganisationRepository> _updateOrganisationRepository;
        private UpdateOrganisationApplicationDeterminedDateHandler _handler;
        private UpdateOrganisationApplicationDeterminedDateRequest _request;

        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<UpdateOrganisationApplicationDeterminedDateHandler>>();
            _validator = new Mock<IOrganisationValidator>();
            _validator.Setup(x => x.IsValidApplicationDeterminedDate(It.IsAny<DateTime?>())).Returns(true);
           
            _updateOrganisationRepository = new Mock<IUpdateOrganisationRepository>();
           
            _handler = new UpdateOrganisationApplicationDeterminedDateHandler(_logger.Object, _validator.Object,
                _updateOrganisationRepository.Object);
            _request = new UpdateOrganisationApplicationDeterminedDateRequest
            {
                OrganisationId = Guid.NewGuid(),
                ApplicationDeterminedDate = DateTime.Today,
                UpdatedBy = "test"
            };
        }

        [Test]
        public void Handler_rejects_request_with_invalid_application_determined_date()
        {
            _validator.Setup(x => x.IsValidApplicationDeterminedDate(It.IsAny<DateTime?>())).Returns(false);

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [Test]
        public void Handler_updates_application_determined_date()
        {
        
            _updateOrganisationRepository.Setup(x =>
                    x.UpdateApplicationDeterminedDate(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .ReturnsAsync(true).Verifiable();

            var result = _handler.Handle(_request, new CancellationToken()).Result;

            result.Should().BeTrue();
            _updateOrganisationRepository.VerifyAll();
        }
    }
}

