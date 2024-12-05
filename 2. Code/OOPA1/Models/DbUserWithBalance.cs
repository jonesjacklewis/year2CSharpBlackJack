using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOPA1.Models
{
    /// <summary>
    /// Inherits from DbUser, and has an additonal property for a custom balance
    /// </summary>
    public class DbUserWithBalanace: DbUser
    {
        public int BalanacePence { get; set; }

        public DbUserWithBalanace(string username, string password, int roleTypesId, int balanacePence): base(username, password, roleTypesId)
        {
            this.BalanacePence = balanacePence;
        }
    }
}
