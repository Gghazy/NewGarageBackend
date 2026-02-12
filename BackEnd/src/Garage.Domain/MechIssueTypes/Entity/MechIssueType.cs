using Garage.Domain.Common;
using Garage.Domain.MechIssues.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Domain.MechIssueTypes.Entity
{
    public class MechIssueType:LookupBase
    {
        private readonly List<MechIssue> _mechIssues = new();

        public MechIssueType() { }
        public MechIssueType(string nameAr, string nameEn) : base(nameAr, nameEn) { }

        public IReadOnlyCollection<MechIssue> MechIssues => _mechIssues;

    }
}

