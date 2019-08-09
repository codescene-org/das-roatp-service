﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Api.Types.Models;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Application.Handlers
{
    public class GetEngagementsHandler : IRequestHandler<GetEngagementsRequest, IEnumerable<Engagement>>
    {
        private IOrganisationRepository _repository;
        private ILogger<GetEngagementsHandler> _logger;

        public GetEngagementsHandler(IOrganisationRepository repository, ILogger<GetEngagementsHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<Engagement>> Handle(GetEngagementsRequest request, CancellationToken cancellationToken)
        {
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
