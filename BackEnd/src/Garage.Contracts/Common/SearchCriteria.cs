using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Contracts.Common
{
    public class SearchCriteria
    {
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public string? TextSearch { get; set; }
        public string? Sort { get; set; }
        public bool Desc { get; set; }
    }
}

