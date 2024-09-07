using DataAccess.Services;
using DomainModel.Comon;
using DomainModel.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace NewsSite.Controllers
{
    public class NewsCategoryController : Controller
    {
        private readonly INewsCategoryRepository _repo;
        public NewsCategoryController(INewsCategoryRepository repo)
        {
            _repo = repo;
        }
      
        private async Task BindRoots()
        {
            var cats = await _repo.GetRoots();
            cats.Insert(0, new NewsCategoryListItem { CategoryName = "دسته اصلی" });
            ViewBag.drpRoots = new SelectList(cats, "NewsCategoryID", "CategoryName");
        }

        private async Task BindChildren(int ParentID)
        {
            var cats = await _repo.GetSubCategory(ParentID);
            ViewBag.drpChildren = new SelectList(cats, "NewsCategoryID", "CategoryName");
        }
      
        public async Task<IActionResult> Index()
        {

            return View();
        }

        public async Task<IActionResult> List()
        {

            var cats = await _repo.GetAll();
            return View(cats);
        }

        public async Task<IActionResult> Add()
        {

            await BindRoots();
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Add(NewsCategoryAddEditModel cat)
        {
            if (ModelState.IsValid)
            {
                if (cat.ParentID == 0)
                {
                    cat.ParentID = null;
                }
                var op = await _repo.Add(cat);
                return Json(op);
            }
            else
            {
                // Extract errors from the ModelState and return them
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new OperationResult().ToFailed("Error in sending data"));
            }
        }



    }
}
