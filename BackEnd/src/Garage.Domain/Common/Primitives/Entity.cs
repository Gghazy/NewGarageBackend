namespace Garage.Domain.Common.Primitives;

public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; private set; }

    public Guid? CreatedBy { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    // Soft Delete Properties
    public bool IsDeleted { get; private set; } = false;
    public DateTime? DeletedAtUtc { get; private set; }
    public Guid? DeletedBy { get; private set; }

    public void SetCreatedBy(Guid userId)
    {
        CreatedBy = userId;
    }

    public void SetUpdatedBy(Guid userId)
    {
        UpdatedBy = userId;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Soft delete: marks entity as deleted without removing from database
    /// </summary>
    public void SoftDelete(Guid? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    /// <summary>
    /// Restore a soft-deleted entity
    /// </summary>
    public void Restore()
    {
        IsDeleted = false;
        DeletedAtUtc = null;
        DeletedBy = null;
    }

}
