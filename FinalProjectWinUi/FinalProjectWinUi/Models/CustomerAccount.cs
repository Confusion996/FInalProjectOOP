using FinalProjectWinUi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectWinUi.Models
{
    public class CustomerAccount
    {
        public int CustomerId { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public CustomerAccount(string firstName, string lastName,string email, string password)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
        }

        public CustomerAccount()
        {}
    }
}

