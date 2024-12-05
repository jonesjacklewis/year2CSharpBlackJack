using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOPA1.Models
{
    /// <summary>
    /// Models a User with a rank and their balance
    /// </summary>
    public class UserBalanceRank
    {
        public string Username { get; set; }
        public int BalanacePence { get; set; }
        public int Rank { get; set; }

        public UserBalanceRank(string username, int balancePence, int rank)
        {
            this.Username = username;
            this.BalanacePence = balancePence;
            this.Rank = rank;
        }

       
    }
}
