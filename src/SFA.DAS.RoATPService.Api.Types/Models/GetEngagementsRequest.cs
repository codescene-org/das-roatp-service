using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Api.Types.Models
{
    public class GetEngagementsRequest : IRequest<IEnumerable<Engagement>>
    {
    }
}
