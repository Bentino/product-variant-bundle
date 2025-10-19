using System.Text.RegularExpressions;

namespace ProductVariantBundle.Core.Helpers;

public static class SlugHelper
{
    public static string Normalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Convert to lowercase and trim
        var normalized = input.Trim().ToLowerInvariant();
        
        // Replace spaces and underscores with hyphens
        normalized = normalized.Replace(" ", "-").Replace("_", "-");
        
        // Remove multiple consecutive hyphens
        normalized = Regex.Replace(normalized, "-+", "-");
        
        // Remove leading and trailing hyphens
        normalized = normalized.Trim('-');
        
        return normalized;
    }
}