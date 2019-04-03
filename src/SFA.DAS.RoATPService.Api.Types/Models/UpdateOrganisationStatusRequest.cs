namespace SFA.DAS.RoATPService.Api.Types.Models
{
    using System;
    using MediatR;

    public class UpdateOrganisationStatusRequest : IRequest<bool>
    {
        public Guid OrganisationId { get; set; }
        public int OrganisationStatusId { get; set; }
        public int? RemovedReasonId { get; set; }
        public string UpdatedBy { get; set; }
    }
}
