using Garage.Domain.Common.Primitives;
using Garage.Domain.Services.Enums;

namespace Garage.Domain.Services.Entities
{
    public class ServicesStage: Entity
    {
        public Guid ServiceId { get; private set; }
        public Service Service { get; private set; } = null!;

        public int StageValue { get; private set; }
        public Stage Stage => Stage.FromValue(StageValue);


        private ServicesStage() { }

        public ServicesStage(Guid serviceId, int stageValue)
        {
            ServiceId = serviceId;
            StageValue = stageValue;
        }
    }
}
