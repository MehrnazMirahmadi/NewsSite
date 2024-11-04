using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security
{
    public class ApplicationRole:IdentityRole<int>
    {
        public string Description { get; set; }
        public ICollection<ApplicationUserRole> ApplicationUserRoles { get; set; }
        public ApplicationRole()
        {
            this.ApplicationUserRoles = new List<ApplicationUserRole>();
        }
    }
}
