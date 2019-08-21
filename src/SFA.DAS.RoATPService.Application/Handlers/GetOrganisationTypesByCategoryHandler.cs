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
        private readonly IOrganisationCategoryValidator _categoryValidator;
        public GetOrganisationTypesByCategoryHandler(ILookupDataRepository repository,
            ILogger<GetOrganisationTypesByCategoryHandler> logger, IProviderTypeValidator providerTypeValidator, IOrganisationCategoryValidator categoryValidator)
        {
            _repository = repository;
            _logger = logger;
            _providerTypeValidator = providerTypeValidator;
            _categoryValidator = categoryValidator;
        }

        public async Task<IEnumerable<OrganisationType>> Handle(GetOrganisationTypesByCategoryRequest request, CancellationToken cancellationToken)
        {
            if (!_providerTypeValidator.IsValidProviderTypeId(request.ProviderTypeId))
            {
                string invalidProviderTypeError = $@"Invalid Provider Type Id [{request.ProviderTypeId}]";
                _logger.LogInformation(invalidProviderTypeError);
                throw new BadRequestException(invalidProviderTypeError);
            }

            if (!_categoryValidator.IsValidCategoryId(request.CategoryId))
            {
                string invalidMessage = $@"Invalid Category Id [{request.CategoryId}]";
                _logger.LogInformation(invalidMessage);
                throw new BadRequestException(invalidMessage);
            }

            _logger.LogInformation($@"Handling Organisation Types for a category lookup for Provider Type Id [{request.ProviderTypeId}], Category Id [{request.CategoryId}]");

            try
            {
                return await _repository.GetOrganisationTypesForProviderTypeIdCategoryId(request.ProviderTypeId, request.CategoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to retrieve Organisation Types for a category lookup for Provider Type Id [{request.ProviderTypeId}], Category Id [{request.CategoryId}]", ex);
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
