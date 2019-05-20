using System;
using MediatR;

namespace SFA.DAS.RoATPService.Api.Types.Models
{
    public class UpdateOrganisationFinancialTrackRecordRequest : IRequest<bool>
    {
        public Guid OrganisationId { get; set; }
        public bool FinancialTrackRecord { get; set; }
        public string UpdatedBy { get; set; }
    }
}
