using System;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.RoatpService.Data.IntegrationTests.Handlers;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;
using SFA.DAS.RoATPService.Data;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Tests
{
    public class LookupDataGetOrganisationStatusesTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private LookupDataRepository _lookupRepository;

        private int _providerTypeId2;
        private int _providerTypeId1;
        private int _organisationStatusId1WithProviderTypeId1;
        private int _organisationStatusId4WithProviderTypeId2;
        private int _organisationStatusId2WithProviderTypeId1;
        private int _organisationStatusId3WithProviderTypeId1;

        [OneTimeSetUp]
        public void Before_the_tests()
        {
            _lookupRepository = new LookupDataRepository(null, _databaseService.WebConfiguration);
            _providerTypeId1 = 1;
            _providerTypeId2 = 2;
            _organisationStatusId1WithProviderTypeId1 = 10;
            _organisationStatusId2WithProviderTypeId1 = 20;
            _organisationStatusId3WithProviderTypeId1 = 30;
            _organisationStatusId4WithProviderTypeId2 = 100;

            OrganisationStatusHandler.InsertRecord(new OrganisationStatusModel{Id=_organisationStatusId1WithProviderTypeId1, CreatedAt = DateTime.Now, CreatedBy="system", Status = "x"});
            OrganisationStatusHandler.InsertRecord(new OrganisationStatusModel { Id = _organisationStatusId2WithProviderTypeId1, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x" });
            OrganisationStatusHandler.InsertRecord(new OrganisationStatusModel { Id = _organisationStatusId3WithProviderTypeId1, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x" });
            OrganisationStatusHandler.InsertRecord(new OrganisationStatusModel { Id = _organisationStatusId4WithProviderTypeId2, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x" });
            ProviderTypeHandler.InsertRecord(new ProviderTypeModel {Id = _providerTypeId1, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x" , ProviderType = "a"});
            ProviderTypeHandler.InsertRecord(new ProviderTypeModel { Id = _providerTypeId2, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", ProviderType = "b"});
            ProviderTypeOrganisationStatusHandler.InsertRecord(new ProviderTypeOrganisationStatusModel{Id=1, OrganisationStatusId = _organisationStatusId1WithProviderTypeId1, ProviderTypeId = _providerTypeId1});
            ProviderTypeOrganisationStatusHandler.InsertRecord(new ProviderTypeOrganisationStatusModel { Id = 2, OrganisationStatusId = _organisationStatusId2WithProviderTypeId1, ProviderTypeId = _providerTypeId1 });
            ProviderTypeOrganisationStatusHandler.InsertRecord(new ProviderTypeOrganisationStatusModel { Id = 3, OrganisationStatusId = _organisationStatusId3WithProviderTypeId1, ProviderTypeId = _providerTypeId1 });
            ProviderTypeOrganisationStatusHandler.InsertRecord(new ProviderTypeOrganisationStatusModel { Id = 4, OrganisationStatusId = _organisationStatusId4WithProviderTypeId2, ProviderTypeId = _providerTypeId2 });
        }

        [TestCase(null,4)]
        [TestCase(1, 3)]
        [TestCase(2, 1)]
        [TestCase(3, 0)]
        public void Get_organisation_statuses_for_provider_type_id_is_returning_correct_counts(int? providerTypeId, int numberOfExpectedResults)
        {
            var result = _lookupRepository.GetOrganisationStatuses(providerTypeId).Result;
            Assert.AreEqual(numberOfExpectedResults, result.Count());
        }

        [OneTimeTearDown]
        public void Tear_down()
        {
            ProviderTypeOrganisationStatusHandler.DeleteAllRecords();
            ProviderTypeHandler.DeleteAllRecords();
            OrganisationStatusHandler.DeleteAllRecords();
        }
    }
}
