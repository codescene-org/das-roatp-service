namespace SFA.DAS.RoATPService.Application.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.RoATPService.Api.Types.Models;
    using SFA.DAS.RoATPService.Application.Api.Middleware;
    using Swashbuckle.AspNetCore.SwaggerGen;

    [Authorize(Roles = "RoATPServiceInternalAPI")]
    [Route("api/v1/duplicateCheck")]
    public class DuplicateCheckController : Controller
    {
        private ILogger<LookupDataController> _logger;

        private IMediator _mediator;

        public DuplicateCheckController(ILogger<LookupDataController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        [Route("ukprn")]
        public async Task<IActionResult> UKPRN(DuplicateUKPRNCheckRequest request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        [Route("companyNumber")]
        public async Task<IActionResult> CompanyNumber(DuplicateCompanyNumberCheckRequest request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        [Route("charityNumber")]
        public async Task<IActionResult> CharityNumber(DuplicateCharityNumberCheckRequest request)
        {
            return Ok(await _mediator.Send(request));
        }
    }
}
