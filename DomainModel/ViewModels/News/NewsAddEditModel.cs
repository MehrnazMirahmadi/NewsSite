using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.ViewModels.News
{
    public class NewsAddEditModel
    {
        public int NewsID { get; set; }
        [Required(ErrorMessage = "عنوان خبر را وارد کنید")]
        [MaxLength(150, ErrorMessage = "عنوان خبر حداکثر صد و پنجاه")]
        public string NewsTitle { get; set; }
        public string SmallDescription { get; set; }
        public string Slug { get; set; }
        public string? ImageUrl { get; set; }
        public string NewsText { get; set; }
        public DateTime RegistrationDate { get; set; }

        public int SortOrder { get; set; }
        public int VisitCount { get; set; }
        public int VoteSumation { get; set; }
        public int VoteCount { get; set; }
        public int NewsCategoryID { get; set; }
    }
}
