using Garage.Domain.Common.Primitives;

namespace Garage.Domain.CarMarkes.Entity
{
    public class CarMark: LookupBase
    {
        public CarMark() { }
        public CarMark(string nameAr, string nameEn) : base(nameAr, nameEn) { }
    }
}
