using DomainModel.Comon;


namespace DomainModel.ViewModels.News
{
    public class NewsSearchModel : PageModel
    {
        public int? NewsCategoryID { get; set; }
        public string NewsTitle { get; set; }
        public string NewsText { get; set; }
        public string Slug { get; set; }
    }
}
