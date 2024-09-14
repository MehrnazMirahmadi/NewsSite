using DataAccess.Services;
using DomainModel.ViewModels.News;
using Microsoft.AspNetCore.Mvc;

namespace NewsSite.ViewComponents
{

    [ViewComponent(Name = "NewsList")]
    public class NewstViewComponent : ViewComponent
    {
        private readonly INewsRepository repo;
        public NewstViewComponent(INewsRepository repo)
        {
            this.repo = repo;
        }
        public async Task<IViewComponentResult> InvokeAsync(NewsSearchModel sm)
        {
            var news = await repo.Search(sm);
            return View(news);
        }
    }
}
