using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.RoatpService.Data.IntegrationTests.Handlers;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;
using SFA.DAS.RoATPService.Data;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Tests
{
    //MFCMFC
    //public class LookupDataIsOrganisationTypeValidForOrganisationTests : TestBase
    //{
    //    private readonly DatabaseService _databaseService = new DatabaseService();
    //    private LookupDataRepository _lookupRepository;

    //    private int _providerTypeId2;
    //    private int _providerTypeId1;
    //    private int _organisationTypeId1WithProviderTypeId1;
    //    private int _organisationTypeId4WithProviderTypeId2;
    //    private int _organisationTypeId2WithProviderTypeId1;
    //    private int _organisationTypeId3WithProviderTypeId1;
    //    private Guid _organisationIdMatched;
    //    private Guid _organisationIdUnmatchedByProviderType;
    //    private OrganisationModel _organisationWithMatchedRecords;
    //    private int _organisationStatusId1;
    //    private OrganisationModel _organisationWithoutMatchedProviderType;
    //    private int _providerTypeId3;


    //    [OneTimeSetUp]
    //    public void Before_the_tests()
    //    {
    //        _lookupRepository = new LookupDataRepository(null, _databaseService.WebConfiguration);
    //        _providerTypeId1 = 1;
    //        _providerTypeId2 = 2;
    //        _providerTypeId3 = 3;
    //        _organisationTypeId1WithProviderTypeId1 = 10;
    //        _organisationTypeId2WithProviderTypeId1 = 20;
    //        _organisationTypeId3WithProviderTypeId1 = 30;
    //        _organisationTypeId4WithProviderTypeId2 = 100;
    //        _organisationIdMatched = Guid.NewGuid();
    //        _organisationIdUnmatchedByProviderType = Guid.NewGuid();
    //        _organisationStatusId1 = 1;

    //        OrganisationTypeHandler.InsertRecord(new OrganisationTypeModel { Id = _organisationTypeId1WithProviderTypeId1, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", Type = "a" });
    //        OrganisationTypeHandler.InsertRecord(new OrganisationTypeModel { Id = _organisationTypeId2WithProviderTypeId1, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", Type = "b" });
    //        OrganisationTypeHandler.InsertRecord(new OrganisationTypeModel { Id = _organisationTypeId3WithProviderTypeId1, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", Type = "c" });
    //        OrganisationTypeHandler.InsertRecord(new OrganisationTypeModel { Id = _organisationTypeId4WithProviderTypeId2, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", Type = "d" });
    //        ProviderTypeHandler.InsertRecord(new ProviderTypeModel { Id = _providerTypeId1, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", ProviderType = "a" });
    //        ProviderTypeHandler.InsertRecord(new ProviderTypeModel { Id = _providerTypeId2, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", ProviderType = "b" });
    //        ProviderTypeHandler.InsertRecord(new ProviderTypeModel { Id = _providerTypeId3, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "x", ProviderType = "c" });
    //        ProviderTypeOrganisationTypeHandler.InsertRecord(new ProviderTypeOrganisationTypeModel { Id = 1, OrganisationTypeId = _organisationTypeId1WithProviderTypeId1, ProviderTypeId = _providerTypeId1 });
    //        ProviderTypeOrganisationTypeHandler.InsertRecord(new ProviderTypeOrganisationTypeModel { Id = 2, OrganisationTypeId = _organisationTypeId2WithProviderTypeId1, ProviderTypeId = _providerTypeId1 });
    //        ProviderTypeOrganisationTypeHandler.InsertRecord(new ProviderTypeOrganisationTypeModel { Id = 3, OrganisationTypeId = _organisationTypeId3WithProviderTypeId1, ProviderTypeId = _providerTypeId1 });
    //        ProviderTypeOrganisationTypeHandler.InsertRecord(new ProviderTypeOrganisationTypeModel { Id = 4, OrganisationTypeId = _organisationTypeId4WithProviderTypeId2, ProviderTypeId = _providerTypeId2 });
    //        OrganisationStatusHandler.InsertRecord(new OrganisationStatusModel { Id = _organisationStatusId1, Status = "Live", CreatedAt = DateTime.Now, CreatedBy = "TestSystem" });
    //        var organisationData = new OrganisationData();

    //        _organisationWithMatchedRecords = new OrganisationModel
    //        {
    //            UKPRN = 11111111,
    //            OrganisationTypeId = _organisationTypeId1WithProviderTypeId1,
    //            ProviderTypeId = _providerTypeId1,
    //            StatusId = _organisationStatusId1,
    //            StatusDate = DateTime.Today.AddDays(5),
    //            LegalName = "legal name 1",
    //            Id = _organisationIdMatched,
    //            CreatedAt = DateTime.Now,
    //            CreatedBy = "Test",
    //            OrganisationData = JsonConvert.SerializeObject(organisationData)
    //        };

    //        _organisationWithoutMatchedProviderType = new OrganisationModel
    //        {
    //            UKPRN = 11111112,
    //            OrganisationTypeId = _organisationTypeId1WithProviderTypeId1,
    //            ProviderTypeId = _providerTypeId3,
    //            StatusId = _organisationStatusId1,
    //            StatusDate = DateTime.Today.AddDays(5),
    //            LegalName = "legal name 1",
    //            Id = _organisationIdUnmatchedByProviderType,
    //            CreatedAt = DateTime.Now,
    //            CreatedBy = "Test",
    //            OrganisationData = JsonConvert.SerializeObject(organisationData)
    //        }; ;
          
    //        OrganisationHandler.InsertRecord(_organisationWithMatchedRecords);
    //        OrganisationHandler.InsertRecord(_organisationWithoutMatchedProviderType);
    //    }

    //    [Test]
    //    public void Check_organisation_type_for_provider_type_id_is_returning_true_for_matched_record()
    //    {
    //        var result =
    //            _lookupRepository.IsOrganisationTypeValidForOrganisation(_organisationTypeId1WithProviderTypeId1,
    //                _organisationIdMatched).Result;
    //        Assert.AreEqual(true, result);
    //    }

    //    [Test]
    //    public void Check_organisation_type_for_provider_type_id_is_returning_false_for_unmatched_provider_type()
    //    {
    //        var result =
    //            _lookupRepository.IsOrganisationTypeValidForOrganisation(_organisationTypeId1WithProviderTypeId1,
    //                _organisationIdUnmatchedByProviderType).Result;
    //        Assert.AreEqual(false, result);
    //    }


    //    [OneTimeTearDown]
    //    public void Tear_down()
    //    {
    //        OrganisationHandler.DeleteAllRecords();
    //        OrganisationStatusHandler.DeleteAllRecords();
    //        ProviderTypeOrganisationTypeHandler.DeleteAllRecords();
    //        ProviderTypeHandler.DeleteAllRecords();
    //        OrganisationTypeHandler.DeleteAllRecords();
    //    }
    //}
}

