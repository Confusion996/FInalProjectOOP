using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectWinUi.Models
{
    public class Payment
    {
        public int PlanID { get; set; }
        public int CustomerID { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Method { get; set; }
        public string ProofOfPayment { get; set; }
        public string Status { get; set; } = "Pending"; // default
    }
    public class PlanHolderData
    {
        public int PlanId { get; set; }
        public string Plantype { get; set; }
        public string PlanName { get; set; }
        public string PaymentStatus { get; set; } 
        public decimal ContractPrice { get; set; }
        public decimal Balance { get; set; }
        public DateTime DateApplied { get; set; }

    }

    public class Transaction
    {
        public int PaymentId { get; set; }
        public int PlanId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; }
        public DateTime Date { get; set; }
        public string ProofOfPayment { get; set; }
    }
}
