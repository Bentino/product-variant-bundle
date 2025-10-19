using System.Net;
using System.Text.Json;
using ProductVariantBundle.Api.DTOs.Common;
using ProductVariantBundle.Core.Exceptions;

namespace ProductVariantBundle.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Clear any existing response
        context.Response.Clear();
        context.Response.ContentType = "application/json";

        // Create consistent API response format instead of ProblemDetails
        var (statusCode, errorMessages) = exception switch
        {
            ValidationException validationEx => ((int)HttpStatusCode.BadRequest, 
                validationEx.Errors.SelectMany(kvp => kvp.Value.Select(error => $"{kvp.Key}: {error}")).ToArray()),

            DuplicateEntityException duplicateEx => ((int)HttpStatusCode.Conflict, 
                new[] { duplicateEx.Message }),

            EntityNotFoundException notFoundEx => ((int)HttpStatusCode.NotFound, 
                new[] { notFoundEx.Message }),

            BusinessException businessEx => ((int)HttpStatusCode.BadRequest, 
                new[] { businessEx.Message }),

            ArgumentException argEx => ((int)HttpStatusCode.BadRequest, 
                new[] { argEx.Message }),

            UnauthorizedAccessException => ((int)HttpStatusCode.Unauthorized, 
                new[] { "Authentication is required to access this resource." }),

            _ => ((int)HttpStatusCode.InternalServerError, 
                new[] { "An unexpected error occurred while processing your request." })
        };

        context.Response.StatusCode = statusCode;

        // Use the same ApiResponse format as successful responses
        var apiResponse = ApiResponse<object>.Error(errorMessages);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(apiResponse, options);
        
        // Write directly to response without setting ContentLength (use chunked encoding)
        await context.Response.WriteAsync(json);
    }
}