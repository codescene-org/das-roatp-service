using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Api.Types.Models;
using SFA.DAS.RoATPService.Application.Exceptions;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Application.Validators;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Application.Handlers
{
    public class GetOrganisationCategoriesHandler : IRequestHandler<GetOrganisationCategoriesRequest, IEnumerable<OrganisationCategory>>
    {
        private readonly ILookupDataRepository _repository;
        private readonly ILogger<GetOrganisationCategoriesHandler> _logger;
        private readonly IProviderTypeValidator _providerTypeValidator;

        public GetOrganisationCategoriesHandler(ILookupDataRepository repository,
            ILogger<GetOrganisationCategoriesHandler> logger, IProviderTypeValidator providerTypeValidator)
        {
            _repository = repository;
            _logger = logger;
            _providerTypeValidator = providerTypeValidator;
        }

        public async Task<IEnumerable<OrganisationCategory>> Handle(GetOrganisationCategoriesRequest request, CancellationToken cancellationToken)
        {
            if (!_providerTypeValidator.IsValidProviderTypeId(request.ProviderTypeId))
            {
                string invalidProviderTypeError = $@"Invalid Provider Type Id [{request.ProviderTypeId}]";
                _logger.LogInformation(invalidProviderTypeError);
                throw new BadRequestException(invalidProviderTypeError);
            }

            _logger.LogInformation($@"Handling Organisation Categories lookup for Provider Type Id [{request.ProviderTypeId}]");

            try
            {
                return await _repository.GetOrganisationCategories(request.ProviderTypeId);
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Unable to retrieve Organisation Categories for Provider Type Id [{request.ProviderTypeId}]", ex);
                throw new ApplicationException(ex.Message);
            }
        }
    }
}

