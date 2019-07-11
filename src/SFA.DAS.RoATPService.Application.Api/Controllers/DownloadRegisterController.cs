using System;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.RoATPService.Application.Api.Controllers
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using OfficeOpenXml;
    using Api.Helpers;
    using Middleware;
    using Interfaces;
    using Swashbuckle.AspNetCore.SwaggerGen;

    [Authorize(Roles = "RoATPServiceInternalAPI")]
    [Route("api/v1/download")]
    public class DownloadRegisterController : Controller
    {
        private readonly ILogger<DownloadRegisterController> _logger;
        private readonly IDownloadRegisterRepository _repository;
        private readonly IDataTableHelper _dataTableHelper;

        public DownloadRegisterController(ILogger<DownloadRegisterController> logger,
            IDownloadRegisterRepository repository, IDataTableHelper dataTableHelper)
        {
            _logger = logger;
            _repository = repository;
            _dataTableHelper = dataTableHelper;
        }

        [HttpGet("complete")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(IEnumerable<IDictionary<string, object>>))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
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
                _logger.LogInformation(
                    $"Could not generate data for complete register download due to : {sqlEx.Message}");
                return NoContent();
            }
        }

        [HttpGet("audit")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(IEnumerable<IDictionary<string, object>>))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
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
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(IEnumerable<IDictionary<string, object>>))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> RoatpSummary()
        {
            _logger.LogInformation($"Received request to download roatp summary");

            try
            {
                return Ok(await _repository.GetRoatpSummary());
            }
            catch (SqlException sqlEx)
            {
                _logger.LogInformation($"Could not generate data for roatp summary due to : {sqlEx.Message}");
                return NoContent();
            }
        }


        [HttpGet("roatp-summary/{ukprn}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<IDictionary<string, object>>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> RoatpSummary(string ukprn)
        {
            _logger.LogInformation($"Received request to download roatp summary for ukprn {ukprn}");

            int ukprnAsInt = 0;

            if (!int.TryParse(ukprn, out ukprnAsInt) || (ukprnAsInt < 10000000 || ukprnAsInt > 99999999))
            {
                _logger.LogError($"Could not generate data for invalid ukprn : {ukprn}");
                return BadRequest();
            }

            try
            {
                return Ok(await _repository.GetRoatpSummaryUkprn(ukprnAsInt));
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError($"Could not generate data for roatp summary due to database error",sqlEx);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpGet("roatp-summary/most-recent")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(DateTime?))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(DateTime?))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> MostRecentOrganisation()
        {
            _logger.LogInformation($"Received request to get date of most recent non-onboarding organisation change");
           
            try
            {
                return Ok(await _repository.GetLatestNonOnboardingOrganisationChangeDate());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not generate data for latest organisation date change due to database or orther internal error", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("roatp-summary-xlsx")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<IDictionary<string, object>>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> RoatpSummaryExcel()
        {
            _logger.LogInformation($"Received request to download complete register xlsx");

            try
            {
                var resultsSummary = await _repository.GetRoatpSummary();
                using (var package = new ExcelPackage())
                {
                    var worksheetToAdd = package.Workbook.Worksheets.Add("RoATP");
                    worksheetToAdd.Cells.LoadFromDataTable(_dataTableHelper.ToDataTable(resultsSummary), true);
                    return File(package.GetAsByteArray(), "application/excel", $"roatp.xlsx");
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogInformation($"Could not generate data for roatp summary due to : {sqlEx.Message}");
                return NoContent();
            }
        }
    }
}