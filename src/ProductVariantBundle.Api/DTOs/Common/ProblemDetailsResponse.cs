using System.Text.Json.Serialization;

namespace ProductVariantBundle.Api.DTOs.Common;

/// <summary>
/// RFC 7807 Problem Details for HTTP APIs
/// </summary>
public class ProblemDetailsResponse
{
    /// <summary>
    /// A URI reference that identifies the problem type
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = "about:blank";

    /// <summary>
    /// A short, human-readable summary of the problem type
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The HTTP status code
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>
    /// A human-readable explanation specific to this occurrence of the problem
    /// </summary>
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }

    /// <summary>
    /// A URI reference that identifies the specific occurrence of the problem
    /// </summary>
    [JsonPropertyName("instance")]
    public string? Instance { get; set; }

    /// <summary>
    /// Additional validation errors or problem-specific extensions
    /// </summary>
    [JsonPropertyName("errors")]
    public Dictionary<string, string[]>? Errors { get; set; }

    /// <summary>
    /// Additional problem-specific extensions
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? Extensions { get; set; }

    public static ProblemDetailsResponse Create(
        int status,
        string title,
        string? detail = null,
        string? instance = null,
        string? type = null,
        Dictionary<string, string[]>? errors = null)
    {
        return new ProblemDetailsResponse
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = instance,
            Type = type ?? GetDefaultTypeForStatus(status),
            Errors = errors
        };
    }

    private static string GetDefaultTypeForStatus(int status)
    {
        return status switch
        {
            400 => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            401 => "https://tools.ietf.org/html/rfc7235#section-3.1",
            403 => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            404 => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            409 => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            422 => "https://tools.ietf.org/html/rfc4918#section-11.2",
            500 => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            _ => "about:blank"
        };
    }
}