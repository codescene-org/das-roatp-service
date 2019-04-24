using System;
using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.RoatpService.Data.IntegrationTests.Handlers;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;
using SFA.DAS.RoATPService.Data;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Tests
{
    public class UpdateOrganisationUpdateProviderTypeTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private UpdateOrganisationRepository _repository;
        private OrganisationStatusModel _status1;
        private OrganisationStatusModel _status2;
        private int _organisationStatusId1;
        private int _newOrganisationStatusId;
        private ProviderTypeModel _providerType1;
        private ProviderTypeModel _providerType2;
        private int _providerTypeId1;
        private int _providerTypeId2;
        private OrganisationTypeModel _organisationTypeModel1;
        private OrganisationTypeModel _organisationTypeModel2;
        private int _organisationTypeId1;
        private int _organisationTypeId2;
        private OrganisationModel _organisation;
        private long _organisationUkprn;
        private string _legalName;
        private Guid _organisationId;
        private string _changedBy;
        private bool _successfulUpdate;
        private int _newProviderType;
        private int _newOrganisationType;

        [OneTimeSetUp]
        public void Set_up_and_run_update()
        {
            _organisationStatusId1 = 1;
            _providerTypeId1 = 10;
            _providerTypeId2 = 20;
            _organisationTypeId1 = 100;
            _organisationTypeId2 = 111;
            _organisationUkprn = 11114433;
            _legalName = "Legal name 1";
            _organisationId = Guid.NewGuid();
            _repository = new UpdateOrganisationRepository(_databaseService.WebConfiguration);
            _status1 = new OrganisationStatusModel { Id = _organisationStatusId1, Status = "Live", CreatedAt = DateTime.Now, CreatedBy = "TestSystem" };
            OrganisationStatusHandler.InsertRecord(_status1);
            _providerType1 = new ProviderTypeModel { Id = _providerTypeId1, ProviderType = "provider type 10", Description = "provider type description", CreatedAt = DateTime.Now, CreatedBy = "TestSystem", Status = "Live" };
            ProviderTypeHandler.InsertRecord(_providerType1);
            _providerType2 = new ProviderTypeModel { Id = _providerTypeId2, ProviderType = "provider type 12", Description = "provider type description", CreatedAt = DateTime.Now, CreatedBy = "TestSystem", Status = "Live" };
            ProviderTypeHandler.InsertRecord(_providerType2);
            _organisationTypeModel1 = new OrganisationTypeModel { Id = _organisationTypeId1, Type = "organisation type 10", Description = "organisation type description", CreatedAt = DateTime.Now, CreatedBy = "TestSystem", Status = "Live" };
            OrganisationTypeHandler.InsertRecord(_organisationTypeModel1);
            _organisationTypeModel2 = new OrganisationTypeModel { Id = _organisationTypeId2, Type = "organisation type 22", Description = "organisation type description", CreatedAt = DateTime.Now, CreatedBy = "TestSystem", Status = "Live" };
            OrganisationTypeHandler.InsertRecord(_organisationTypeModel2);
            _organisation = new OrganisationModel
            {
                UKPRN = _organisationUkprn,
                OrganisationTypeId = _organisationTypeId1,
                ProviderTypeId = _providerTypeId1,
                StatusId = _organisationStatusId1,
                StatusDate = DateTime.Today.AddDays(5),
                LegalName = _legalName,
                Id = _organisationId,
                CreatedAt = DateTime.Now,
                CreatedBy = "Test"
            };
            OrganisationHandler.InsertRecord(_organisation);
            _changedBy = "SystemChange";

            _successfulUpdate = _repository.UpdateProviderTypeAndOrganisationType(_organisationId, _providerTypeId2, _organisationTypeId2, _changedBy).Result;
            _newProviderType = _repository.GetProviderType(_organisationId).Result;
            _newOrganisationType = _repository.GetOrganisationType(_organisationId).Result;
        }

        [Test]
        public void Update_was_marked_successful()
        {
            Assert.AreEqual(true, _successfulUpdate);
        }

        [Test]
        public void Organisation_has_new_provider_type()
        {
            Assert.AreEqual(_providerTypeId2, _newProviderType);
        }

        [Test]
        public void Organisation_has_new_organisation_type()
        {
            Assert.AreEqual(_organisationTypeId2, _newOrganisationType);
        }

        [Test]
        public void Updated_by_is_correct()
        {
            var changedOrganisation = OrganisationHandler.GetOrganisationFromId(_organisationId);
            Assert.AreEqual(_changedBy, changedOrganisation.UpdatedBy);
        }

        [OneTimeTearDown]
        public void Tear_down()
        {
            OrganisationHandler.DeleteAllRecords();
            OrganisationTypeHandler.DeleteRecord(_organisationTypeId1);
            ProviderTypeHandler.DeleteAllRecords();
            OrganisationStatusHandler.DeleteRecords(new List<int> { _status1.Id });
        }
    }
}
