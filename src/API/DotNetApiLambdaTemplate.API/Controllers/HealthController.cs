using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DotNetApiLambdaTemplate.API.Controllers;

/// <summary>
/// Controller for health checks
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;
    private readonly ILogger<HealthController> _logger;

    public HealthController(HealthCheckService healthCheckService, ILogger<HealthController> logger)
    {
        _healthCheckService = healthCheckService ?? throw new ArgumentNullException(nameof(healthCheckService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Performs a comprehensive health check
    /// </summary>
    /// <returns>Health status</returns>
    [HttpGet]
    [ProducesResponseType(typeof(HealthReport), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HealthReport), StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<HealthReport>> GetHealth()
    {
        _logger.LogInformation("Performing health check");

        var healthReport = await _healthCheckService.CheckHealthAsync();

        var statusCode = healthReport.Status == HealthStatus.Healthy ?
            StatusCodes.Status200OK :
            StatusCodes.Status503ServiceUnavailable;

        _logger.LogInformation("Health check completed with status: {Status}", healthReport.Status);

        return StatusCode(statusCode, healthReport);
    }

    /// <summary>
    /// Performs a liveness check
    /// </summary>
    /// <returns>Liveness status</returns>
    [HttpGet("live")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public IActionResult GetLiveness()
    {
        _logger.LogInformation("Performing liveness check");

        // Simple liveness check - if we can respond, we're alive
        return Ok(new { status = "alive", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Performs a readiness check
    /// </summary>
    /// <returns>Readiness status</returns>
    [HttpGet("ready")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetReadiness()
    {
        _logger.LogInformation("Performing readiness check");

        var healthReport = await _healthCheckService.CheckHealthAsync();

        if (healthReport.Status == HealthStatus.Healthy)
        {
            _logger.LogInformation("Readiness check passed");
            return Ok(new { status = "ready", timestamp = DateTime.UtcNow });
        }

        _logger.LogWarning("Readiness check failed with status: {Status}", healthReport.Status);
        return StatusCode(StatusCodes.Status503ServiceUnavailable, new
        {
            status = "not ready",
            timestamp = DateTime.UtcNow,
            details = healthReport.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                exception = e.Value.Exception?.Message
            })
        });
    }
}
