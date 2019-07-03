using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.RoatpService.Data.IntegrationTests.Handlers;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;
using SFA.DAS.RoATPService.Data;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Tests
{
    public class UpdateOrganisationUpdateApplicationDeterminedDateTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private UpdateOrganisationRepository _updateOrganisationRepository;
        private OrganisationRepository _repository;
        private OrganisationStatusModel _status;
        private int _organisationStatusId;
        private ProviderTypeModel _providerType;
        private int _providerTypeId;
        private OrganisationTypeModel _organisationTypeModel;
        private int _organisationTypeId;
        private OrganisationModel _organisation;
        private long _organisationUkprn;
        private Guid _organisationId;
        private string _changedBy;
        private bool _successfulUpdate;
        private DateTime? _newApplicationDeterminedDate;
        private DateTime? _applicationDeterminedate;
        private DateTime? _originalDeterminedDate;

        [OneTimeSetUp]
        public void Set_up_and_run_update()
        {
            _organisationStatusId = 1;
            _providerTypeId = 10;
            _organisationTypeId = 100;
            _applicationDeterminedate = DateTime.Today;
            _newApplicationDeterminedDate = DateTime.Today.AddDays(-1);
            _organisationUkprn = 11114433;
            _organisationId = Guid.NewGuid();
            _updateOrganisationRepository = new UpdateOrganisationRepository(_databaseService.WebConfiguration);
            _repository = new OrganisationRepository(_databaseService.WebConfiguration);
            _status = new OrganisationStatusModel { Id = _organisationStatusId, Status = "Live", CreatedAt = DateTime.Now, CreatedBy = "TestSystem" };
            OrganisationStatusHandler.InsertRecord(_status);
            _providerType = new ProviderTypeModel { Id = _providerTypeId, ProviderType = "provider type 10", Description = "provider type description", CreatedAt = DateTime.Now, CreatedBy = "TestSystem", Status = "Live" };
            ProviderTypeHandler.InsertRecord(_providerType);
            _organisationTypeModel = new OrganisationTypeModel { Id = _organisationTypeId, Type = "organisation type 10", Description = "organisation type description", CreatedAt = DateTime.Now, CreatedBy = "TestSystem", Status = "Live" };
            OrganisationTypeHandler.InsertRecord(_organisationTypeModel);
            var organisationData = new OrganisationData { ApplicationDeterminedDate = _applicationDeterminedate };
            _organisation = new OrganisationModel
            {
                UKPRN = _organisationUkprn,
                OrganisationTypeId = _organisationTypeId,
                ProviderTypeId = _providerTypeId,
                StatusId = _organisationStatusId,
                StatusDate = DateTime.Today.AddDays(5),
                LegalName = "legal name",
                Id = _organisationId,
                CreatedAt = DateTime.Now,
                CreatedBy = "Test",
                OrganisationData = JsonConvert.SerializeObject(organisationData)
            };
            OrganisationHandler.InsertRecord(_organisation);
            _originalDeterminedDate = _repository.GetApplicationDeterminedDate(_organisationId).Result;
            _changedBy = "SystemChange";

            _successfulUpdate = _updateOrganisationRepository.UpdateApplicationDeterminedDate(_organisationId, _newApplicationDeterminedDate.Value, _changedBy).Result;
            _newApplicationDeterminedDate = _repository.GetApplicationDeterminedDate(_organisationId).Result;
        }

        [Test]
        public void Original_company_number_is_correct()
        {
            Assert.AreEqual(_originalDeterminedDate, _applicationDeterminedate);
        }

        [Test]
        public void Update_was_marked_successful()
        {
            Assert.AreEqual(true, _successfulUpdate);
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
            OrganisationTypeHandler.DeleteRecord(_organisationTypeId);
            ProviderTypeHandler.DeleteAllRecords();
            OrganisationStatusHandler.DeleteRecords(new List<int> { _status.Id });
        }
    }
}
