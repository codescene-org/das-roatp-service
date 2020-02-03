using MediatR;
using SFA.DAS.RoATPService.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.RoATPService.Api.Types.Models
{
    public class GetEngagementsFromEventIdPagedRequest : IRequest<IEnumerable<Engagement>>
    {
        public int FromEventId { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
