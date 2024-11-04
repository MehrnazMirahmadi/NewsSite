using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
      
        public ICollection<ApplicationUserRole> ApplicationUserRoles { get; set; }
        public ApplicationUser()
        {
            this.ApplicationUserRoles = new List<ApplicationUserRole>();
        }
    }
}
