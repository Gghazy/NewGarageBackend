using Garage.Domain.Common.Primitives;
using Garage.Domain.Manufacturers.Entity;

namespace Garage.Domain.CarMarkes.Entity;

public class CarMark : LookupBase
{
    public Guid ManufacturerId { get; private set; }
    public Manufacturer Manufacturer { get; private set; } = null!;

    private CarMark() { }

    public CarMark(string nameAr, string nameEn, Guid manufacturerId)
        : base(nameAr, nameEn)
    {
        if (manufacturerId == Guid.Empty)
            throw new ArgumentException("ManufacturerId is required.", nameof(manufacturerId));
        ManufacturerId = manufacturerId;
    }

    public void Update(string nameAr, string nameEn, Guid manufacturerId)
    {
        if (manufacturerId == Guid.Empty)
            throw new ArgumentException("ManufacturerId is required.", nameof(manufacturerId));
        base.Update(nameAr, nameEn);
        ManufacturerId = manufacturerId;
    }
}
