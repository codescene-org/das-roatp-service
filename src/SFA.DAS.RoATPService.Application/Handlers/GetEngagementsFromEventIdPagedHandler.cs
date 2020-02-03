using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Api.Types.Models;
using SFA.DAS.RoATPService.Application.Exceptions;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.Handlers
{
    public class GetEngagementsFromEventIdPagedHandler : IRequestHandler<GetEngagementsFromEventIdPagedRequest, IEnumerable<Engagement>>
    {
        private readonly IOrganisationRepository _repository;
        private readonly ILogger<GetEngagementsFromEventIdPagedHandler> _logger;

        public GetEngagementsFromEventIdPagedHandler(IOrganisationRepository repository, ILogger<GetEngagementsFromEventIdPagedHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<Engagement>> Handle(GetEngagementsFromEventIdPagedRequest request, CancellationToken cancellationToken)
        {
            if (request.FromEventId.Equals(0))
            {
                string invalidFromEventIdError = $@"Invalid From Event Id [{request.FromEventId}]";
                _logger.LogInformation(invalidFromEventIdError);
                throw new BadRequestException(invalidFromEventIdError);
            }

            if (request.PageSize.Equals(0))
            {
                string invalidPageSizeError = $@"Invalid Page Size [{request.PageSize}]";
                _logger.LogInformation(invalidPageSizeError);
                throw new BadRequestException(invalidPageSizeError);
            }

            if (request.PageNumber.Equals(0))
            {
                string invalidPageNumberError = $@"Invalid Page Number [{request.PageNumber}]";
                _logger.LogInformation(invalidPageNumberError);
                throw new BadRequestException(invalidPageNumberError);
            }

            try
            {
                return await _repository.GetEngagements();
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to retrieve list of engagements", ex);
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
