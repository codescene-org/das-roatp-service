using System;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.RoatpService.Data.IntegrationTests.Handlers;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;
using SFA.DAS.RoATPService.Data;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Tests
{
    public class LookupDataGetOrganisationTypesTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private LookupDataRepository _lookupRepository;

        private int _providerTypeId2;
        private int _providerTypeId1;
        private int _organisationTypeId1WithProviderTypeId1;
        private int _organisationTypeId4WithProviderTypeId2;
        private int _organisationTypeId2WithProviderTypeId1;
        private int _organisationTypeId3WithProviderTypeId1;

        [OneTimeSetUp]
        public void Before_the_tests()
        {
            _lookupRepository = new LookupDataRepository(null, _databaseService.WebConfiguration);
            _providerTypeId1 = 1;
            _providerTypeId2 = 2;
            _organisationTypeId1WithProviderTypeId1 = 10;
            _organisationTypeId2WithProviderTypeId1 = 20;
            _organisationTypeId3WithProviderTypeId1 = 30;
            _organisationTypeId4WithProviderTypeId2 = 100;

            OrganisationTypeHandler.InsertRecord(new OrganisationTypeModel { Id = _organisationTypeId1WithProviderTypeId1, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", Type="a" });
            OrganisationTypeHandler.InsertRecord(new OrganisationTypeModel { Id = _organisationTypeId2WithProviderTypeId1, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", Type="b"});
            OrganisationTypeHandler.InsertRecord(new OrganisationTypeModel { Id = _organisationTypeId3WithProviderTypeId1, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", Type="c" });
            OrganisationTypeHandler.InsertRecord(new OrganisationTypeModel { Id = _organisationTypeId4WithProviderTypeId2, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x" , Type="d"});
            ProviderTypeHandler.InsertRecord(new ProviderTypeModel { Id = _providerTypeId1, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", ProviderType = "a" });
            ProviderTypeHandler.InsertRecord(new ProviderTypeModel { Id = _providerTypeId2, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", ProviderType = "b" });
            ProviderTypeOrganisationTypeHandler.InsertRecord(new ProviderTypeOrganisationTypeModel { Id = 1, OrganisationTypeId = _organisationTypeId1WithProviderTypeId1, ProviderTypeId = _providerTypeId1 });
            ProviderTypeOrganisationTypeHandler.InsertRecord(new ProviderTypeOrganisationTypeModel { Id = 2, OrganisationTypeId = _organisationTypeId2WithProviderTypeId1, ProviderTypeId = _providerTypeId1 });
            ProviderTypeOrganisationTypeHandler.InsertRecord(new ProviderTypeOrganisationTypeModel { Id = 3, OrganisationTypeId = _organisationTypeId3WithProviderTypeId1, ProviderTypeId = _providerTypeId1 });
            ProviderTypeOrganisationTypeHandler.InsertRecord(new ProviderTypeOrganisationTypeModel { Id = 4, OrganisationTypeId = _organisationTypeId4WithProviderTypeId2, ProviderTypeId = _providerTypeId2 });
        }

        [TestCase(null, 4)]
        [TestCase(1, 3)]
        [TestCase(2, 1)]
        [TestCase(3, 0)]
        public void Get_organisation_types_for_provider_type_id_is_returning_correct_counts(int? providerTypeId, int numberOfExpectedResults)
        {
            var result = _lookupRepository.GetOrganisationTypesForProviderTypeId(providerTypeId).Result;
            Assert.AreEqual(numberOfExpectedResults, result.Count());
        }

        [OneTimeTearDown]
        public void Tear_down()
        {
            ProviderTypeOrganisationTypeHandler.DeleteAllRecords();
            ProviderTypeHandler.DeleteAllRecords();
            OrganisationTypeHandler.DeleteAllRecords();
        }
    }
}
