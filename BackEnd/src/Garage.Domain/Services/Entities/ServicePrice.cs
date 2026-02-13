using Garage.Domain.Common.Exceptions;
using Garage.Domain.Common.Primitives;



namespace Garage.Domain.Services.Entities
{
    public class ServicePrice: Entity
    {
        public Guid ServiceId { get; private set; }
        public Guid MarkId { get; private set; }

        public int FromYear { get; private set; }
        public int ToYear { get; private set; }

        public decimal Price { get; private set; }

        private ServicePrice() { }

        public ServicePrice(Guid serviceId, Guid markId, int fromYear, int toYear, decimal price)
        {
            if (fromYear > toYear)
                throw new DomainException("Invalid year range");

            ServiceId = serviceId;
            MarkId = markId;
            FromYear = fromYear;
            ToYear = toYear;
            Price = price;
        }

        public void UpdatePrice(decimal price)
        {
            if (price < 0) throw new DomainException("Invalid price");
            Price = price;
        }
    }
}
