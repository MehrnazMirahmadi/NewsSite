using DomainModel.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.ViewModels
{
    public class NewsCategoryAddEditModel
    {
        public int NewsCategoryID { get; set; }
       // [Required(ErrorMessage = "نام رده را وارد کنید")]
        [Display(Name = "نام رده")]
        //[EmailAddress(ErrorMessage ="آدرس ایمیل را به درستی وارد نمایید")]        
        public string CategoryName { get; set; }
        public string SmallDescription { get; set; }
        public string Slug { get; set; }
        public int? ParentID { get; set; }
        public NewsCategory? Parent { get; set; }
    }
}
