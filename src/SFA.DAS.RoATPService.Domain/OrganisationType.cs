namespace SFA.DAS.RoATPService.Domain
{
    public class OrganisationType : BaseEntity
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }

        public const int Unassigned = 0;
    }
}
