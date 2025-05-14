using FinalProjectWinUi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectWinUi.Services
{
    public static class PricingData
    {
        public static readonly Dictionary<string, PlanPricing> AllPlanPrices = new()
    {
        { "Basic Plan", new PlanPricing { ContractPrice = 60000 } },
{ "Standard Plan", new PlanPricing { ContractPrice = 72000 } },
{ "Premium Plan", new PlanPricing { ContractPrice = 84000 } },
{ "Deluxe Plan", new PlanPricing { ContractPrice = 96000 } },

{ "Simple Cremation", new PlanPricing { ContractPrice = 54000 } },
{ "Memorial Cremation", new PlanPricing { ContractPrice = 65000 } },
{ "Premium Cremation", new PlanPricing { ContractPrice = 78000 } },

    };
    }

}
