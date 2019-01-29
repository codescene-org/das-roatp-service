namespace SFA.DAS.RoATPService.Api.Types.Models
{
    using Domain;
    using MediatR;

    public class UpdateOrganisationRequest : IRequest<bool>
    {
        public Organisation Organisation { get; set; }

        public string Username { get; set; }
    }
}
