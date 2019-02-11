namespace SFA.DAS.RoATPService.Domain
{
    using System.ComponentModel;

    public class ProviderType : BaseEntity
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
    }
}
