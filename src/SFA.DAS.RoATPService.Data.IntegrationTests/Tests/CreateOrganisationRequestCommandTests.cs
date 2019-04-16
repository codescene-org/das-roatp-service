using System;
using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.RoatpService.Data.DapperTypeHandlers;
using SFA.DAS.RoatpService.Data.IntegrationTests.Handlers;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;
using SFA.DAS.RoATPService.Api.Types.Models;
using SFA.DAS.RoATPService.Application.Commands;
using SFA.DAS.RoATPService.Application.Mappers;
using SFA.DAS.RoATPService.Data;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Tests
{
    public class CreateOrganisationRequestToCommandTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private  OrganisationRepository _repository;
        private OrganisationStatusModel _statusActive;
        private OrganisationStatusModel _statusOnboarding;
        private int _organisationStatusIdActive = OrganisationStatus.Active;
        private int _organisationStatusIdOnboarding = OrganisationStatus.Onboarding;


        private ProviderTypeModel _providerType1;
        private int _providerTypeIdMainProvider = ProviderType.MainProvider;
        private int _providerTypeIdEmployerProvider = ProviderType.EmployerProvider;
        private int _providerTypeIdSupportingProvider  = ProviderType.SupportingProvider;
        private OrganisationTypeModel _organisationTypeModel1;
        private int _organisationTypeId1;
        private ProviderTypeModel _providerType2;
        private ProviderTypeModel _providerType3;

        [OneTimeSetUp]
        public void Before_the_tests()
        {
            _organisationTypeId1 = 1;
            _repository = new OrganisationRepository(_databaseService.WebConfiguration);
            _statusActive = new OrganisationStatusModel {Id = _organisationStatusIdActive, Status = "Live", CreatedAt = DateTime.Now,CreatedBy="TestSystem"};
            _statusOnboarding = new OrganisationStatusModel { Id = _organisationStatusIdOnboarding, Status = "Live", CreatedAt = DateTime.Now, CreatedBy = "TestSystem" };
            OrganisationStatusHandler.InsertRecord(_statusActive);
            OrganisationStatusHandler.InsertRecord(_statusOnboarding);
            _providerType1 = new ProviderTypeModel {Id = _providerTypeIdMainProvider, ProviderType = "Main Provider", Description = "provider type description 1", CreatedAt = DateTime.Now, CreatedBy = "TestSystem", Status="Live"};
            _providerType2 = new ProviderTypeModel { Id = _providerTypeIdEmployerProvider, ProviderType = "Employer Provider", Description = "provider type description 2", CreatedAt = DateTime.Now, CreatedBy = "TestSystem", Status = "Live" };
            _providerType3 = new ProviderTypeModel { Id = _providerTypeIdSupportingProvider, ProviderType = "Supporting Provider", Description = "provider type description 3", CreatedAt = DateTime.Now, CreatedBy = "TestSystem", Status = "Live" };
            ProviderTypeHandler.InsertRecords(new List<ProviderTypeModel>{_providerType1, _providerType2, _providerType3});
            _organisationTypeModel1 = new OrganisationTypeModel { Id = _organisationTypeId1, Type = "organisation type 10", Description = "organisation type description", CreatedAt = DateTime.Now, CreatedBy = "TestSystem", Status = "Live" };
            OrganisationTypeHandler.InsertRecord(_organisationTypeModel1);
        }

        [OneTimeTearDown]
        public void Tear_down()
        {
            OrganisationHandler.DeleteAllRecords();
            OrganisationTypeHandler.DeleteRecord(_organisationTypeId1);
            ProviderTypeHandler.DeleteAllRecords();
            OrganisationStatusHandler.DeleteRecords(new List<int> { _statusActive.Id, _statusOnboarding.Id });
        }
    }
}
