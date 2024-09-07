using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.ViewModels
{
    public class NewsCategoryListItem
    {
        public int NewsCategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Slug { get; set; }
        public int NewsCount { get; set; }
    }
}
