namespace SFA.DAS.RoATPService.Application.Services
{
    public interface IOrganisationStatusManager
    {
        bool ShouldChangeStatusToOnboarding(int newProviderTypeId, int previousProviderTypeId, int previousOrganisationStatusId);

        bool IsOrganisationStatusActive(int organisationStatusId);

        bool ShouldChangeStatustoActiveAndSetStartDateToToday(int newProviderTypeId, int previousProviderTypeId,int previousOrganisationStatusId);

    }
}