namespace SFA.DAS.RoATPService.Domain
{
    public class OrganisationStatus : BaseEntity
    {
        public int Id { get; set; }
        public string Status { get; set; }

        public const int Removed = 0;
        public const int Active = 1;
        public const int ActiveNotTakingOnApprentices = 2;
        public const int Onboarding = 3;
    }
}
