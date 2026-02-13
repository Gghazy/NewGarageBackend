using Garage.Domain.Common.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Domain.Services.Enums
{


    public sealed class Stage : SmartEnum<Stage>
    {
        public string NameAr { get; }

        private Stage(int value, string name, string nameAr) : base(name,value)
        {
            NameAr = nameAr;

        }

        // ================== STAGES ==================

        public static readonly Stage Sensors =  new(1, "Sensors", "الحساسات");

        public static readonly Stage DashboardIndicators = new(2, "DashboardIndicators", "مؤشرات الطبلون");

        public static readonly Stage InteriorBody = new(3, "InteriorBody", "هيكل السيارة الداخلي");

        public static readonly Stage ExteriorBody = new(4, "ExteriorBody", "هيكل السيارة الخارجي");

        public static readonly Stage InteriorAndTrim = new(5, "InteriorAndTrim", "الداخلية والديكور");

        public static readonly Stage ExteriorAccessories = new(6, "ExteriorAccessories", "الإكسسوارات الخارجية");

        public static readonly Stage MechanicalIssues = new(7, "MechanicalIssues", "الأعطال الميكانيكية");

        public static readonly Stage Tires = new(8, "Tires", "الإطارات");

        public static readonly Stage RoadTest = new(9, "RoadTest", "التجربة الميدانية");

        public static readonly Stage Approval = new(10, "Approval", "الاعتماد");
    }

}
