namespace ProductVariantBundle.Core.Exceptions;

public class ValidationException : Exception
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(Dictionary<string, string[]> errors) 
        : base("Validation failed")
    {
        Errors = errors ?? new Dictionary<string, string[]>();
    }

    public ValidationException(string field, string error) 
        : base("Validation failed")
    {
        Errors = new Dictionary<string, string[]>
        {
            { field, new[] { error } }
        };
    }

    public ValidationException(string message, Dictionary<string, string[]> errors) 
        : base(message)
    {
        Errors = errors ?? new Dictionary<string, string[]>();
    }

    public override string Message
    {
        get
        {
            if (!Errors.Any())
                return base.Message;

            var errorMessages = Errors
                .SelectMany(kvp => kvp.Value.Select(error => $"{kvp.Key}: {error}"))
                .ToList();

            return string.Join("; ", errorMessages);
        }
    }
}