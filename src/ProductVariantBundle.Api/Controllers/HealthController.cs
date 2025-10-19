using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductVariantBundle.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class HealthController : ControllerBase
{
    /// <summary>
    /// Get API health status
    /// </summary>
    /// <returns>Health status information</returns>
    /// <response code="200">API is healthy</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Check API health",
        Description = "Returns the current health status of the API including version and environment information.",
        OperationId = "GetHealth",
        Tags = new[] { "Health" }
    )]
    [SwaggerResponse(200, "Healthy", typeof(object))]
    public IActionResult Get()
    {
        return Ok(new 
        { 
            status = "healthy", 
            timestamp = DateTime.UtcNow,
            version = "1.0.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
        });
    }

    [HttpGet("database")]
    public IActionResult Database()
    {
        // This will be implemented when DbContext is fully configured
        return Ok(new 
        { 
            status = "database check not implemented yet", 
            timestamp = DateTime.UtcNow 
        });
    }
}