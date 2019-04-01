namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Domain;
    using Interfaces;
    using MediatR;
    using Microsoft.Extensions.Logging;
    
    public class GetOrganisationStatusesHandler : IRequestHandler<GetOrganisationStatusesRequest, IEnumerable<OrganisationStatus>>
    {
        private ILookupDataRepository _repository;
        private ILogger<GetOrganisationStatusesHandler> _logger;
        
        public GetOrganisationStatusesHandler(ILookupDataRepository repository,
            ILogger<GetOrganisationStatusesHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<OrganisationStatus>> Handle(GetOrganisationStatusesRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($@"Handling Organisation Statuses lookup");

            try
            {
                return await _repository.GetOrganisationStatuses();
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to retrieve Organisation Statuses", ex);
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
