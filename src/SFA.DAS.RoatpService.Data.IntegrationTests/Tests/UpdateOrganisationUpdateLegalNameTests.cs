using System;
using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.RoatpService.Data.IntegrationTests.Handlers;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;
using SFA.DAS.RoATPService.Data;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Tests
{
    public class UpdateOrganisationUpdateLegalNameTests : TestBase
    {

        private readonly DatabaseService _databaseService = new DatabaseService();
        private UpdateOrganisationRepository _repository;
        private OrganisationStatusModel _status1;
        private int _organisationStatusId1;
        private ProviderTypeModel _providerType1;
        private int _providerTypeId1;
        private OrganisationTypeModel _organisationTypeModel1;
        private int _organisationTypeId1;
        private OrganisationModel _organisation;
        private long _organisationUkprn;
        private string _legalName;
        private Guid _organisationId;
        private string _legalNameAfterChange;
        private string _originalLegalName;
        private string _changedBy;
        private bool _successfulUpdate;
        private string _newLegaName;

        [OneTimeSetUp]
        public void Set_up_and_run_update()
        {
            _organisationStatusId1 = 1;
            _providerTypeId1 = 10;
            _organisationTypeId1 = 100;
            _organisationUkprn = 11114433;
            _legalName = "Legal name 1";
            _legalNameAfterChange = "Legal Name Version 2";
            _organisationId = Guid.NewGuid();
            _repository = new UpdateOrganisationRepository(_databaseService.WebConfiguration);
            _status1 = new OrganisationStatusModel { Id = _organisationStatusId1, Status = "Live", CreatedAt = DateTime.Now, CreatedBy = "TestSystem" };
            OrganisationStatusHandler.InsertRecord(_status1);
            _providerType1 = new ProviderTypeModel { Id = _providerTypeId1, ProviderType = "provider type 10", Description = "provider type description", CreatedAt = DateTime.Now, CreatedBy = "TestSystem", Status = "Live" };
            ProviderTypeHandler.InsertRecord(_providerType1);
            _organisationTypeModel1 = new OrganisationTypeModel { Id = _organisationTypeId1, Type = "organisation type 10", Description = "organisation type description", CreatedAt = DateTime.Now, CreatedBy = "TestSystem", Status = "Live" };
            OrganisationTypeHandler.InsertRecord(_organisationTypeModel1);
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
            _originalLegalName = _repository.GetLegalName(_organisationId).Result;
            _changedBy = "SystemChange";

            _successfulUpdate = _repository.UpdateLegalName(_organisationId, _legalNameAfterChange, _changedBy).Result;
            _newLegaName = _repository.GetLegalName(_organisationId).Result;

        }

        [Test]
        public void Original_legal_name_is_correct()
        {
            Assert.AreEqual(_originalLegalName, _legalName);
        }

        [Test]
        public void Update_was_marked_successful()
        {
            Assert.AreEqual(true, _successfulUpdate);
        }

        [Test]
        public void Organisation_has_new_legal_name()
        {
            Assert.AreEqual(_newLegaName, _legalNameAfterChange);
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
