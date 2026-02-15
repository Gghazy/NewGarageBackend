using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Contracts.Roles
{
   public sealed record UpsertRoleRequest(string RoleName,List<string> Permissions);
}
