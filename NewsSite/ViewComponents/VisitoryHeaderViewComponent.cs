using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;

namespace NewsSite.ViewComponents
{
    [ViewComponent(Name = "VisitoryHeader")]
    public class VisitoryHeaderViewComponent : ViewComponent
    {

        private readonly IAdvertiseVisitoryRepository repo;
        public VisitoryHeaderViewComponent(IAdvertiseVisitoryRepository repo)
        {
            this.repo = repo;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var adv = await repo.GetActiveAdvertise();
            return View("~/Views/Shared/Components/VisitoryHeader/Default.cshtml", adv);
        }
    }
}
