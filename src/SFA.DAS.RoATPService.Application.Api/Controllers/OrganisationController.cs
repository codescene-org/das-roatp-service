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
            return Ok(await _mediator.Send(createOrganisationRequest));
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(OrganisationRegisterStatus))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        [Route("register-status")]
        public async Task<IActionResult> GetRegisterStatus(GetOrganisationRegisterStatusRequest getOrganisationRegisterStatusRequest)
        {
            return Ok(await _mediator.Send(getOrganisationRegisterStatusRequest));
        }

    }
}