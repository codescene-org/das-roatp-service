using System;
using MediatR;

namespace SFA.DAS.RoATPService.Api.Types.Models.UpdateOrganisation
{
    public class UpdateOrganisationTradingNameRequest : IRequest<bool>
    {
        public Guid OrganisationId { get; set; }
        public string TradingName { get; set; }
        public string UpdatedBy { get; set; }
    }
}
