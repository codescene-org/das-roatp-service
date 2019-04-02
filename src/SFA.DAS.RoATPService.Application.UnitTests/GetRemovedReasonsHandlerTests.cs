namespace SFA.DAS.RoATPService.Application.UnitTests
{
    using FluentAssertions;
    using Handlers;
    using Interfaces;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using SFA.DAS.RoATPService.Domain;
    using System.Collections.Generic;
    using System.Threading;
    using Api.Types.Models;
    using System;
    using System.Threading.Tasks;

    [TestFixture]
    public class GetRemovedReasonsHandlerTests
    {
        private GetRemovedReasonsHandler _handler;
        private Mock<ILookupDataRepository> _repository;
        private Mock<ILogger<GetRemovedReasonsHandler>> _logger;

        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<GetRemovedReasonsHandler>>();
            _repository = new Mock<ILookupDataRepository>();
            var removedReasons = new List<RemovedReason>
            {
                new RemovedReason{ Id = 1, Reason = "Provider request" },
                new RemovedReason{ Id = 2, Reason = "Provider error" }
            };
            _repository.Setup(x => x.GetRemovedReasons()).ReturnsAsync(removedReasons);
            _handler = new GetRemovedReasonsHandler(_repository.Object, _logger.Object);
        }

        [Test]
        public void Handler_returns_list_of_removed_reasons()
        {
            var removedReasons = _handler.Handle(new GetRemovedReasonsRequest(), new CancellationToken()).Result;

            removedReasons.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Handler_returns_exception_from_repository()
        {
            _repository.Setup(x => x.GetRemovedReasons())
                .Throws(new Exception("Unit test exception"));

            Func<Task> result = async () => await
                _handler.Handle(new GetRemovedReasonsRequest(), new CancellationToken());
            result.Should().Throw<ApplicationException>();
        }
    }
}
