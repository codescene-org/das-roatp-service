namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Domain;
    using Exceptions;
    using Interfaces;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Validators;

    public class OrganisationSearchHandler : IRequestHandler<OrganisationSearchRequest, IEnumerable<Organisation>>
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

        public Task<IEnumerable<Organisation>> Handle(OrganisationSearchRequest request, CancellationToken cancellationToken)
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
                return _organisationSearchRepository.OrganisationSearchByUkPrn(request.SearchTerm);
            }

            return _organisationSearchRepository.OrganisationSearchByName(request.SearchTerm);
        }
    }
}
