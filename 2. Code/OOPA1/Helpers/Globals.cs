using OOPA1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOPA1.Helpers
{
    /// <summary>
    /// This class is used for variables between forms
    /// </summary>
    public static class Globals
    {
        public static string DatabaseName { get; set; } = "cardGame.db"; // the name of the database
        public static CurrentUserModel CurrentUser { get; set; } = new("", Enums.RoleTypes.Player); // holds the current user
        public static string? ToEditUsername { get; set; } // used to control which user is being edited
        public static int BetAmount { get; set; } = -1; // the amount that is being bet
    }
}
