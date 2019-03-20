namespace SFA.DAS.RoATPService.Application.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using Domain;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Middleware;
    using RoATPService.Api.Types.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    [Authorize(Roles = "RoATPServiceInternalAPI")]
    [Route("api/v1/[controller]")]
    public class OrganisationController : Controller
    {
        private readonly ILogger<OrganisationController> _logger;
        private readonly IMediator _mediator;

        public OrganisationController(ILogger<OrganisationController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Organisation))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        [Route("get/{organisationId}")]
        public async Task<IActionResult> Get(Guid organisationId)
        {
            GetOrganisationRequest getOrganisationRequest =  new GetOrganisationRequest {OrganisationId = organisationId};

            Organisation organisation = await _mediator.Send(getOrganisationRequest);

            return Ok(organisation);
        }

        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] CreateOrganisationRequest createOrganisationRequest)
        {
            bool result = await _mediator.Send(createOrganisationRequest);

            return Ok(result);
        }

        [Obsolete("Use operations in UpdateOrganisationController instead")]
        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] UpdateOrganisationRequest updateOrganisationRequest)
        {
            bool result = await _mediator.Send(updateOrganisationRequest);

            return Ok(result);
        }
    }
}