using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewSite.ViewModel.Security
{
    public class RoleAddEditModel
    {
        public int RoleID { get; set; }
        [Required(ErrorMessage ="*")]
        public string RoleName { get; set; }
        public string Description { get; set; }
    }
}
