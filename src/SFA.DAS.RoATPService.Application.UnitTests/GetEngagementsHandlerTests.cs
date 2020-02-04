using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Api.Types.Models;
using SFA.DAS.RoATPService.Application.Handlers;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Application.UnitTests
{
    [TestFixture]
    public class GetEngagementsHandlerTests
    {
        private GetEngagementsHandler _handler;
        private Mock<IOrganisationRepository> _repository;
        private Mock<ILogger<GetEngagementsHandler>> _logger;

        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<GetEngagementsHandler>>();
            _repository = new Mock<IOrganisationRepository>();
            var engagements = new List<Engagement>
            {
                new Engagement {ProviderId = 11111111, Event = "INITIATED", CreatedOn = DateTime.Today},
                new Engagement {ProviderId = 11111112, Event = "INITIATED", CreatedOn = DateTime.Today.AddDays(-1)},
                new Engagement {ProviderId = 11111113, Event = "INITIATED", CreatedOn = DateTime.Today.AddDays(-2)}
            };
            var request = new GetEngagementsRequest { SinceEventId = 0, PageSize = 1000, PageNumber = 1 };

            _repository.Setup(x => x.GetEngagements(request)).ReturnsAsync(engagements);
            _handler = new GetEngagementsHandler(_repository.Object, _logger.Object);
        }

        [Test]
        public void Handler_returns_list_of_engagements()
        {
            var request = new GetEngagementsRequest { SinceEventId = 0, PageSize = 1000, PageNumber = 1 };
            var engagements = _handler.Handle(request, new CancellationToken()).Result;

            engagements.Should().BeNullOrEmpty(); 
        }

        [TestCase(null, null, null)]
        [TestCase(0, 10, 1)]
        [TestCase(1, 3, 1)]
        public void Handler_paged_returns_list_of_engagements(long sinceEventId = 0, int pageSize = 1000, int pageNumber = 1)
        {
            _logger = new Mock<ILogger<GetEngagementsHandler>>();
            _repository = new Mock<IOrganisationRepository>();

            var engagementsDummy = new List<Engagement>
            {
                new Engagement {ProviderId = 11111111, Event = "INITIATED", CreatedOn = DateTime.Today},
                new Engagement {ProviderId = 11111112, Event = "INITIATED", CreatedOn = DateTime.Today.AddDays(-1)},
                new Engagement {ProviderId = 11111113, Event = "INITIATED", CreatedOn = DateTime.Today.AddDays(-2)},
                new Engagement {ProviderId = 11111114, Event = "REMOVED", CreatedOn = DateTime.Today.AddDays(-3)},
                new Engagement {ProviderId = 11111115, Event = "ACTIVE", CreatedOn = DateTime.Today.AddDays(-4)},
                new Engagement {ProviderId = 11111116, Event = "ACTIVENOSTARTS", CreatedOn = DateTime.Today.AddDays(-5)}
            };

            var request = new GetEngagementsRequest { SinceEventId = sinceEventId, PageSize = pageSize, PageNumber = pageNumber };

            _repository.Setup(x => x.GetEngagements(request)).ReturnsAsync(engagementsDummy);
            _handler = new GetEngagementsHandler(_repository.Object, _logger.Object);

            var engagementsActual = _handler.Handle(request, new CancellationToken()).Result;

            engagementsActual.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Handler_returns_exception_from_repository()
        {
            var request = new GetEngagementsRequest { SinceEventId = 0, PageSize = 1000, PageNumber = 1 };

            _repository.Setup(x => x.GetEngagements(request))
                .Throws(new Exception("Unit test exception"));

            Func<Task> result = async () => await
                _handler.Handle(request, new CancellationToken());
            result.Should().Throw<ApplicationException>();
        }
    }
}