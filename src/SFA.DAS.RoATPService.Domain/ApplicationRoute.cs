namespace SFA.DAS.RoATPService.Domain
{
    public class ApplicationRoute : BaseEntity
    {
        public int Id { get; set; }
        public string Route { get; set; }
        public string Description { get; set; }
    }
}
