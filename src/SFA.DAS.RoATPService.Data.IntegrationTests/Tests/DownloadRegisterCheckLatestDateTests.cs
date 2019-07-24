using System;
using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.RoatpService.Data.IntegrationTests.Handlers;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;
using SFA.DAS.RoATPService.Data;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Tests
{
    public class DownloadRegisterCheckLatestDateTests : TestBase
    {

        private readonly DatabaseService _databaseService = new DatabaseService();
        private DownloadRegisterRepository _repository;
        private ProviderTypeModel _providerType1;
        private int _providerTypeId1;
        private OrganisationTypeModel _organisationTypeModel1;
        private int _organisationTypeId1;
        private OrganisationModel _organisation;
        private long _organisationUkprn;
        private string _legalName;
        private Guid _organisationId;
        private int _organisationUkprn2;
        private Guid _organisationId2;

        [OneTimeSetUp]
        public void setup_organisations_are_added()
        {

            _providerTypeId1 = 10;
            _organisationTypeId1 = 100;
            _organisationUkprn = 11114433;
            _organisationUkprn2 = 11114432;
            _legalName = "Legal name 1";
            _organisationId = Guid.NewGuid();
            _organisationId2 = Guid.NewGuid();
            _repository = new DownloadRegisterRepository(_databaseService.WebConfiguration);
            OrganisationStatusHandler.InsertRecords(
                new List<OrganisationStatusModel>
                {
                    new OrganisationStatusModel { Id = OrganisationStatusHandler.Active, Status = "Active", CreatedAt = DateTime.Now, CreatedBy = "TestSystem" },
                    new OrganisationStatusModel { Id = OrganisationStatusHandler.Removed, Status = "Removed", CreatedAt = DateTime.Now, CreatedBy = "TestSystem" },
                    new OrganisationStatusModel { Id = OrganisationStatusHandler.ActiveNotTakingOnApprentices, Status = "Active - but not taking on apprentices", CreatedAt = DateTime.Now, CreatedBy = "TestSystem" },
                    new OrganisationStatusModel { Id = OrganisationStatusHandler.Onboarding, Status = "On-boarding", CreatedAt = DateTime.Now, CreatedBy = "TestSystem" }

                });
            _providerType1 = new ProviderTypeModel { Id = _providerTypeId1, ProviderType = "provider type 10", Description = "provider type description", CreatedAt = DateTime.Now, CreatedBy = "TestSystem", Status = "Live" };
            ProviderTypeHandler.InsertRecord(_providerType1);
            _organisationTypeModel1 = new OrganisationTypeModel { Id = _organisationTypeId1, Type = "organisation type 10", Description = "organisation type description", CreatedAt = DateTime.Now, CreatedBy = "TestSystem", Status = "Live" };
            OrganisationTypeHandler.InsertRecord(_organisationTypeModel1);
            
        }

        [TestCase("2019-06-11 11:57:00", null, "2019-06-09 11:57:00", null, "2019-06-11 11:57:00", 1)]
        [TestCase("2019-06-11 11:57:00", null, "2019-06-09 11:57:00", null, "2019-06-11 11:57:00", 0)]
        [TestCase("2019-06-11 11:57:00", null, "2019-06-09 11:57:00", null, "2019-06-11 11:57:00", 2)]
        [TestCase("2019-06-11 11:57:00", "2019-06-11 11:59:00", "2019-06-10 11:57:00", "2019-06-10 11:59:00", "2019-06-11 11:59:00", 1)]
        [TestCase("2019-06-11 11:57:00", "2019-06-11 11:59:00", "2019-06-10 11:57:00", "2019-06-10 11:59:00", "2019-06-11 11:59:00", 0)]
        [TestCase("2019-06-11 11:57:00", "2019-06-11 11:59:00", "2019-06-10 11:57:00", "2019-06-10 11:59:00", "2019-06-11 11:59:00", 2)]
        [TestCase("2019-06-11 11:57:00", "2019-06-11 11:59:00", "2019-06-10 11:57:00", "2019-06-10 11:59:00", "2019-06-10 11:59:00", 3)]
        [TestCase("2019-06-11 11:57:00", null, "2019-06-10 11:57:00", null, "2019-06-10 11:57:00", 3)]
        public void ExpectedLatestDateIsReturnedWithUpdatedTakingPrecedence(DateTime createdDate,  DateTime? updatedDate, 
                                                                            DateTime previousCreatedDate, DateTime? previousUpdatedDate, 
                                                                            DateTime expectedLatestDate, int organisationStatusId)
        {

            var expectedLatest = updatedDate ?? createdDate;

            if (organisationStatusId == 3)
            {
                expectedLatest = previousUpdatedDate ?? previousCreatedDate;
            }

            _organisation = new OrganisationModel
            {
                UKPRN = _organisationUkprn,
                OrganisationTypeId = _organisationTypeId1,
                ProviderTypeId = _providerTypeId1,
                StatusId = organisationStatusId,
                StatusDate = DateTime.Today.AddDays(5),
                LegalName = _legalName,
                Id = _organisationId,
                CreatedAt = createdDate,
                UpdatedAt = updatedDate,
                CreatedBy = "Test"
            };


            var _organisationOld = new OrganisationModel
            {
                UKPRN = _organisationUkprn2,
                OrganisationTypeId = _organisationTypeId1,
                ProviderTypeId = _providerTypeId1,
                StatusId = OrganisationStatusHandler.Active,
                StatusDate = DateTime.Today.AddDays(5),
                LegalName = _legalName,
                Id = _organisationId2,
                CreatedAt = previousCreatedDate,
                UpdatedAt = previousUpdatedDate,
                CreatedBy = "Test"
            };

            OrganisationHandler.InsertRecord(_organisation);
            OrganisationHandler.InsertRecord(_organisationOld);
            var latestDate = _repository.GetLatestNonOnboardingOrganisationChangeDate().Result;
            Assert.AreEqual(expectedLatest.ToString("F"), latestDate.Value.ToString("F"));
            OrganisationHandler.DeleteAllRecords();
        }

        [OneTimeTearDown]
        public void tear_down()
        {
            OrganisationHandler.DeleteAllRecords();
            OrganisationTypeHandler.DeleteRecord(_organisationTypeId1);
            ProviderTypeHandler.DeleteAllRecords();
            OrganisationStatusHandler.DeleteAllRecords();
        }
    }
}

