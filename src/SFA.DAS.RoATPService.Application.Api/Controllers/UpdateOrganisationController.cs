using SFA.DAS.RoATPService.Api.Types.Models.UpdateOrganisation;

namespace SFA.DAS.RoATPService.Application.Api.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SFA.DAS.RoATPService.Application.Api.Middleware;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using System.Collections.Generic;
    using System.Net;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using RoATPService.Api.Types.Models;
    using System.Threading.Tasks;

    [Authorize(Roles = "RoATPServiceInternalAPI")]
    [Route("api/v1/[controller]")]
    public class UpdateOrganisationController : Controller
    {
        private readonly ILogger<UpdateOrganisationController> _logger;
        private readonly IMediator _mediator;

        public UpdateOrganisationController(ILogger<UpdateOrganisationController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPut]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        [Route("legalName")]
        public async Task<IActionResult> UpdateLegalName([FromBody] UpdateOrganisationLegalNameRequest updateLegalNameRequest)
        {
            return Ok(await _mediator.Send(updateLegalNameRequest));
        }

        [HttpPut]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        [Route("tradingName")]
        public async Task<IActionResult> UpdateTradingName([FromBody] UpdateOrganisationTradingNameRequest updateLegalNameRequest)
        {
            return Ok(await _mediator.Send(updateLegalNameRequest));
        }

        [HttpPut]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        [Route("status")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateOrganisationStatusRequest updateStatusRequest)
        {
            return Ok(await _mediator.Send(updateStatusRequest));
        }

        [HttpPut]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        [Route("parentCompanyGuarantee")]
        public async Task<IActionResult> UpdateParentCompanyGuarantee([FromBody] UpdateOrganisationParentCompanyGuaranteeRequest request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpPut]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        [Route("financialTrackRecord")]
        public async Task<IActionResult> UpdateFinancialTrackRecord([FromBody] UpdateOrganisationFinancialTrackRecordRequest request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpPut]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        [Route("providerType")]
        public async Task<IActionResult> UpdateProviderType([FromBody] UpdateOrganisationProviderTypeRequest updateProviderTypeRequest)
        {
            return Ok(await _mediator.Send(updateProviderTypeRequest));
        }
    }
}
