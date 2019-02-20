namespace SFA.DAS.RoATPService.Application.Api.Controllers
{
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.RoATPService.Api.Types.Models;
    using SFA.DAS.RoATPService.Application.Api.Middleware;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    [Authorize(Roles = "RoATPServiceInternalAPI")]
    [Route("api/v1/[controller]")]
    public class ImportController : Controller
    {
        private readonly ILogger<ImportController> _logger;
        private readonly IMediator _mediator;

        public ImportController(ILogger<ImportController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Index([FromBody] RegisterImportRequest createOrganisationRequest)
        {
            var result = await _mediator.Send(createOrganisationRequest);

            if (result.Success)
            {

                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
