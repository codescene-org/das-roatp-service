using System;
using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using SFA.DAS.RoatpService.Data.IntegrationTests.Handlers;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;
using SFA.DAS.RoATPService.Application.Commands;
using SFA.DAS.RoATPService.Data;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Tests
{
    public class OrganisationDataStartDateTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private  OrganisationRepository _repository;
        private OrganisationStatusModel _status1;
        private int _organisationStatusId1;
        private ProviderTypeModel _providerType1;
        private int _providerTypeId1;
        private OrganisationTypeModel _organisationTypeModel1;
        private int _organisationTypeId1;

        [OneTimeSetUp]
        public void SetUpOrganisationTests()
        {
            _organisationStatusId1 = 1;
            _providerTypeId1 = 10;
            _organisationTypeId1 = 100;
            _repository = new OrganisationRepository(_databaseService.WebConfiguration);
            _status1 = new OrganisationStatusModel {Id = _organisationStatusId1, Status = "Live", CreatedAt = DateTime.Now,CreatedBy="TestSystem"};
            OrganisationStatusHandler.InsertRecord(_status1);
            _providerType1 = new ProviderTypeModel {Id = _providerTypeId1, ProviderType = "provider type 10", Description = "provider type description", CreatedAt = DateTime.Now, CreatedBy = "TestSystem", Status="Live"};
            ProviderTypeHandler.InsertRecord(_providerType1);
            _organisationTypeModel1 = new OrganisationTypeModel { Id = _organisationTypeId1, Type = "organisation type 10", Description = "organisation type description", CreatedAt = DateTime.Now, CreatedBy = "TestSystem", Status = "Live" };
            OrganisationTypeHandler.InsertRecord(_organisationTypeModel1);
        }

        [Test]
        public void RunCreateOrganisationAndCheckStartDateIsTodayIsReturned()
        {
            const long organisationUkprn = 12344321;

            var command = new CreateOrganisationCommand
            {
                Ukprn = organisationUkprn,
                OrganisationTypeId = _organisationTypeId1,
                ProviderTypeId = _providerTypeId1,
                OrganisationStatusId =  _organisationStatusId1,
                StatusDate = DateTime.Today.AddDays(5),
                LegalName = "Legal McLegal",
                Username = "Tester McTestface"
       
            };

            var orgPlaceholder = _repository.CreateOrganisation(command).Result;

            var organisationDetails = OrganisationHandler.GetOrganisationFromukprn(organisationUkprn);
            var organisationData = new OrganisationDataHandler().Parse(organisationDetails.OrganisationData);
            Assert.AreEqual(DateTime.Today,organisationData.StartDate);
        }

        [OneTimeTearDown]
        public void TearDownOrganisationTypesTests()
        {
            OrganisationHandler.DeleteAllRecords();
            OrganisationTypeHandler.DeleteRecord(_organisationTypeId1);
            ProviderTypeHandler.DeleteAllRecords();
            OrganisationStatusHandler.DeleteRecords(new List<int> { _status1.Id });
        }
    }
}
