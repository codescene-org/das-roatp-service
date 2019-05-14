using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Application.Services
{
    public class OrganisationStatusManager: IOrganisationStatusManager
    {
        public bool ShouldChangeStatusToOnboarding(int newProviderTypeId, int currentProviderTypeId, int currentOrganisationStatusId)
        {
            var isActive = IsOrganisationStatusActive(currentOrganisationStatusId);

            return isActive && currentProviderTypeId == ProviderType.SupportingProvider
                   && (newProviderTypeId == ProviderType.MainProvider || newProviderTypeId == ProviderType.EmployerProvider);
        }

        public bool IsOrganisationStatusActive(int organisationStatusId)
        {
            return organisationStatusId == OrganisationStatus.Active || organisationStatusId == OrganisationStatus.ActiveNotTakingOnApprentices;
        }

        public bool ShouldChangeStatustoActiveAndSetStartDateToToday(int newProviderTypeId, int currentProviderTypeId, int currentOrganisationStatusId)
        {
            var isOnboarding = (currentOrganisationStatusId == OrganisationStatus.Onboarding);

            return isOnboarding &&
                   (currentProviderTypeId == ProviderType.MainProvider || currentProviderTypeId == ProviderType.EmployerProvider) &&
                   newProviderTypeId == ProviderType.SupportingProvider;
        }
    }
}
