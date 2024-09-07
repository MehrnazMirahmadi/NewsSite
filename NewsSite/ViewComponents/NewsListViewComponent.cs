using DataAccess.Services;
using DomainModel.ViewModels.News;
using Microsoft.AspNetCore.Mvc;

namespace NewsSite.ViewComponents
{

    [ViewComponent(Name = "NewsList")]
    public class NewsListViewComponent : ViewComponent
    {
        private readonly INewsRepository repo;
        public NewsListViewComponent(INewsRepository repo)
        {
            this.repo = repo;
        }
        public async Task<IViewComponentResult> InvokeAsync(NewsSearchModel sm)
        {
            var r = await repo.Search(sm);

            return View(r.NewsList);
        }
    }
}
