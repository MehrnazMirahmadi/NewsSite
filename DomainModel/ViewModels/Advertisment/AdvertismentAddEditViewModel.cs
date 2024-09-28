using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DomainModel.ViewModels.Advertisment
{
    public class AdvertismentAddEditViewModel
    {
        public int AdvertisementID { get; set; }
        public string Title { get; set; }
        public string Alt { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? Picture { get; set; }
        public string Url { get; set; }
        public bool IsDefault { get; set; }
    }
}
