using Garage.Domain.Common.Exceptions;
using Garage.Domain.Common.Primitives;
using Garage.Domain.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Domain.Services.Entities
{
    public class Service : AggregateRoot
    {
        public string NameAr { get; private set; } = null!;
        public string NameEn { get; private set; } = null!;

        private readonly List<ServicesStage> _stages = new();
        public IReadOnlyCollection<ServicesStage> Stages => _stages.AsReadOnly();

        private readonly List<ServicePrice> _prices = new();
        public IReadOnlyCollection<ServicePrice> Prices => _prices;

        private Service() { }

        public Service(string nameAr, string nameEn)
        {
            NameAr = nameAr;
            NameEn = nameEn;
        }
        public void SetNames(string nameAr, string nameEn)
        {
            NameAr = nameAr;
            NameEn = nameEn;
        }
        public void SetStages(IEnumerable<int> stages)
        {
            _stages.Clear();
            _stages.AddRange(stages.Select(x => new ServicesStage(Id, x)));
        }

        public void UpsertPrice(Guid markId, int fromYear, int toYear, decimal price)
        {
            ValidatePriceRow(markId, fromYear, toYear, price);
            var existing = _prices.FirstOrDefault(x =>
                x.MarkId == markId &&
                x.FromYear == fromYear &&
                x.ToYear == toYear);

            EnsureNoOverlappingRanges(markId, fromYear, toYear, ignore: existing);

            if (existing is null)
            {
                _prices.Add(new ServicePrice(Id, markId, fromYear, toYear, price));
                return;
            }

            existing.UpdatePrice(price);
        }

        public void RemovePrice(Guid markId, int fromYear, int toYear)
        {
            var existing = _prices.FirstOrDefault(x =>
                x.MarkId == markId &&
                x.FromYear == fromYear &&
                x.ToYear == toYear);

            if (existing is null) return;

            _prices.Remove(existing);
        }

        public void ClearPrices() => _prices.Clear();


        private static void ValidatePriceRow(Guid brandId, int fromYear, int toYear, decimal price)
        {
            if (brandId == Guid.Empty)
                throw new DomainException("BrandId is required");

            if (fromYear < 1950 || fromYear > 2100)
                throw new DomainException("Invalid FromYear");

            if (toYear < 1950 || toYear > 2100)
                throw new DomainException("Invalid ToYear");

            if (fromYear > toYear)
                throw new DomainException("FromYear cannot be greater than ToYear");

            if (price < 0)
                throw new DomainException("Price must be >= 0");
        }
        private void EnsureNoOverlappingRanges(Guid markId, int fromYear, int toYear, ServicePrice? ignore)
        {
            var hasOverlap = _prices
                .Where(x => x.MarkId == markId && x != ignore)
                .Any(x => fromYear <= x.ToYear && toYear >= x.FromYear);

            if (hasOverlap)
                throw new DomainException("لا يمكن إضافة سعر لنفس الماركة لأن نطاق السنين يتداخل مع نطاق موجود.");
        }

    }
}
