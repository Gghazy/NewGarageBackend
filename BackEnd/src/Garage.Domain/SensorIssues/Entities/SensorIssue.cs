using Garage.Domain.Common.Primitives;


namespace Garage.Domain.SensorIssues.Entities
{
    public class SensorIssue : AggregateRoot
    {
        public string NameAr { get; private set; } = default!;
        public string NameEn { get; private set; } = default!;
        public string Code { get; private set; } = default!;
        private SensorIssue() { }
        public SensorIssue(string nameAr, string nameEn, string code)
        {
            NameAr = nameAr;
            NameEn = nameEn;
            Code = code;
        }
        public void Update(string ar, string en, string code)
        {
            NameAr = ar;
            NameEn = en;
            Code = code;

        }
    }
}

