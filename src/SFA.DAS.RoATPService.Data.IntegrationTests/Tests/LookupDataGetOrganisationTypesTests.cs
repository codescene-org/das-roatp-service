using System;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.RoatpService.Data.IntegrationTests.Handlers;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;
using SFA.DAS.RoATPService.Application.Validators;
using SFA.DAS.RoATPService.Data;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Tests
{
    public class LookupDataGetOrganisationTypesTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private LookupDataRepository _lookupRepository;
        private OrganisationValidator _organisationValidator;

        private int _organisationTypeId1;
        private int _organisationTypeId2;
        private int _organisationTypeId3;
        private int _organisationTypeIdNonExistent;
        private double _numberOfExpectedResults;

        [OneTimeSetUp]
        public void Before_the_tests()
        {
            _lookupRepository = new LookupDataRepository(null, _databaseService.WebConfiguration);
            _organisationValidator = new OrganisationValidator(null,_lookupRepository,null);
            _organisationTypeId1 = 10;
            _organisationTypeId2 = 20;
            _organisationTypeId3 = 30;
            _organisationTypeIdNonExistent = 100;
            _numberOfExpectedResults = 3;


            OrganisationTypeHandler.InsertRecord(new OrganisationTypeModel { Id = _organisationTypeId1, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", Type = "a" });
            OrganisationTypeHandler.InsertRecord(new OrganisationTypeModel { Id = _organisationTypeId2, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", Type = "b" });
            OrganisationTypeHandler.InsertRecord(new OrganisationTypeModel { Id = _organisationTypeId3, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", Type = "c" });
           }

        [Test]
        public void Get_organisation_types()
        {
            var result = _lookupRepository.GetOrganisationTypes().Result;
            Assert.AreEqual(_numberOfExpectedResults, result.Count());
        }

        [TestCase(10, "a")]
        [TestCase(20, "b")]
        [TestCase(30, "c")]
        public void Get_organisation_type_for_valid_id(int organisationTypeId, string organisationType)
        {
            var result = _lookupRepository.GetOrganisationType(organisationTypeId).Result;
            Assert.AreEqual(organisationTypeId, result.Id);
            Assert.AreEqual(organisationType, result.Type);
        }

        [TestCase(10, true)]
        [TestCase(20, true)]
        [TestCase(30, true)]
        [TestCase(40, false)]

        public void Check_organisation_type_valid_is_returning_expected_result(int organisationTypeId, bool expectedResult)
        {
            var result = _organisationValidator.IsValidOrganisationTypeId(organisationTypeId);
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Get_null_provider_type_for_invalid_id()
        {
            var result = _lookupRepository.GetOrganisationType(_organisationTypeIdNonExistent).Result;
            Assert.IsNull(result);
        }

        [OneTimeTearDown]
        public void Tear_down()
        {
            OrganisationTypeHandler.DeleteAllRecords();
        }
    }
}
