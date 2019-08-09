using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.RoatpService.Data.IntegrationTests.Handlers;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;
using SFA.DAS.RoATPService.Data;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Tests
{
    public class GetEngagementTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private OrganisationRepository _repository;
        private OrganisationModel _organisation1WithOnboarding;
        private OrganisationModel _organisation2WithOnboarding;
        private OrganisationModel _organisation3WithoutOnboarding;
        private Guid _organisationId;
        private List<Engagement> _actualEngagements;
        private int _organisationTypeId;

        [OneTimeSetUp]
        public void Set_up_data()
        {

            _organisationTypeId = 100;
            _repository = new OrganisationRepository(_databaseService.WebConfiguration);

            var organisationData = new OrganisationData();
            _organisation1WithOnboarding = new OrganisationModel
            {
                UKPRN = 11112222,
                OrganisationTypeId = _organisationTypeId,
                ProviderTypeId = ProviderType.MainProvider,
                StatusId = OrganisationStatus.Onboarding,
                StatusDate = DateTime.Today,
                LegalName = "legal name 1",
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                CreatedBy = "Test",
                OrganisationData = JsonConvert.SerializeObject(organisationData)
            };

            _organisation2WithOnboarding = new OrganisationModel
            {
                UKPRN = 11113333,
                OrganisationTypeId = _organisationTypeId,
                ProviderTypeId = ProviderType.MainProvider,
                StatusId = OrganisationStatus.Onboarding,
                StatusDate = DateTime.Today.AddDays(-1),
                LegalName = "legal name 2",
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                CreatedBy = "Test",
                OrganisationData = JsonConvert.SerializeObject(organisationData)
            };

            _organisation3WithoutOnboarding = new OrganisationModel
            {
                UKPRN = 11113333,
                OrganisationTypeId = _organisationTypeId,
                ProviderTypeId = ProviderType.MainProvider,
                StatusId = OrganisationStatus.Active,
                StatusDate = DateTime.Today,
                LegalName = "legal name 3",
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                CreatedBy = "Test",
                OrganisationData = JsonConvert.SerializeObject(organisationData)
            };

            OrganisationTypeHandler.InsertRecord(new OrganisationTypeModel { Id = _organisationTypeId, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", Type = "a" });
            ProviderTypeHandler.InsertRecord(new ProviderTypeModel {Id = ProviderType.MainProvider, ProviderType = "Main Provider", CreatedAt = DateTime.Now, CreatedBy = "system", Status="Live"});

            OrganisationStatusHandler.InsertRecord(new OrganisationStatusModel { Id = OrganisationStatus.Active, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "Active" });
            OrganisationStatusHandler.InsertRecord(new OrganisationStatusModel { Id = OrganisationStatus.Onboarding, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "Onboarding" });
            OrganisationStatusHandler.InsertRecord(new OrganisationStatusModel { Id = OrganisationStatus.ActiveNotTakingOnApprentices, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "active not taking" });
            OrganisationStatusHandler.InsertRecord(new OrganisationStatusModel { Id = OrganisationStatus.Removed, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "Removed" });


            OrganisationHandler.InsertRecord(_organisation1WithOnboarding);
            OrganisationHandler.InsertRecord(_organisation2WithOnboarding);
            OrganisationHandler.InsertRecord(_organisation3WithoutOnboarding);





            //_actualEngagements = _repository.GetEngagements().Result.ToList();
            //_newCharityNumber = _repository.GetCharityNumber(_organisationId).Result;
        }

        [Test]
        public void Number_Of_Returned_Records_As_Expected()
        {
            _actualEngagements = _repository.GetEngagements().Result.ToList();
            Assert.AreEqual(2,_actualEngagements.Count);
        }


        [OneTimeTearDown]
        public void Tear_down()
        {
            OrganisationHandler.DeleteAllRecords();
            OrganisationTypeHandler.DeleteAllRecords();
            OrganisationStatusHandler.DeleteAllRecords();
            ProviderTypeHandler.DeleteAllRecords();
            //ProviderTypeHandler.DeleteAllRecords();
            //OrganisationStatusHandler.DeleteRecords(new List<int> { _status1.Id });
        }
    }
}
