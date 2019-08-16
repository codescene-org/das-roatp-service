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
    public class GetOrganisationTypesByCategoryHandler : IRequestHandler<GetOrganisationTypesByCategoryRequest, IEnumerable<OrganisationType>>
    {
      


        private readonly ILookupDataRepository _repository;
        private readonly ILogger<GetOrganisationTypesByCategoryHandler> _logger;
        private readonly IProviderTypeValidator _providerTypeValidator;

        public GetOrganisationTypesByCategoryHandler(ILookupDataRepository repository,
            ILogger<GetOrganisationTypesByCategoryHandler> logger, IProviderTypeValidator providerTypeValidator)
        {
            _repository = repository;
            _logger = logger;
            _providerTypeValidator = providerTypeValidator;
        }

        public async Task<IEnumerable<OrganisationType>> Handle(GetOrganisationTypesByCategoryRequest request, CancellationToken cancellationToken)
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
                return await _repository.GetOrganisationTypesForProviderTypeIdCategoryId(request.ProviderTypeId, request.CategoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to retrieve Organisation Types", ex);
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
