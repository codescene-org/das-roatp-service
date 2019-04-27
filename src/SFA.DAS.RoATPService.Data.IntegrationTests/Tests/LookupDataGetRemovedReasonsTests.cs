using System.Linq;
using NUnit.Framework;
using SFA.DAS.RoatpService.Data.IntegrationTests.Handlers;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;
using SFA.DAS.RoATPService.Data;
using SFA.DAS.RoATPService.Data.Helpers;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Tests
{
    public class LookupDataGetRemovedReasonsTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private LookupDataRepository _lookupRepository;
        private readonly CacheHelper _cacheHelper = new CacheHelper();
        private int _removedReasonId2;
        private int _removedReasonId1;
        private int _removedReasonNonExistent;
        private double _numberOfExpectedResults;
        private RemovedReasonModel _removedReason1;
        private RemovedReasonModel _removedReason2;

        [OneTimeSetUp]
        public void Before_the_tests()
        {
            _lookupRepository = new LookupDataRepository(null, _databaseService.WebConfiguration, _cacheHelper);
            _removedReasonId1 = 1;
            _removedReasonId2 = 2;
            _removedReasonNonExistent = 100;
            _numberOfExpectedResults = 2;

            _removedReason1 = new RemovedReasonModel
            {
                Id = _removedReasonId1,
                CreatedBy = "system",
                Status = "x",
                Reason = "a",
                Description = "description a"
            };
            _removedReason2 = new RemovedReasonModel
            {
                Id = _removedReasonId2,
                CreatedBy = "system",
                Status = "x",
                Reason = "b",
                Description = "description b"
            };
            RemovedReasonHandler.InsertRecord(_removedReason1);
            RemovedReasonHandler.InsertRecord(_removedReason2);
        }

        [Test]
        public void Get_removed_reasons()
        {
            var result = _lookupRepository.GetRemovedReasons().Result;
            Assert.AreEqual(_numberOfExpectedResults, result.Count());
        }

        [TestCase(1, "a")]
        [TestCase(2, "b")]
        public void Get_removed_reason_for_valid_id(int removedReasonId, string removedReason)
        {
            var result = _lookupRepository.GetRemovedReason(removedReasonId).Result;
            Assert.AreEqual(removedReasonId, result.Id);
            Assert.AreEqual(removedReason, result.Reason);
        }

        [Test]
        public void Get_null_provider_type_for_invalid_id()
        {
            var result = _lookupRepository.GetRemovedReason(_removedReasonNonExistent).Result;
            Assert.IsNull(result);
        }

        [OneTimeTearDown]
        public void Tear_down()
        {
            RemovedReasonHandler.DeleteAllRecords();
        }
    }
}
