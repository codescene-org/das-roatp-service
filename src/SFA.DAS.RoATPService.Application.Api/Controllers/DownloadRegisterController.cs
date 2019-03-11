
namespace SFA.DAS.RoATPService.Application.Api.Controllers
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.RoATPService.Application.Api.Middleware;
    using SFA.DAS.RoATPService.Data;
    using Swashbuckle.AspNetCore.SwaggerGen;

    [Authorize(Roles = "RoATPServiceInternalAPI")]
    [Route("api/v1/download")]
    public class DownloadRegisterController : Controller
    {
        private ILogger<DownloadRegisterController> _logger;
        private IDownloadRegisterRepository _repository;

        public DownloadRegisterController(ILogger<DownloadRegisterController> logger,
                                          IDownloadRegisterRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet("complete")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<IDictionary<string, object>>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> CompleteRegister()
        {
            _logger.LogInformation($"Received request to download complete register");

            try
            {
                var result = await _repository.GetCompleteRegister();
                return Ok(result);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogInformation($"Could not generate data for complete register download due to : {sqlEx.Message}");
                return NoContent();
            }
        }

        [HttpGet("audit")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<IDictionary<string, object>>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> AuditHistory()
        {
            _logger.LogInformation($"Received request to download register audit history");

            try
            {
                var result = await _repository.GetAuditHistory();
                return Ok(result);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogInformation($"Could not generate data for register audit history due to : {sqlEx.Message}");
                return NoContent();
            }
        }


        [HttpGet("roatp-summary")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<IDictionary<string, object>>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> RoatpSummary()
        {
            _logger.LogInformation($"Received request to download complete register");

            try
            {
                return Ok(await _repository.GetRoatpCsvSummary());
            }
            catch (SqlException sqlEx)
            {
                _logger.LogInformation($"Could not generate data for roatp summary due to : {sqlEx.Message}");
                return NoContent();
            }
        }
    }
}
