using Garage.Domain.Common;
namespace Garage.Domain.Branches.Entities;
public class Branch : AggregateRoot
{
    public string NameAr { get; private set; } = default!;
    public string NameEn { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    private Branch() { }
    public Branch(string nameAr, string nameEn, bool isActive = true)
    {
        NameAr = nameAr;
        NameEn = nameEn;
        IsActive = isActive;
    }
    public void Update(string ar, string en)
    {
        NameAr = ar;
        NameEn = en;
    }
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}

