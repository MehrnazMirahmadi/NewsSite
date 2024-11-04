using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security
{
    public class ApplicationUserRole
    {
     
        public int ApplicationUserID { get; set; }
        public int ApplicationRoleID { get; set; }
        public ApplicationRole ApplicationRole { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
