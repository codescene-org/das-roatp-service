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
    public class UpdateOrganisationUpdateCompanyNumberTests : TestBase
    {

        private readonly DatabaseService _databaseService = new DatabaseService();
        private UpdateOrganisationRepository _updateOrganisationRepository;
        private OrganisationRepository _repository;
        private OrganisationStatusModel _status1;
        private int _organisationStatusId1;
        private ProviderTypeModel _providerType1;
        private int _providerTypeId1;
        private OrganisationTypeModel _organisationTypeModel1;
        private int _organisationTypeId1;
        private OrganisationModel _organisation;
        private long _organisationUkprn;
        private Guid _organisationId;
        private bool _originalParentCompanyGuarantee;
        private string _changedBy;
        private bool _successfulUpdate;
        private string _newCompanyNumber;
        private string _companyNumber;
        private bool _parentCompanyGuarantee;
        private bool _newParentCompanyGuarantee;
        private bool _parentCompanyGuaranteeAfterChange;
        private string _originalCompanyNumber;

        [OneTimeSetUp]
        public void Set_up_and_run_update()
        {
            _organisationStatusId1 = 1;
            _providerTypeId1 = 10;
            _organisationTypeId1 = 100;
            _companyNumber = "44443333";
            _organisationUkprn = 11114433;
            _organisationId = Guid.NewGuid();
            _parentCompanyGuarantee = true;
            _parentCompanyGuaranteeAfterChange = false;
            _updateOrganisationRepository = new UpdateOrganisationRepository(_databaseService.WebConfiguration);
            _repository = new OrganisationRepository(_databaseService.WebConfiguration);
            _status1 = new OrganisationStatusModel { Id = _organisationStatusId1, Status = "Live", CreatedAt = DateTime.Now, CreatedBy = "TestSystem" };
            OrganisationStatusHandler.InsertRecord(_status1);
            _providerType1 = new ProviderTypeModel { Id = _providerTypeId1, ProviderType = "provider type 10", Description = "provider type description", CreatedAt = DateTime.Now, CreatedBy = "TestSystem", Status = "Live" };
            ProviderTypeHandler.InsertRecord(_providerType1);
            _organisationTypeModel1 = new OrganisationTypeModel { Id = _organisationTypeId1, Type = "organisation type 10", Description = "organisation type description", CreatedAt = DateTime.Now, CreatedBy = "TestSystem", Status = "Live" };
            OrganisationTypeHandler.InsertRecord(_organisationTypeModel1);
            var organisationData = new OrganisationData { ParentCompanyGuarantee = _parentCompanyGuarantee, CompanyNumber = _companyNumber};
            _organisation = new OrganisationModel
            {
                UKPRN = _organisationUkprn,
                OrganisationTypeId = _organisationTypeId1,
                ProviderTypeId = _providerTypeId1,
                StatusId = _organisationStatusId1,
                StatusDate = DateTime.Today.AddDays(5),
                LegalName = "legal name",
                Id = _organisationId,
                CreatedAt = DateTime.Now,
                CreatedBy = "Test",
                OrganisationData = JsonConvert.SerializeObject(organisationData)
            };
            OrganisationHandler.InsertRecord(_organisation);
            _originalCompanyNumber = _repository.GetCompanyNumber(_organisationId).Result;
            _changedBy = "SystemChange";

            _successfulUpdate = _updateOrganisationRepository.UpdateCompanyNumber(_organisationId, _newCompanyNumber, _changedBy).Result;
            _newCompanyNumber = _repository.GetCompanyNumber(_organisationId).Result;
        }

        [Test]
        public void Original_company_number_is_correct()
        {
            Assert.AreEqual(_originalCompanyNumber, _companyNumber);
        }

        [Test]
        public void Update_was_marked_successful()
        {
            Assert.AreEqual(true, _successfulUpdate);
        }

        [Test]
        public void Organisation_has_new_parent_company_guarantee()
        {
            Assert.AreEqual(_newCompanyNumber, _newCompanyNumber);
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
