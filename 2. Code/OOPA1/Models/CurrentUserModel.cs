using OOPA1.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOPA1.Models
{
    /// <summary>
    /// Class represent the current user, minimal model of just username and the type of role
    /// </summary>
    public class CurrentUserModel
    {
        public string Username { get; set; }
        public RoleTypes RoleType { get; set; }


        public CurrentUserModel(string username, RoleTypes roleType)
        {
            this.Username = username;
            this.RoleType = roleType;
        }
    }
}
