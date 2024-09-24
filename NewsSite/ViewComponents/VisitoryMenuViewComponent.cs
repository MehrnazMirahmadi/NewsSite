using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace NewsSite.ViewComponents
{
    [ViewComponent(Name = "VisitoryMenu")]
    public class VisitoryMenuViewComponent : ViewComponent
    {
        private readonly IMenuVisitoryRepository repo;
        public VisitoryMenuViewComponent(IMenuVisitoryRepository repo)
        {
            this.repo = repo;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cats = await repo.GetAll();
            return View("~/Views/Shared/Components/VisitoryMenu/Default.cshtml", cats);
        }
    }
}
