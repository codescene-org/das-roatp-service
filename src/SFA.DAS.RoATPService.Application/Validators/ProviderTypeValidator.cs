namespace SFA.DAS.RoATPService.Application.Validators
{
    public class ProviderTypeValidator : IProviderTypeValidator
    {
        public bool IsValidProviderTypeId(int providerTypeId)
        {
            return (providerTypeId >= 1 && providerTypeId <= 3);
        }
    }
}
