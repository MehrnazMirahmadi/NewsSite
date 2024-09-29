using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.ViewModels.Advertisment
{
    public class AdvertismentSearchModel
    {
        public int AdvertisementID { get; set; }
        public string Title { get; set; }
        public bool IsDefault { get; set; }
    }
}
