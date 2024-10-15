using AeroMetrics.Application.Interfaces;
using AeroMetrics.Core.Request;
using AeroMetrics.Core.Response;
using Microsoft.AspNetCore.Mvc;

namespace AeroMetrics.Api.Service.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PerformanceController : ControllerBase
    {
        private readonly IPerformanceService _performanceService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="performanceService"></param>
        public PerformanceController(IPerformanceService performanceService)
        {
            _performanceService = performanceService;
        }

        /// <summary>
        /// Stores telemetry data from a base64 encoded file string.
        /// </summary>
        /// <param name="request">The request containing the base64 encoded string of the file.</param>
        /// <returns>Action result indicating success or failure.</returns>
        [HttpPost("StoreTelemetryData")]
        public async Task<ActionResult> StoreTelemetryData([FromBody] StoreTelemetryRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request cannot be null.");
            }

            if (string.IsNullOrEmpty(request.FileContent))
            {
                return BadRequest("Base64 file string is empty or not provided.");
            }

            try
            {
                byte[] fileBytes = Convert.FromBase64String(request.FileContent);
                using var stream = new MemoryStream(fileBytes);
                await _performanceService.StoreTelemetryDataAsync(stream);
                return new OkResult();
            }
            catch (FormatException)
            {
                return BadRequest("Invalid base64 string.");
            }
            catch (Exception ex)
            {
                // Log the exception details here if logging is implemented
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves telemetry data analysis results.
        /// </summary>
        /// <returns>List of telemetry data analysis results.</returns>
        [HttpGet("GetTelemetryData")]
        public async Task<ActionResult<List<AnalyzeTelemetryDataResult>>> GetTelemetryData()
        {
            try
            {
                var results = await _performanceService.GetTelemetryDataAsync();
                return new OkObjectResult(results);
            }
            catch (Exception ex)
            {
                // Log the exception details here if logging is implemented
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves the times for default conditions.
        /// </summary>
        /// <returns>The times when default conditions are met.</returns>
        [HttpGet("GetTimesForDefaultCondition")]
        public async Task<ActionResult<DefaultConditionResult>> GetTimesForDefaultCondition()
        {
            try
            {
                var result = await _performanceService.GetTimesForDefaultConditionAsync();
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                // Log the exception details here if logging is implemented
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes the telemetry data.
        /// </summary>
        /// <returns>Action result indicating success or failure.</returns>
        [HttpDelete("ClearTelemetryData")]
        public async Task<ActionResult> ClearTelemetryData()
        {
            try
            {
                await _performanceService.ClearTelemetryDataAsync();
                return new OkResult();
            }
            catch (Exception ex)
            {
                // Log the exception details here if logging is implemented
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
