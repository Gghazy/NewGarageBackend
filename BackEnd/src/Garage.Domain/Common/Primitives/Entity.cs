namespace Garage.Domain.Common.Primitives;

public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; private set; }

    public Guid? CreatedBy { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    public void SetCreatedBy(Guid userId)
    {
        CreatedBy = userId;
    }

    public void SetUpdatedBy(Guid userId)
    {
        UpdatedBy = userId;
        UpdatedAtUtc = DateTime.UtcNow;
    }

}
