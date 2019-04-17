using System;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.RoatpService.Data.IntegrationTests.Handlers;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;
using SFA.DAS.RoATPService.Data;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Tests
{
    public class LookupDataIsOrganisationStatusValidForOrganisationTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private LookupDataRepository _lookupRepository;

        private int _providerTypeId2;
        private int _providerTypeId1;
        private int _organisationTypeId1;
        private Guid _organisationIdMatched;
        private Guid _organisationIdUnmatchedByProviderType;
        private OrganisationModel _organisationWithMatchedRecords;
        private int _organisationStatusId1;
        private OrganisationModel _organisationWithoutMatchedProviderType;
        private int _providerTypeId3;
        private int _organisationStatusId2;


        [OneTimeSetUp]
        public void Before_the_tests()
        {
            _lookupRepository = new LookupDataRepository(null, _databaseService.WebConfiguration);
            _providerTypeId1 = 1;
            _providerTypeId2 = 2;
            _providerTypeId3 = 3;
            _organisationTypeId1 = 10;
            _organisationIdMatched = Guid.NewGuid();
            _organisationIdUnmatchedByProviderType = Guid.NewGuid();
            _organisationStatusId1 = 1;
            _organisationStatusId2 = 2;

            OrganisationTypeHandler.InsertRecord(new OrganisationTypeModel { Id = _organisationTypeId1, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", Type = "a" });
            ProviderTypeHandler.InsertRecord(new ProviderTypeModel { Id = _providerTypeId1, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", ProviderType = "a" });
            ProviderTypeHandler.InsertRecord(new ProviderTypeModel { Id = _providerTypeId2, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", ProviderType = "b" });
            ProviderTypeHandler.InsertRecord(new ProviderTypeModel { Id = _providerTypeId3, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", ProviderType = "c" });
            OrganisationStatusHandler.InsertRecord(new OrganisationStatusModel { Id = _organisationStatusId1, Status = "Live", CreatedAt = DateTime.Now, CreatedBy = "TestSystem" });
            OrganisationStatusHandler.InsertRecord(new OrganisationStatusModel { Id = _organisationStatusId2, Status = "Live", CreatedAt = DateTime.Now, CreatedBy = "TestSystem" });
            ProviderTypeOrganisationStatusHandler.InsertRecord(new ProviderTypeOrganisationStatusModel{Id=_organisationTypeId1,OrganisationStatusId = _organisationStatusId1,ProviderTypeId = _providerTypeId1});
            var organisationData = new OrganisationData();

            _organisationWithMatchedRecords = new OrganisationModel
            {
                UKPRN = 11111111,
                OrganisationTypeId = _organisationTypeId1,
                ProviderTypeId = _providerTypeId1,
                StatusId = _organisationStatusId1,
                StatusDate = DateTime.Today.AddDays(5),
                LegalName = "legal name 1",
                Id = _organisationIdMatched,
                CreatedAt = DateTime.Now,
                CreatedBy = "Test",
                OrganisationData = JsonConvert.SerializeObject(organisationData)
            };

            _organisationWithoutMatchedProviderType = new OrganisationModel
            {
                UKPRN = 11111112,
                OrganisationTypeId = _organisationTypeId1,
                ProviderTypeId = _providerTypeId3,
                StatusId = _organisationStatusId1,
                StatusDate = DateTime.Today.AddDays(5),
                LegalName = "legal name 1",
                Id = _organisationIdUnmatchedByProviderType,
                CreatedAt = DateTime.Now,
                CreatedBy = "Test",
                OrganisationData = JsonConvert.SerializeObject(organisationData)
            }; ;

            OrganisationHandler.InsertRecord(_organisationWithMatchedRecords);
            OrganisationHandler.InsertRecord(_organisationWithoutMatchedProviderType);
        }

        [Test]
        public void Check_organisation_status_for_provider_type_id_is_returning_true_for_matched_record()
        {
            var result =
                _lookupRepository.IsOrganisationStatusValidForOrganisation(_organisationStatusId1,
                    _organisationIdMatched).Result;
            Assert.AreEqual(true, result);
        }

        [Test]
        public void Check_organisation_status_for_provider_type_id_is_returning_false_for_unmatched_provider_type()
        {
            var result =
                _lookupRepository.IsOrganisationStatusValidForOrganisation(_organisationStatusId2,
                    _organisationIdUnmatchedByProviderType).Result;
            Assert.AreEqual(false, result);
        }


        [OneTimeTearDown]
        public void Tear_down()
        {
            OrganisationHandler.DeleteAllRecords();
            OrganisationStatusHandler.DeleteAllRecords();
            ProviderTypeOrganisationTypeHandler.DeleteAllRecords();
            ProviderTypeHandler.DeleteAllRecords();
            OrganisationTypeHandler.DeleteAllRecords();
        }
    }
}

