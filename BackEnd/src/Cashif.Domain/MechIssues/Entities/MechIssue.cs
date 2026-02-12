using Cashif.Domain.Common;
using Cashif.Domain.MechIssueTypes.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashif.Domain.MechIssues.Entities
{


    public class MechIssue : AggregateRoot
    {
        public string NameAr { get; private set; }
        public string NameEn { get; private set; }
        public Guid MechIssueTypeId { get; private set; }

        public virtual MechIssueType MechIssueType { get; private set; }
        private MechIssue() { }
        public MechIssue(string nameAr, string nameEn, Guid mechIssueTypeId)
        {
            MechIssueTypeId = mechIssueTypeId;
            NameAr = nameAr;
            NameEn = nameEn;
        }
        public void Update(string nameAr, string nameEn, Guid mechIssueTypeId)
        {
            MechIssueTypeId = mechIssueTypeId;
            NameAr = nameAr;
            NameEn = nameEn;
        }



    }
}
