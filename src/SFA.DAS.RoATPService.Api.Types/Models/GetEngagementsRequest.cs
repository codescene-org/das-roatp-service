using System.Collections.Generic;
using MediatR;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Api.Types.Models
{
    public class GetEngagementsRequest : IRequest<IEnumerable<Engagement>>
    {
        public long SinceEventId { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
