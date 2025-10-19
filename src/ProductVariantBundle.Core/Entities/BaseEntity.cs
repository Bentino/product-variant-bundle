using ProductVariantBundle.Core.Enums;

namespace ProductVariantBundle.Core.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public EntityStatus Status { get; set; }
}