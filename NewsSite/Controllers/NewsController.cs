using DataAccess.Services;
using DomainModel.ViewModels;
using DomainModel.ViewModels.News;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NewsSite.ViewModel;
using Framework.Implementations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Xml.Linq;
using System.Xml;

namespace NewsSite.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsRepository newsRepo;
        private readonly INewsCategoryRepository catRepo;
        private readonly IHostEnvironment env;
        public async Task InflateCategories()
        {
            var cats = await catRepo.GetAll();
            cats.Insert(0, new NewsCategoryListItem
            {
                CategoryName = "...Please Select"
            ,
                NewsCategoryID = -1
            });
            ViewBag.Categories = new SelectList(cats, "NewsCategoryID", "CategoryName");

        }
        [Route("News/Update/{NewsID}")]
        public async Task<IActionResult> Update(int NewsID)
        {
         await InflateCategories();
            var news = await newsRepo.Get(NewsID);

            if (news == null)
            {
                return NotFound();
            }

            var model = new NewsAddEditViewModel
            {
                NewsID = news.NewsID,
                NewsTitle = news.NewsTitle,
                Slug = news.Slug,
                NewsCategoryID = news.NewsCategoryID,
                SortOrder = news.SortOrder,
                NewsText = news.NewsText,
                SmallDescription = news.SmallDescription,
                VisitCount = news.VisitCount,
                VoteCount = news.VoteCount,
                VoteSumation = news.VoteSumation,
                ImageUrl = news.ImageUrl
            };

            return View(model);
        }

        [HttpPost]
        [Route("News/Update/{NewsID}")]
 
        public async Task<IActionResult> Update(NewsAddEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await InflateCategories();
                return View(model);
            }

            var news = await newsRepo.Get(model.NewsID);
            if (news == null)
            {
                return NotFound();
            }
            news.NewsTitle = model.NewsTitle;
            news.Slug = model.Slug;
            news.NewsCategoryID = model.NewsCategoryID;
            news.SortOrder = model.SortOrder;
            news.NewsText = model.NewsText;
            news.SmallDescription = model.SmallDescription;
            news.VisitCount = model.VisitCount;
            news.VoteCount = model.VoteCount;
            news.VoteSumation = model.VoteSumation;

            if (model.Picture != null)
            {
                if (!model.Picture.FileName.CheckFileName())
                {
                    TempData["ErrorMessage"] = "Invalid file name.";
                    await InflateCategories();
                    return View(model);
                }

                if (model.Picture.Length < 2048 || model.Picture.Length > 2097152)
                {
                    TempData["ErrorMessage"] = "Invalid file size.";
                    await InflateCategories();
                    return View(model);
                }

                if (!string.IsNullOrEmpty(news.ImageUrl))
                {
                    var oldImagePath = Path.Combine(env.ContentRootPath, "wwwroot", news.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                if (!model.Picture.IsValidImageHeader())
                {
                    TempData["ErrorMessage"] = "Invalid image format.";
                    return View(model);
                }

                var physicalFileName = Path.GetFileName(model.Picture.FileName).ToUniqueFileName();
                var relativePath = @"~/NewsImage/" + physicalFileName;
                var physicalPath = env.ContentRootPath + @"\wwwroot\NewsImage\" + physicalFileName;
             

                using (var fs = new FileStream(physicalPath, FileMode.Create))
                {
                    await model.Picture.CopyToAsync(fs);
                }

                news.ImageUrl = relativePath;
            }
            else
            {
                news.ImageUrl = news.ImageUrl;
            }

            var op = await newsRepo.Update(news);
            if (!op.Success)
            {
                TempData["ErrorMessage"] = op.Message;
                await InflateCategories();
                return View(model);
            }

            return RedirectToAction("Index");
        }

        #region Xml
        private async Task LogInvalidModelStateToXml(ModelStateDictionary modelState)
        {
            var logDirectory = Path.Combine(env.ContentRootPath, "logs");

          
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            var xmlFilePath = Path.Combine(logDirectory, "ModelStateErrors.xml");

            var xDocument = new XDocument(
                new XElement("ModelStateErrors",
                    modelState
                        .Where(ms => ms.Value.Errors.Count > 0)
                        .Select(ms => new XElement("Error",
                            new XElement("Key", ms.Key),
                            new XElement("Errors",
                                ms.Value.Errors.Select(e =>
                                    new XElement("ErrorMessage", e.ErrorMessage)
                                )
                            )
                        ))
                )
            );

            using (var writer = XmlWriter.Create(xmlFilePath, new XmlWriterSettings { Indent = true }))
            {
                xDocument.WriteTo(writer);
            }

            await Task.CompletedTask;
        }


        #endregion

        public NewsController(INewsRepository newsRepo, INewsCategoryRepository catRepo, IHostEnvironment env)
        {
            this.newsRepo = newsRepo;
            this.catRepo = catRepo;
            this.env = env;
        }
        public async Task<IActionResult> Index(NewsSearchModel sm)
        {
            await InflateCategories();
            return View(sm);
        }
        public async Task<IActionResult> NewListAction(NewsSearchModel sm)
        {
            return ViewComponent("NewsList", sm);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int NewsID)
        {
            var news = await newsRepo.Get(NewsID);
            if (!string.IsNullOrEmpty(news.ImageUrl))
            {
                var url = env.ContentRootPath + @"\wwwroot" + news.ImageUrl.Substring(1, news.ImageUrl.Length - 1).Replace(@"/", @"\");
                if (System.IO.File.Exists(url))
                {
                    System.IO.File.Delete(url);
                }
            }
            var op = await newsRepo.Delete(NewsID);


            return RedirectToAction("Index", "News");
        }
        public async Task<IActionResult> Add()
        {
            await InflateCategories();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(NewsAddEditViewModel news)
        {

            if (news.NewsCategoryID < 0)
            {
                TempData["ErrorMessage"] = "Please Select a Category";
                return View(news);
            }

            if (!news.Picture.FileName.CheckFileName())
            {
                TempData["ErrorMessage"] = "Invalid FileName";
                return View(news);
            }
            if (news.Picture.Length < 2048 || news.Picture.Length > 2097152)
            {
                TempData["ErrorMessage"] = "Invalid File Size";
                return View(news);
            }

            string PhisycalAddress = Path.GetFileName(news.Picture.FileName).ToUniqueFileName();
            string Relativeaddress = @"~/NewsImage/" + PhisycalAddress;
            PhisycalAddress = env.ContentRootPath + @"\wwwroot\NewsImage\" + PhisycalAddress;
            FileStream fs = new FileStream(PhisycalAddress, FileMode.Create);
            {
                await news.Picture.CopyToAsync(fs);
            };
            NewsAddEditModel n = new NewsAddEditModel
            {
                ImageUrl = Relativeaddress
                ,
                NewsCategoryID = news.NewsCategoryID
                ,
                NewsText = news.NewsText
                ,
                NewsTitle = news.NewsTitle
                ,
                RegistrationDate = DateTime.Now
                ,
                Slug = news.Slug
                ,
                SmallDescription = news.SmallDescription
                ,
                SortOrder = news.SortOrder
                ,
                VisitCount = news.VisitCount
                ,
                VoteCount = news.VoteCount
                ,
                VoteSumation = news.VoteSumation

            };
            var op = await newsRepo.Add(n);
            if (!op.Success)
            {
                TempData["ErrorMessage"] = op.Message;
                return View(news);
            }
            return RedirectToAction("index");
        }



    }

}
