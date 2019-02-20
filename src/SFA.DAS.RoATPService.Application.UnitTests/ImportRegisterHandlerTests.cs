namespace SFA.DAS.RoATPService.Application.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using FluentAssertions;
    using Handlers;
    using Importer;
    using Importer.Exceptions;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using SFA.DAS.RoATPService.Application.Exceptions;

    [TestFixture]
    public class ImportRegisterHandlerTests
    {
        private Mock<IRegisterImportRepository> _repository;
        private Mock<ILogger<ImportRegisterHandler>> _logger;
        private ImportRegisterHandler _handler;

        private RegisterImportRequest _request;

        [SetUp]
        public void Before_each_test()
        {
            _repository = new Mock<IRegisterImportRepository>();
            _logger = new Mock<ILogger<ImportRegisterHandler>>();
            _handler = new ImportRegisterHandler(_repository.Object, _logger.Object);
            _request = new RegisterImportRequest
            {
                BlobReference = "blobref",
                ContainerName = "container"
            };
        }

        [Test]
        public void Import_register_returns_successful_result()
        {
            var successfulResult = new RegisterImportResultsResponse
            {
                ElapsedTimeMs = 1122,
                EntriesImported = 100,
                ErrorMessages = new List<string>(),
                Success = true
            };

            _repository.Setup(x => x.ImportRegisterData(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(successfulResult);

            var response = _handler.Handle(_request, new CancellationToken()).GetAwaiter().GetResult();

            response.ElapsedTimeMs.Should().Be(successfulResult.ElapsedTimeMs);
            response.EntriesImported.Should().Be(successfulResult.EntriesImported);
            response.ErrorMessages.Count.Should().Be(0);
            response.Success.Should().Be(successfulResult.Success);
        }

        [Test]
        public void Import_register_returns_unsuccessful_result_with_error_messages()
        {
            var successfulResult = new RegisterImportResultsResponse
            {
                ElapsedTimeMs = 1122,
                EntriesImported = 0,
                ErrorMessages = new List<string>
                {
                    "Invalid headers"
                },
                Success = false
            };

            _repository.Setup(x => x.ImportRegisterData(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(successfulResult);

            var response = _handler.Handle(_request, new CancellationToken()).GetAwaiter().GetResult();

            response.ElapsedTimeMs.Should().Be(successfulResult.ElapsedTimeMs);
            response.EntriesImported.Should().Be(successfulResult.EntriesImported);
            response.ErrorMessages.Count.Should().Be(1);
            response.Success.Should().Be(successfulResult.Success);
        }

        [Test]
        public void Import_register_throws_exception()
        {
            _repository.Setup(x => x.ImportRegisterData(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new RegisterImportException("Import error")
                {
                    UKPRN = 10001111,
                    ImportErrorMessage = "Invalid legal name"
                });

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }
    }
}
