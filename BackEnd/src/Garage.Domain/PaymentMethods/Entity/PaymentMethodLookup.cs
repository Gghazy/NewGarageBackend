using Garage.Domain.Common.Primitives;

namespace Garage.Domain.PaymentMethods.Entity
{
    public class PaymentMethodLookup : LookupBase
    {
        public PaymentMethodLookup() { }
        public PaymentMethodLookup(string nameAr, string nameEn) : base(nameAr, nameEn) { }
    }
}
