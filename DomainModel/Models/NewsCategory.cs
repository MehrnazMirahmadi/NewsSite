using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Models
{
    public class NewsCategory
    {
        public int NewsCategoryID { get; set; }
        public string CategoryName { get; set; }
        public string SmallDescription { get; set; }
        public string Slug { get; set; }
        public int? ParentID { get; set; }
        public NewsCategory Parent { get; set; }
        public List<NewsCategory> Children { get; set; }
        public ICollection<News> News { get; set; }
        public NewsCategory()
        {
            Children = new List<NewsCategory>();
            News = new List<News>();
        }
    }
}
