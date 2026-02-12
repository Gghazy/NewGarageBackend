using Cashif.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashif.Domain.AccessoryIssues.Entity
{
    public class AccessoryIssue : LookupBase
    {
        public AccessoryIssue() { }
        public AccessoryIssue(string name, string description) : base(name, description){}
    }
}
