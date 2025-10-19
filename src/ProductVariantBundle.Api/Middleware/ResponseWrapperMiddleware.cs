using System.Text.Json;
using ProductVariantBundle.Api.DTOs.Common;

namespace ProductVariantBundle.Api.Middleware;

public class ResponseWrapperMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ResponseWrapperMiddleware> _logger;

    public ResponseWrapperMiddleware(RequestDelegate next, ILogger<ResponseWrapperMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine($"=== ResponseWrapperMiddleware: {context.Request.Method} {context.Request.Path} ===");
        
        // Skip wrapping for certain paths
        if (ShouldSkipWrapping(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        // Check content type after response is generated
        var contentType = context.Response.ContentType?.ToLowerInvariant() ?? string.Empty;
        if (contentType.Contains("text/html") || 
            contentType.Contains("text/css") || 
            contentType.Contains("application/javascript") ||
            contentType.Contains("image/"))
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await context.Response.Body.CopyToAsync(originalBodyStream);
            return;
        }

        // Only wrap successful responses (2xx status codes)
        // For error responses, let GlobalExceptionMiddleware handle them completely
        if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
        {
            await WrapSuccessResponse(context, originalBodyStream);
        }
        else
        {
            // For error responses, just pass through - GlobalExceptionMiddleware already handled them
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await context.Response.Body.CopyToAsync(originalBodyStream);
        }
    }

    private static bool ShouldSkipWrapping(PathString path)
    {
        // Skip wrapping for health checks, swagger, and other infrastructure endpoints
        var pathValue = path.Value?.ToLowerInvariant() ?? string.Empty;
        return pathValue == "/" ||
               pathValue == "/index.html" ||
               pathValue.StartsWith("/health") ||
               pathValue.StartsWith("/swagger") ||
               pathValue.StartsWith("/api-docs") ||
               pathValue.Contains("favicon") ||
               pathValue.Contains(".css") ||
               pathValue.Contains(".js") ||
               pathValue.Contains(".png") ||
               pathValue.Contains(".ico") ||
               pathValue.Contains(".html");
    }

    private static async Task WrapSuccessResponse(HttpContext context, Stream originalBodyStream)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();

        // Skip wrapping for HTML content (like Swagger UI)
        var contentType = context.Response.ContentType?.ToLowerInvariant() ?? string.Empty;
        if (contentType.Contains("text/html") || 
            contentType.Contains("text/css") || 
            contentType.Contains("application/javascript") ||
            contentType.Contains("image/") ||
            responseText.TrimStart().StartsWith("<!DOCTYPE") ||
            responseText.TrimStart().StartsWith("<html"))
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await context.Response.Body.CopyToAsync(originalBodyStream);
            return;
        }

        // If the response is already wrapped (contains "data", "meta", "errors"), don't wrap again
        if (IsAlreadyWrapped(responseText))
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await context.Response.Body.CopyToAsync(originalBodyStream);
            return;
        }

        object? responseData = null;
        if (!string.IsNullOrEmpty(responseText))
        {
            try
            {
                // Try to parse as JsonElement to preserve structure
                using var document = JsonDocument.Parse(responseText);
                responseData = document.RootElement.Clone();
            }
            catch
            {
                responseData = responseText;
            }
        }

        var wrappedResponse = ApiResponse<object>.Success(responseData);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var wrappedJson = JsonSerializer.Serialize(wrappedResponse, options);
        var wrappedBytes = System.Text.Encoding.UTF8.GetBytes(wrappedJson);

        context.Response.ContentLength = wrappedBytes.Length;
        context.Response.ContentType = "application/json";

        context.Response.Body = originalBodyStream;
        await context.Response.Body.WriteAsync(wrappedBytes);
    }

    private static bool IsAlreadyWrapped(string responseText)
    {
        if (string.IsNullOrEmpty(responseText))
            return false;

        try
        {
            using var document = JsonDocument.Parse(responseText);
            var root = document.RootElement;
            
            return root.TryGetProperty("data", out _) ||
                   root.TryGetProperty("meta", out _) ||
                   root.TryGetProperty("errors", out _);
        }
        catch
        {
            return false;
        }
    }
}