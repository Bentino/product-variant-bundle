using ProductVariantBundle.Core.Enums;

namespace ProductVariantBundle.Api.DTOs.Common;

public abstract class BaseDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public EntityStatus Status { get; set; }
}