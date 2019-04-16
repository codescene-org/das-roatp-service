namespace SFA.DAS.RoATPService.Domain
{
    public class ProviderType : BaseEntity
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }

        public const int MainProvider = 1;
        public const int EmployerProvider = 2;
        public const int SupportingProvider = 3;
    }
}
