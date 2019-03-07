namespace SFA.DAS.RoATPService.Api.Types.Models
{
    using System;
    using MediatR;

    public class DuplicateCompanyNumberCheckRequest : IRequest<bool>
    {
        public Guid OrganisationId { get; set; }
        public string CompanyNumber { get; set; }
    }
}
