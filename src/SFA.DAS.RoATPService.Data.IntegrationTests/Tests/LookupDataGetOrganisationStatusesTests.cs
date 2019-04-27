using System;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.RoatpService.Data.IntegrationTests.Handlers;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;
using SFA.DAS.RoATPService.Application.Validators;
using SFA.DAS.RoATPService.Data;
using SFA.DAS.RoATPService.Data.Helpers;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Tests
{
    public class LookupDataGetOrganisationStatusesTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private readonly CacheHelper _cacheHelper = new CacheHelper();
        private LookupDataRepository _lookupRepository;
        private OrganisationValidator _organisationValidator;

        private int _organisationStatusId1;
        private int _organisationStatusId2;
        private int _organisationStatusId3;
        private int _organisationStatusIdNonExistent;
        private double _numberOfExpectedResults;

        [OneTimeSetUp]
        public void Before_the_tests()
        {
            _lookupRepository = new LookupDataRepository(null, _databaseService.WebConfiguration, _cacheHelper);
            _organisationValidator = new OrganisationValidator(null, _lookupRepository, null);
            _organisationStatusId1 = 10;
            _organisationStatusId2 = 20;
            _organisationStatusId3 = 30;
            _organisationStatusIdNonExistent = 100;
            _numberOfExpectedResults = 3;
            _cacheHelper.PurgeAllCaches();

            OrganisationStatusHandler.InsertRecord(new OrganisationStatusModel { Id = _organisationStatusId1, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "a" });
            OrganisationStatusHandler.InsertRecord(new OrganisationStatusModel { Id = _organisationStatusId2, CreatedAt = DateTime.Now, CreatedBy = "system", Status =  "b" });
            OrganisationStatusHandler.InsertRecord(new OrganisationStatusModel { Id = _organisationStatusId3, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "c" });
        }

        [Test]
        public void Get_organisation_statuses()
        {
            var result = _lookupRepository.GetOrganisationStatuses().Result;
            Assert.AreEqual(_numberOfExpectedResults, result.Count());
        }

        [TestCase(10, "a")]
        [TestCase(20, "b")]
        [TestCase(30, "c")]
        public void Get_organisation_status_for_valid_id(int organisationStatusId, string organisationStatus)
        {
            var result = _lookupRepository.GetOrganisationStatus(organisationStatusId).Result;
            Assert.AreEqual(organisationStatusId, result.Id);
            Assert.AreEqual(organisationStatus, result.Status);
        }

        [TestCase(10, true)]
        [TestCase(20, true)]
        [TestCase(30, true)]
        [TestCase(40, false)]

        public void Check_organisation_status_valid_is_returning_expected_result(int organisationStatusId, bool expectedResult)
        {
            var result = _organisationValidator.IsValidStatusId(organisationStatusId);
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Get_null_provider_status_for_invalid_id()
        {
            var result = _lookupRepository.GetOrganisationStatus(_organisationStatusIdNonExistent).Result;
            Assert.IsNull(result);
        }

        [OneTimeTearDown]
        public void Tear_down()
        {
            OrganisationStatusHandler.DeleteAllRecords();
        }
    }
}

