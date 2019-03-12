namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Exceptions;
    using Interfaces;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Validators;

    public class OrganisationSearchHandler : IRequestHandler<OrganisationSearchRequest, OrganisationSearchResults>
    {
        private readonly IOrganisationSearchRepository _organisationSearchRepository;
        private readonly ILogger<OrganisationSearchHandler> _logger;
        private readonly IOrganisationSearchValidator _organisationSearchValidator;

        public OrganisationSearchHandler(IOrganisationSearchRepository repository, ILogger<OrganisationSearchHandler> logger, 
                                         IOrganisationSearchValidator organisationSearchOrganisationSearchValidator)
        {
            _organisationSearchRepository = repository;
            _logger = logger;
            _organisationSearchValidator = organisationSearchOrganisationSearchValidator;
        }

        public async Task<OrganisationSearchResults> Handle(OrganisationSearchRequest request, CancellationToken cancellationToken)
        {
            if (!_organisationSearchValidator.IsValidSearchTerm(request.SearchTerm))
            {
                string invalidSearchTermError = $@"Invalid Organisation Search term [{request.SearchTerm}]";
                _logger.LogInformation(invalidSearchTermError);
                throw new BadRequestException(invalidSearchTermError);
            }

            _logger.LogInformation($@"Handling Organisation Search for [{request.SearchTerm}]");

            if (_organisationSearchValidator.IsValidUKPRN(request.SearchTerm))
            {
                var searchResults = await _organisationSearchRepository.OrganisationSearchByUkPrn(request.SearchTerm);
                if (searchResults.TotalCount > 0)
                {
                    return searchResults;
                }
            }

            return await _organisationSearchRepository.OrganisationSearchByName(request.SearchTerm);
        }
    }
}
