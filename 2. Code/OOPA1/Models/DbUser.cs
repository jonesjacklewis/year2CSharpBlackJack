using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOPA1.Models
{
    /// <summary>
    /// Minimal information to insert a user into the database
    /// </summary>
    public class DbUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int RoleTypesId { get; set; }

        public DbUser(string username, string password, int roleTypesId)
        {
            this.Username = username;
            this.Password = password;
            this.RoleTypesId = roleTypesId;
        }
    }
}
