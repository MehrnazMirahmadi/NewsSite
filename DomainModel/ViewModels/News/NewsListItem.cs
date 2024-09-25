using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.ViewModels.News
{
    public class NewsListItem
    {
        public int NewsID { get; set; }
        public string NewsTitle { get; set; }

        public string Slug { get; set; }
        public string ImageUrl { get; set; }
        public int SortOrder { get; set; }
        public string CategoryName { get; set; }
       // public bool IsSpecial { get; set; }

    }
}
