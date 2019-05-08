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
    using Exceptions;
    using Validators;

    public class GetOrganisationTypesHandler : IRequestHandler<GetOrganisationTypesRequest, IEnumerable<OrganisationType>>
    {
        private readonly ILookupDataRepository _repository;
        private readonly ILogger<GetOrganisationTypesHandler> _logger;
        private readonly IProviderTypeValidator _providerTypeValidator;

        public GetOrganisationTypesHandler(ILookupDataRepository repository,
            ILogger<GetOrganisationTypesHandler> logger, IProviderTypeValidator providerTypeValidator)
        {
            _repository = repository;
            _logger = logger;
            _providerTypeValidator = providerTypeValidator;
        }

        public async Task<IEnumerable<OrganisationType>> Handle(GetOrganisationTypesRequest request, CancellationToken cancellationToken)
        {
            if (!_providerTypeValidator.IsValidProviderTypeId(request.ProviderTypeId))
            {
                string invalidProviderTypeError = $@"Invalid Provider Type Id [{request.ProviderTypeId}]";
                _logger.LogInformation(invalidProviderTypeError);
                throw new BadRequestException(invalidProviderTypeError);
            }

            _logger.LogInformation($@"Handling Organisation Types lookup for Provider Type Id [{request.ProviderTypeId}]");

            try
            {
                return await _repository.GetOrganisationTypesForProviderTypeId(request.ProviderTypeId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to retrieve Organisation Types", ex);
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
