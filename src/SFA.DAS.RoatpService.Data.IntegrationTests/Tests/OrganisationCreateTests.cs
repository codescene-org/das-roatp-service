using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SFA.DAS.RoatpService.Data.IntegrationTests.Handlers;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;
using SFA.DAS.RoATPService.Data;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Tests
{
    public class OrganisationCreateTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private  OrganisationRepository _repository;
        private OrganisationStatusModel _status1;


        [OneTimeSetUp]
        public void SetUpOrganisationTests()
        {
            _repository = new OrganisationRepository(_databaseService.WebConfiguration);
            _status1 = new OrganisationStatusModel {Id = 1, Status = "statues goes here", CreatedAt = DateTime.Now,CreatedBy="TestSystem"};
            OrganisationStatusHandler.InsertRecord(_status1);


        }

        [Test]
        public void RunGetAllOrganisationTypesAndCheckAllOrganisationsExpectedAreReturned()
        {
            //var res = OrganisationStatusHandler.GetOrganisationStatusFromId(1);
            var z = 2;
            Assert.AreEqual(1,z);
        }


        [OneTimeTearDown]
        public void TearDownOrganisationTypesTests()
        {
            OrganisationStatusHandler.DeleteRecords(new List<int> { _status1.Id });
        }

    }
}
