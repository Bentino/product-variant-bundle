namespace ProductVariantBundle.Core.Exceptions;

public abstract class BusinessException : Exception
{
    public string ErrorCode { get; }
    
    protected BusinessException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }
    
    protected BusinessException(string errorCode, string message, Exception innerException) : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}



public class EntityNotFoundException : BusinessException
{
    public EntityNotFoundException(string entityType, object id) 
        : base("ENTITY_NOT_FOUND", $"{entityType} with id {id} not found") { }
}

public class DuplicateEntityException : BusinessException
{
    public DuplicateEntityException(string entityType, string field, object value) 
        : base("DUPLICATE_ENTITY", $"{entityType} with {field} '{value}' already exists") { }
}