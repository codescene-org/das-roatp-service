using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Application.Services
{
    public class OrganisationStatusManager: IOrganisationStatusManager
    {
        public bool ShouldChangeStatusToOnboarding(int newProviderTypeId, int previousProviderTypeId, int previousOrganisationStatusId)
        {
            var isActive = IsOrganisationStatusActive(previousOrganisationStatusId);

            return isActive && previousProviderTypeId == ProviderType.SupportingProvider
                   && (newProviderTypeId == ProviderType.MainProvider || newProviderTypeId == ProviderType.EmployerProvider);
        }

        public bool IsOrganisationStatusActive(int organisationStatusId)
        {
            return organisationStatusId == OrganisationStatus.Active || organisationStatusId == OrganisationStatus.ActiveNotTakingOnApprentices;
        }

        public bool ShouldChangeStatustoActiveAndSetStartDateToToday(int newProviderTypeId, int previousProviderTypeId, int previousOrganisationStatusId)
        {
            var isOnboarding = (previousOrganisationStatusId == OrganisationStatus.Onboarding);

            return isOnboarding &&
                   (previousProviderTypeId == ProviderType.MainProvider || previousProviderTypeId == ProviderType.EmployerProvider) &&
                   newProviderTypeId == ProviderType.SupportingProvider;
        }
    }
}
