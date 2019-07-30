namespace SFA.DAS.RoATPService.Domain
{
    using System;

    public class OrganisationRegisterStatus
    {
        public bool UkprnOnRegister { get; set; }
        public Guid? OrganisationId { get; set; }
        public int? ProviderTypeId { get; set; }
        public int? StatusId { get; set; }
        public int? RemovedReasonId { get; set; }
        public DateTime? StatusDate { get; set; }
    }
}
