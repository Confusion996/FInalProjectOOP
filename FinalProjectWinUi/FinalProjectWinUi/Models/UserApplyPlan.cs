using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectWinUi.Models
{
    public class UserApplyPlan
        {
        // Personal Info
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }
        public int Age { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string Sex { get; set; }

        // Civil Status Info
        public string Nationality { get; set; }
        public string CivilStatus { get; set; }
        public string Occupation { get; set; }

        // Address
        public string Lot { get; set; }
        public string Street { get; set; }
        public string Barangay { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string ZipCode { get; set; }

        // Plan Data
        public int PlanId { get; set; }
        public string PlanType { get; set; }
        public string PlanName { get; set; }
        public string SelectedPlan { get; set; }
        public string ModeOfPayment { get; set; }
        public string PaymentMethod { get; set; }
        public string Insurance { get; set; }
        public decimal ContractPrice { get; set; }
        public decimal TotalAmountPayable { get; set; }
        public decimal InstallmentDue { get; set; }
        public decimal Balance { get; set; }     
        public string Status { get; set; }
        public string PaymentStatus { get; set; } 
        public string ClaimStatus { get; set; }
        public DateTime DateApplied { get; set; }
    }
}
