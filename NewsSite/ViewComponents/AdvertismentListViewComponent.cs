using DataAccess.Services;
using DomainModel.ViewModels.Advertisment;
using DomainModel.ViewModels.News;
using Microsoft.AspNetCore.Mvc;

namespace NewsSite.ViewComponents
{
    [ViewComponent(Name = "AdvertismentList")]
    public class AdvertismentListViewComponent : ViewComponent
    {
        private readonly IAdvertiseVisitoryRepository _advertiseVisitoryRepository;
        public AdvertismentListViewComponent(IAdvertiseVisitoryRepository advertiseVisitoryRepository)
        {
                _advertiseVisitoryRepository = advertiseVisitoryRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync(AdvertismentSearchModel sm)
        {
            var advList = await _advertiseVisitoryRepository.Search(sm);
            return View(advList);
        }

    }
}
