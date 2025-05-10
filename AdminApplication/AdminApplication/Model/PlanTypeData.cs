using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminApplication.Model
{
    public class MonthlySalesData
    {
        public string MonthLabel { get; set; } // e.g. "Jan", "Feb"
        public decimal Amount { get; set; }
    }

    public class TopPlanData
    {
        public string PlanType { get; set; }
        public int Count { get; set; }
    }

    public class ClaimTypeData
    {
        public string Relationship { get; set; } // e.g. "Spouse", "Child"
        public int Count { get; set; }
    }

}
