using DataAccess.Services;
using DomainModel.Migrations;
using Microsoft.AspNetCore.Mvc;

namespace NewsSite.ViewComponents
{
   [ViewComponent(Name = "SpecialNews")]
public class SpecialNewsViewComponent : ViewComponent
{
    private readonly INewsRepository _newsRepository;

    public SpecialNewsViewComponent(INewsRepository newsRepository)
    {
        _newsRepository = newsRepository;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var result = await _newsRepository.GetSpecialNewsAsync();
        return View("~/Views/Shared/Components/SpecialNews/Default.cshtml", result);
    }
}

}
