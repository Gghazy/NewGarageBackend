using Garage.Domain.Common.Primitives;


namespace Garage.Domain.Employees.Entities
{
    public class Employee:AggregateRoot
    {
        public Guid UserId { get; private set; }       
        public string NameAr { get; private set; }
        public string NameEn { get; private set; }
        public Guid BranchId { get; set; }

        private Employee() { }

        public Employee( Guid userId, string nameAr, string nameEn,Guid branchId)
        {
            UserId = userId;
            NameAr = nameAr;
            NameEn = nameEn;
            BranchId = branchId;
        }

        public void Update(string nameAr, string nameEn, Guid branchId)
        {
            NameAr = nameAr;
            NameEn = nameEn;
            BranchId = branchId;
        }
    }
}
