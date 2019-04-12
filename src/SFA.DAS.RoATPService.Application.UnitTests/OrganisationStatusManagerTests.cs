using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Services;
using SFA.DAS.RoATPService.Application.Validators;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Application.UnitTests
{
    [TestFixture]
    public class OrganisationStatusManagerTests
    {
        private OrganisationStatusManager _manager;

        [SetUp]
        public void Before_each_test()
        {
            _manager = new OrganisationStatusManager();
        }


        [TestCase(OrganisationStatus.Active,true)]
        [TestCase(OrganisationStatus.ActiveNotTakingOnApprentices, true)]
        [TestCase(OrganisationStatus.Removed, false)]
        [TestCase(OrganisationStatus.Onboarding, false)]

        public void Manager_returns_matching_active_status_for_matching_organisation_status_values(int organisationStatusId, bool expectedActiveStatus)
        {
            var isActive = _manager.IsOrganisationStatusActive(organisationStatusId);

            Assert.AreEqual(expectedActiveStatus,isActive);

        }

        [TestCase(ProviderType.MainProvider, ProviderType.SupportingProvider, OrganisationStatus.Active, true)]
        [TestCase(ProviderType.EmployerProvider, ProviderType.SupportingProvider, OrganisationStatus.Active, true)]
        [TestCase(ProviderType.MainProvider, ProviderType.SupportingProvider, OrganisationStatus.ActiveNotTakingOnApprentices, true)]
        [TestCase(ProviderType.EmployerProvider, ProviderType.SupportingProvider, OrganisationStatus.ActiveNotTakingOnApprentices, true)]
        [TestCase(ProviderType.MainProvider, ProviderType.SupportingProvider, OrganisationStatus.Onboarding, false)]
        [TestCase(ProviderType.EmployerProvider, ProviderType.SupportingProvider, OrganisationStatus.Removed, false)]
        [TestCase(ProviderType.SupportingProvider, ProviderType.SupportingProvider, OrganisationStatus.Active, false)]
        [TestCase(ProviderType.SupportingProvider, ProviderType.MainProvider, OrganisationStatus.Active, false)]
        [TestCase(ProviderType.SupportingProvider, ProviderType.EmployerProvider, OrganisationStatus.Active, false)]
        [TestCase(ProviderType.SupportingProvider, ProviderType.SupportingProvider, OrganisationStatus.ActiveNotTakingOnApprentices, false)]
        [TestCase(ProviderType.SupportingProvider, ProviderType.MainProvider, OrganisationStatus.ActiveNotTakingOnApprentices, false)]
        [TestCase(ProviderType.SupportingProvider, ProviderType.EmployerProvider, OrganisationStatus.ActiveNotTakingOnApprentices, false)]
        [TestCase(ProviderType.SupportingProvider, ProviderType.SupportingProvider, OrganisationStatus.Removed, false)]
        [TestCase(ProviderType.SupportingProvider, ProviderType.MainProvider, OrganisationStatus.Removed, false)]
        [TestCase(ProviderType.SupportingProvider, ProviderType.EmployerProvider, OrganisationStatus.Removed, false)]
        [TestCase(ProviderType.SupportingProvider, ProviderType.SupportingProvider, OrganisationStatus.Onboarding, false)]
        [TestCase(ProviderType.SupportingProvider, ProviderType.MainProvider, OrganisationStatus.Onboarding, false)]
        [TestCase(ProviderType.SupportingProvider, ProviderType.EmployerProvider, OrganisationStatus.Onboarding, false)]
        [TestCase(ProviderType.MainProvider, ProviderType.MainProvider, OrganisationStatus.Active, false)]
        [TestCase(ProviderType.MainProvider, ProviderType.EmployerProvider, OrganisationStatus.Active, false)]
        [TestCase(ProviderType.MainProvider, ProviderType.MainProvider, OrganisationStatus.ActiveNotTakingOnApprentices, false)]
        [TestCase(ProviderType.MainProvider, ProviderType.EmployerProvider, OrganisationStatus.ActiveNotTakingOnApprentices, false)]
        [TestCase(ProviderType.MainProvider, ProviderType.MainProvider, OrganisationStatus.Removed, false)]
        [TestCase(ProviderType.MainProvider, ProviderType.EmployerProvider, OrganisationStatus.Removed, false)]
        [TestCase(ProviderType.MainProvider, ProviderType.MainProvider, OrganisationStatus.Onboarding, false)]
        [TestCase(ProviderType.MainProvider, ProviderType.EmployerProvider, OrganisationStatus.Onboarding, false)]
        [TestCase(ProviderType.EmployerProvider, ProviderType.MainProvider, OrganisationStatus.Active, false)]
        [TestCase(ProviderType.EmployerProvider, ProviderType.EmployerProvider, OrganisationStatus.Active, false)]
        [TestCase(ProviderType.EmployerProvider, ProviderType.MainProvider, OrganisationStatus.ActiveNotTakingOnApprentices, false)]
        [TestCase(ProviderType.EmployerProvider, ProviderType.EmployerProvider, OrganisationStatus.ActiveNotTakingOnApprentices, false)]
        [TestCase(ProviderType.EmployerProvider, ProviderType.MainProvider, OrganisationStatus.Removed, false)]
        [TestCase(ProviderType.EmployerProvider, ProviderType.EmployerProvider, OrganisationStatus.Removed, false)]
        [TestCase(ProviderType.EmployerProvider, ProviderType.MainProvider, OrganisationStatus.Onboarding, false)]
        [TestCase(ProviderType.EmployerProvider, ProviderType.EmployerProvider, OrganisationStatus.Onboarding, false)]
        public void Manager_returns_onboarding_change_status_for_current_organisation_status_and_changing_provider_type(
            int newProviderTypeId, int currentProviderTypeId, int currentOrganisationStatusId,
            bool expectedShouldChange)
        {
            var shouldChange =
                _manager.ShouldChangeStatusToOnboarding(newProviderTypeId, currentProviderTypeId,
                    currentOrganisationStatusId);
            Assert.AreEqual(expectedShouldChange, shouldChange);
        }


        [TestCase(ProviderType.SupportingProvider, ProviderType.MainProvider, OrganisationStatus.Onboarding, true)]
        [TestCase(ProviderType.SupportingProvider, ProviderType.EmployerProvider, OrganisationStatus.Onboarding, true)]
        [TestCase(ProviderType.SupportingProvider, ProviderType.SupportingProvider, OrganisationStatus.Onboarding, false)]
        [TestCase(ProviderType.MainProvider, ProviderType.MainProvider, OrganisationStatus.Onboarding, false)]
        [TestCase(ProviderType.MainProvider, ProviderType.EmployerProvider, OrganisationStatus.Onboarding, false)]
        [TestCase(ProviderType.MainProvider, ProviderType.SupportingProvider, OrganisationStatus.Onboarding, false)]
        [TestCase(ProviderType.EmployerProvider, ProviderType.MainProvider, OrganisationStatus.Onboarding, false)]
        [TestCase(ProviderType.EmployerProvider, ProviderType.EmployerProvider, OrganisationStatus.Onboarding, false)]
        [TestCase(ProviderType.EmployerProvider, ProviderType.SupportingProvider, OrganisationStatus.Onboarding, false)]
        [TestCase(ProviderType.MainProvider, ProviderType.MainProvider, OrganisationStatus.Active, false)]
        [TestCase(ProviderType.MainProvider, ProviderType.EmployerProvider, OrganisationStatus.Active, false)]
        [TestCase(ProviderType.MainProvider, ProviderType.SupportingProvider, OrganisationStatus.Active, false)]
        [TestCase(ProviderType.EmployerProvider, ProviderType.MainProvider, OrganisationStatus.Active, false)]
        [TestCase(ProviderType.EmployerProvider, ProviderType.EmployerProvider, OrganisationStatus.Active, false)]
        [TestCase(ProviderType.EmployerProvider, ProviderType.SupportingProvider, OrganisationStatus.Active, false)]
        [TestCase(ProviderType.MainProvider, ProviderType.MainProvider, OrganisationStatus.ActiveNotTakingOnApprentices, false)]
        [TestCase(ProviderType.MainProvider, ProviderType.EmployerProvider, OrganisationStatus.ActiveNotTakingOnApprentices, false)]
        [TestCase(ProviderType.MainProvider, ProviderType.SupportingProvider, OrganisationStatus.ActiveNotTakingOnApprentices, false)]
        [TestCase(ProviderType.EmployerProvider, ProviderType.MainProvider, OrganisationStatus.ActiveNotTakingOnApprentices, false)]
        [TestCase(ProviderType.EmployerProvider, ProviderType.EmployerProvider, OrganisationStatus.ActiveNotTakingOnApprentices, false)]
        [TestCase(ProviderType.EmployerProvider, ProviderType.SupportingProvider, OrganisationStatus.ActiveNotTakingOnApprentices, false)]
        [TestCase(ProviderType.MainProvider, ProviderType.MainProvider, OrganisationStatus.Removed, false)]
        [TestCase(ProviderType.MainProvider, ProviderType.EmployerProvider, OrganisationStatus.Removed, false)]
        [TestCase(ProviderType.MainProvider, ProviderType.SupportingProvider, OrganisationStatus.Removed, false)]
        [TestCase(ProviderType.EmployerProvider, ProviderType.MainProvider, OrganisationStatus.Removed, false)]
        [TestCase(ProviderType.EmployerProvider, ProviderType.EmployerProvider, OrganisationStatus.Removed, false)]
        [TestCase(ProviderType.EmployerProvider, ProviderType.SupportingProvider, OrganisationStatus.Removed, false)]
        public void Manager_returns_active_and_start_date__change_status_for_changing_provider_type_and_organisation_status(
            int newProviderTypeId, int currentProviderTypeId, int currentOrganisationStatusId,
            bool expectedShouldChange)
        {
            var shouldChange =
                _manager.ShouldChangeStatustoActiveAndSetStartDateToToday(newProviderTypeId, currentProviderTypeId,
                    currentOrganisationStatusId);
            Assert.AreEqual(expectedShouldChange, shouldChange);
        }
    }
}
