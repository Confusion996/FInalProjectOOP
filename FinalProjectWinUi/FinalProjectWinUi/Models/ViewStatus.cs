using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectWinUi.Models
{
    public class PlanApplication
    {
        public int PlanId { get; set; }
        public string PlanType { get; set; }
        public string PlanName { get; set; }
        public double ContractPrice { get; set; }
        public string ModeOfPayment { get; set; }
        public string Status { get; set; }
    }

    public class ClaimApplication
    {
        public int ClaimId { get; set; }
        public int PlanId { get; set; }
        public string ClaimantName { get; set; }
        public string RelationshipToPlanHolder { get; set; }
        public string ContactNumber { get; set; }
        public string EmailAddress { get; set; }
        public DateTime DateOfClaim { get; set; }
        public string ClaimStatus { get; set; }
    }

}
