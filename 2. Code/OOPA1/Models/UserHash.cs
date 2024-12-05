using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOPA1.Models
{
    /// <summary>
    /// Models a user with a hash
    /// </summary>
    public class UserHash
    {
        public string Username { get; set; }
        public string Hash { get; set; }

        public UserHash(string username, string hash)
        {
            this.Username = username;
            this.Hash = hash;
        }
    }
}
