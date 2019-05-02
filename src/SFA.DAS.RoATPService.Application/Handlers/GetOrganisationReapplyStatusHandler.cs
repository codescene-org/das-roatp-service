namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Domain;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.RoATPService.Application.Interfaces;

    public class GetOrganisationReapplyStatusHandler : IRequestHandler<GetOrganisationReapplyStatusRequest, OrganisationReapplyStatus>
    { 
        private IOrganisationRepository _repository;
        private ILogger<GetOrganisationReapplyStatusHandler> _logger;

        public GetOrganisationReapplyStatusHandler(IOrganisationRepository repository,
            ILogger<GetOrganisationReapplyStatusHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<OrganisationReapplyStatus> Handle(GetOrganisationReapplyStatusRequest request, CancellationToken cancellationToken)
        {
            return await _repository.GetOrganisationReapplyStatus(request.OrganisationId);
        }
    }
}
