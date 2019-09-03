using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SFA.DAS.RoatpService.Data.IntegrationTests;
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
    private List<Engagement> _actualEngagements;
    private OrganisationStatusEventModel event1Active;
    private OrganisationStatusEventModel event2Onboarding;
    private OrganisationStatusEventModel event3Active;
    private OrganisationStatusEventModel event4Removed;
    private OrganisationStatusEventModel event5ActiveNotTaking;
    private OrganisationStatusEventModel event6Active;
    private DateTime event1CreatedOn;
    private DateTime event2CreatedOn;
    private DateTime event3CreatedOn;
    private DateTime event4CreatedOn;
    private DateTime event5CreatedOn;
    private DateTime event6CreatedOn;


    [OneTimeSetUp]
    public void Set_up_data()
    {
        event1CreatedOn = DateTime.Today;
        event2CreatedOn = DateTime.Today.AddDays(-1);
        event3CreatedOn = DateTime.Today.AddDays(-2);
        event4CreatedOn = DateTime.Today.AddDays(-3);
        event5CreatedOn = DateTime.Today.AddDays(-4);
        event6CreatedOn = DateTime.Today.AddDays(-5);

        _repository = new OrganisationRepository(_databaseService.WebConfiguration);

        OrganisationStatusHandler.InsertRecord(new OrganisationStatusModel { Id = OrganisationStatus.Active, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "Active" });
        OrganisationStatusHandler.InsertRecord(new OrganisationStatusModel { Id = OrganisationStatus.Onboarding, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "Onboarding" });
        OrganisationStatusHandler.InsertRecord(new OrganisationStatusModel { Id = OrganisationStatus.ActiveNotTakingOnApprentices, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "active not taking" });
        OrganisationStatusHandler.InsertRecord(new OrganisationStatusModel { Id = OrganisationStatus.Removed, CreatedAt = DateTime.Now, CreatedBy = "system", Status = "Removed" });

        event1Active = new OrganisationStatusEventModel { Id = 1, CreatedOn = event1CreatedOn, OrganisationStatusId = OrganisationStatus.Active, ProviderId = 11112221 };
        event2Onboarding = new OrganisationStatusEventModel { Id = 2, CreatedOn = event2CreatedOn, OrganisationStatusId = OrganisationStatus.Onboarding, ProviderId = 11112222 };
        event3Active = new OrganisationStatusEventModel { Id = 3, CreatedOn = event3CreatedOn, OrganisationStatusId = OrganisationStatus.Active, ProviderId = 11112223 };
        event4Removed = new OrganisationStatusEventModel { Id = 4, CreatedOn = event4CreatedOn, OrganisationStatusId = OrganisationStatus.Removed, ProviderId = 11112224 };
        event5ActiveNotTaking = new OrganisationStatusEventModel { Id = 5, CreatedOn = event5CreatedOn, OrganisationStatusId = OrganisationStatus.ActiveNotTakingOnApprentices, ProviderId = 11112225 };
        event6Active = new OrganisationStatusEventModel { Id = 6, CreatedOn = event6CreatedOn, OrganisationStatusId = OrganisationStatus.Active, ProviderId = 11112226 };


        OrganisationStatusEventHandler.InsertRecord(event1Active);
        OrganisationStatusEventHandler.InsertRecord(event2Onboarding);
        OrganisationStatusEventHandler.InsertRecord(event3Active);
        OrganisationStatusEventHandler.InsertRecord(event4Removed);
        OrganisationStatusEventHandler.InsertRecord(event5ActiveNotTaking);
        OrganisationStatusEventHandler.InsertRecord(event6Active);
    }



    [Test]
    public void Total_Number_Of_Returned_Records_As_Expected()
    {
        _actualEngagements = _repository.GetEngagements().Result.ToList();
        Assert.AreEqual(6, _actualEngagements.Count);
    }


    [Test]
    public void Number_Of_Active_Returned_Records_As_Expected()
    {
        _actualEngagements = _repository.GetEngagements().Result.ToList();
        Assert.AreEqual(3, _actualEngagements.Where(x => x.Event == MapEventDescriptionToStatus(OrganisationStatus.Active)).ToList().Count);
    }

    [Test]
    public void Number_Of_Active_Not_Starting_Returned_Records_As_Expected()
    {
        _actualEngagements = _repository.GetEngagements().Result.ToList();
        Assert.AreEqual(1, _actualEngagements.Where(x => x.Event == MapEventDescriptionToStatus(OrganisationStatus.ActiveNotTakingOnApprentices)).ToList().Count);
    }

    [Test]
    public void Number_Of_Removed_Returned_Records_As_Expected()
    {
        _actualEngagements = _repository.GetEngagements().Result.ToList();
        Assert.AreEqual(1, _actualEngagements.Where(x => x.Event == MapEventDescriptionToStatus(OrganisationStatus.Removed)).ToList().Count);
    }

    [Test]
    public void Number_OfOnboarding_Returned_Records_As_Expected()
    {
        _actualEngagements = _repository.GetEngagements().Result.ToList();
        Assert.AreEqual(1, _actualEngagements.Where(x => x.Event == MapEventDescriptionToStatus(OrganisationStatus.Onboarding)).ToList().Count);
    }

    [OneTimeTearDown]
    public void Tear_down()
    {
        OrganisationStatusEventHandler.DeleteAllRecords();
        OrganisationStatusHandler.DeleteAllRecords();
    }


    private string MapEventDescriptionToStatus(int status)
    {
        if (status == OrganisationStatus.Active)
            return "ACTIVE";
        if (status == OrganisationStatus.Removed)
            return "REMOVED";
        if (status == OrganisationStatus.ActiveNotTakingOnApprentices)
            return "ACTIVENOSTARTS";
        if (status == OrganisationStatus.Onboarding)
            return "INITIATED";

        return "UNKNOWN";
    }
}
}
