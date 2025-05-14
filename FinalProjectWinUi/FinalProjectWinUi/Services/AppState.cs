using FinalProjectWinUi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinalProjectWinUi.Services
{
    public static class AppState
    {
        public static bool isUserLoggedIn { get; set; } = false;
        public static CustomerAccount LoggedInUser { get; set; } = null;
    }

    public static class Connection
    {
        public static readonly string Conn=
            $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\A.H\source\repos\FinalProjectWInUI\FinalProjectWinUi\Database\CustomerDatabase.accdb";
    }

}
