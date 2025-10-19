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
        context.Response.ContentType = "application/problem+json";

        var problemDetails = exception switch
        {
            ValidationException validationEx => ProblemDetailsResponse.Create(
                status: (int)HttpStatusCode.BadRequest,
                title: "Validation Error",
                detail: "One or more validation errors occurred.",
                instance: context.Request.Path,
                errors: validationEx.Errors.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)),

            DuplicateEntityException duplicateEx => ProblemDetailsResponse.Create(
                status: (int)HttpStatusCode.Conflict,
                title: "Duplicate Entity",
                detail: duplicateEx.Message,
                instance: context.Request.Path),

            EntityNotFoundException notFoundEx => ProblemDetailsResponse.Create(
                status: (int)HttpStatusCode.NotFound,
                title: "Entity Not Found",
                detail: notFoundEx.Message,
                instance: context.Request.Path),

            BusinessException businessEx => ProblemDetailsResponse.Create(
                status: (int)HttpStatusCode.BadRequest,
                title: "Business Rule Violation",
                detail: businessEx.Message,
                instance: context.Request.Path),

            ArgumentException argEx => ProblemDetailsResponse.Create(
                status: (int)HttpStatusCode.BadRequest,
                title: "Invalid Argument",
                detail: argEx.Message,
                instance: context.Request.Path),

            UnauthorizedAccessException => ProblemDetailsResponse.Create(
                status: (int)HttpStatusCode.Unauthorized,
                title: "Unauthorized",
                detail: "Authentication is required to access this resource.",
                instance: context.Request.Path),

            _ => ProblemDetailsResponse.Create(
                status: (int)HttpStatusCode.InternalServerError,
                title: "Internal Server Error",
                detail: "An unexpected error occurred while processing your request.",
                instance: context.Request.Path)
        };

        context.Response.StatusCode = problemDetails.Status;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(problemDetails, options);
        await context.Response.WriteAsync(json);
    }
}