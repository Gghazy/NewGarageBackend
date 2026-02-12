using Cashif.Domain.Common;
using Cashif.Domain.MechIssues.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashif.Domain.MechIssueTypes.Entity
{
    public class MechIssueType:LookupBase
    {
        private readonly List<MechIssue> _mechIssues = new();

        public MechIssueType() { }
        public MechIssueType(string nameAr, string nameEn) : base(nameAr, nameEn) { }

        public IReadOnlyCollection<MechIssue> MechIssues => _mechIssues;

    }
}
