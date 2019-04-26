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
    public class LookupDataGetProviderTypesTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private LookupDataRepository _lookupRepository;
        private OrganisationValidator _organisationValidator;

        private int _providerTypeId2;
        private int _providerTypeId1;
        private int _providerTypeIdNonExistent;
        private double _numberOfExpectedResults;
        private ProviderTypeModel _providerType1;
        private ProviderTypeModel _providerType2;

        [OneTimeSetUp]
        public void Before_the_tests()
        {
            _lookupRepository = new LookupDataRepository(null, _databaseService.WebConfiguration);
            _organisationValidator = new OrganisationValidator(null,_lookupRepository,null);
            _providerTypeId1 = 1;
            _providerTypeId2 = 2;
            _providerTypeIdNonExistent = 100;
            _numberOfExpectedResults = 2;

            _providerType1 = new ProviderTypeModel
            {
                Id = _providerTypeId1,
                CreatedAt = DateTime.Now,
                CreatedBy = "system",
                Status = "x",
                ProviderType = "a"
            };
            _providerType2 = new ProviderTypeModel
            {
                Id = _providerTypeId2,
                CreatedAt = DateTime.Now,
                CreatedBy = "system",
                Status = "x",
                ProviderType = "b"
            };
            ProviderTypeHandler.InsertRecord(_providerType1);
            ProviderTypeHandler.InsertRecord(_providerType2);
      }

        [Test]
        public void Get_provider_types()
        {
            var result = _lookupRepository.GetProviderTypes().Result;
            Assert.AreEqual(_numberOfExpectedResults, result.Count());
        }

        [TestCase(1,"a")]
        [TestCase(2,"b")]
        public void Get_provider_type_for_valid_id(int providerTypeId, string providerType)
        {
            var result = _lookupRepository.GetProviderType(providerTypeId).Result;
            Assert.AreEqual(providerTypeId, result.Id);
            Assert.AreEqual(providerType, result.Type);
        }

        [TestCase(1, true)]
        [TestCase(2, true)]
        [TestCase(3, false)]
        public void Check_provider_type_valid_is_returning_expected_result(int providerTypeId, bool expectedResult)
        {
            var result = _organisationValidator.IsValidProviderTypeId(providerTypeId);
            Assert.AreEqual(expectedResult,result);
        }

        [Test]
        public void Get_null_provider_type_for_invalid_id()
        {
            var result = _lookupRepository.GetProviderType(_providerTypeIdNonExistent).Result;
            Assert.IsNull(result);
        }

        [OneTimeTearDown]
        public void Tear_down()
        {
            ProviderTypeHandler.DeleteAllRecords();
        }
    }
}
