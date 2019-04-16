namespace SFA.DAS.RoATPService.Application.Services
{
    public interface IOrganisationStatusManager
    {
        bool ShouldChangeStatusToOnboarding(int newProviderTypeId, int currentProviderTypeId, int currentOrganisationStatusId);

        bool IsOrganisationStatusActive(int organisationStatusId);

        bool ShouldChangeStatustoActiveAndSetStartDateToToday(int newProviderTypeId, int currentProviderTypeId,int currentOrganisationStatusId);

    }
}