namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Interfaces;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Exceptions;
    using Validators;

    public class UpdateOrganisationApplicationDeterminedDateHandler : IRequestHandler<UpdateOrganisationApplicationDeterminedDateRequest, bool>
    {
        private readonly ILogger<UpdateOrganisationApplicationDeterminedDateHandler> _logger;
        private readonly IOrganisationValidator _validator;
        private readonly IUpdateOrganisationRepository _updateOrganisationRepository;

        public UpdateOrganisationApplicationDeterminedDateHandler(ILogger<UpdateOrganisationApplicationDeterminedDateHandler> logger,
            IOrganisationValidator validator, IUpdateOrganisationRepository updateOrganisationRepository)
        {
            _logger = logger;
            _validator = validator;
            _updateOrganisationRepository = updateOrganisationRepository;

        }

        public async Task<bool> Handle(UpdateOrganisationApplicationDeterminedDateRequest request, CancellationToken cancellationToken)
        {
            if (!_validator.IsValidApplicationDeterminedDate(request.ApplicationDeterminedDate))
            {
                var invalidApplicationDeterminedDate = $@"Invalid Application Determined Date '{request.ApplicationDeterminedDate}'";
                _logger.LogInformation(invalidApplicationDeterminedDate);
                throw new BadRequestException(invalidApplicationDeterminedDate);
              }
    
            var success = await _updateOrganisationRepository.UpdateApplicationDeterminedDate(request.OrganisationId, request.ApplicationDeterminedDate, request.UpdatedBy);

            return await Task.FromResult(success);
        }
    }
}