using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using SFA.DAS.RoatpService.Data.IntegrationTests.Handlers;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;
using SFA.DAS.RoATPService.Data;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Tests
{
    public class CheckDuplicateUkprnDetailsTests:TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private DuplicateCheckRepository _repository;
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
        private ILogger<DuplicateCheckRepository> _logger;

        [OneTimeSetUp]
        public void setup_organisation_is_added()
        {
            _organisationStatusId1 = 1;
            _providerTypeId1 = 10;
            _organisationTypeId1 = 100;
            _organisationUkprn = 11114433;
            _legalName = "Legal name 1";
            _organisationId = Guid.NewGuid();
            _repository = new DuplicateCheckRepository(_databaseService.WebConfiguration, _logger);
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
        }

        [Test]
        public void Duplicate_message_is_returned_for_different_organisation_id()
        {
            var duplicateCheckResponse = _repository.DuplicateUKPRNExists(Guid.NewGuid(),_organisationUkprn).Result;
            Assert.AreEqual(true, duplicateCheckResponse.DuplicateFound);
        }

        [Test]
        public void Duplicate_message_is_returned_for_same_organisation_id()
        {
            var duplicateCheckResponse = _repository.DuplicateUKPRNExists(_organisationId, _organisationUkprn).Result;
            Assert.AreEqual(false, duplicateCheckResponse.DuplicateFound);
        }

        [OneTimeTearDown]
        public void tear_down()
        {
            OrganisationHandler.DeleteAllRecords();
            OrganisationTypeHandler.DeleteRecord(_organisationTypeId1);
            ProviderTypeHandler.DeleteAllRecords();
            OrganisationStatusHandler.DeleteRecords(new List<int> { _status1.Id });
        }
    }
}
