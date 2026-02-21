using Garage.Domain.Common.Exceptions;
using Garage.Domain.Common.Primitives;
using Garage.Domain.ServicePrices.Entities;
using Garage.Domain.Services.Enums;

namespace Garage.Domain.Services.Entities
{
    public class Service : AggregateRoot
    {
        public string NameAr { get; private set; } = null!;
        public string NameEn { get; private set; } = null!;

        private readonly List<ServicesStage> _stages = new();
        public IReadOnlyCollection<ServicesStage> Stages => _stages.AsReadOnly();

        private readonly List<ServicePrice> _prices = new();
        public IReadOnlyCollection<ServicePrice> Prices => _prices.AsReadOnly();

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
            if (stages is null)
                throw new DomainException("Stages are required");

            var desired = stages.Distinct().ToList();
            if (desired.Count == 0)
                throw new DomainException("Stages are required");

            // remove stages that are not desired
            var toRemove = _stages.Where(x => !desired.Contains(x.Stage.Value)).ToList();
            foreach (var r in toRemove)
                _stages.Remove(r);

            // add missing stages
            var existing = _stages.Select(x => x.Stage.Value).ToHashSet();
            foreach (var s in desired)
            {
                // لو عندك validation Stage.FromValue(s) اعمله قبلها في handler زي ما بتعمل
                if (!existing.Contains(s))
                    _stages.Add(new ServicesStage(Id, s));
            }
        }


    }
}
