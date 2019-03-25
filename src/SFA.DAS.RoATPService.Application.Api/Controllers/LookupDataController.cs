namespace SFA.DAS.RoATPService.Application.Api.Controllers
{
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RoATPService.Api.Types.Models;

    [Authorize(Roles = "RoATPServiceInternalAPI")]
    [Route("api/v1/lookupData")]
    [ApiController]
    public class LookupDataController : Controller
    {
        private ILogger<LookupDataController> _logger;

        private IMediator _mediator;

        public LookupDataController(ILogger<LookupDataController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        [Route("providerTypes")]
        public async Task<IActionResult> ProviderTypes()
        {
            var request = new GetProviderTypesRequest();

            return Ok(await _mediator.Send(request));
        }

        [HttpGet]
        [Route("organisationTypes")]
        public async Task<IActionResult> OrganisationTypes(int providerTypeId)
        {
            var request = new GetOrganisationTypesRequest {ProviderTypeId = providerTypeId};

            return Ok(await _mediator.Send(request));
        }

        [HttpGet]
        [Route("organisationStatuses")]
        public async Task<IActionResult> OrganisationStatuses()
        {
            var request = new GetOrganisationStatusesRequest();

            return Ok(await _mediator.Send(request));
        }

        [HttpGet]
        [Route("removedReasons")]
        public async Task<IActionResult> RemovedReasons()
        {
            var request = new GetRemovedReasonsRequest();

            return Ok(await _mediator.Send(request));
        }
    }
}