using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;

namespace NewsSite.ViewComponents
{
    [ViewComponent(Name ="LatestSpecialNews")]
    public class LatestSpecialNewsViewComponent : ViewComponent
    {
        private readonly INewsRepository _newsRepository;

        public LatestSpecialNewsViewComponent(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var latestSpecialNews = await _newsRepository.GetTwoLatestSpecialNews();
            return View("~/Views/Shared/Components/LatestSpecialNews/Default.cshtml", latestSpecialNews);
        }
    }
}
