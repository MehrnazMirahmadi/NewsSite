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
using NewsSite.FrameworkUI.Services;
using DomainModel.Comon;

namespace NewsSite.Controllers
{
    public class NewsController : Controller
    {
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

        #region Cto
        private readonly INewsRepository newsRepo;
        private readonly INewsCategoryRepository catRepo;
        private readonly IHostEnvironment env;
        private readonly IFileManager fileManager;
        public NewsController(INewsRepository newsRepo, INewsCategoryRepository catRepo, IHostEnvironment env, IFileManager fileManager)
        {
            this.newsRepo = newsRepo;
            this.catRepo = catRepo;
            this.env = env;
            this.fileManager = fileManager;
        }
        #endregion

        #region InflateCategories
        public async Task InflateCategories()
        {
            var cats = await catRepo.GetAll();
            cats.Insert(0, new NewsCategoryListItem
            {
                CategoryName = "...Please Select"
            ,
                NewsCategoryID = -1
            });
            var Categories = new SelectList(cats, "NewsCategoryID", "CategoryName");
            ViewBag.Categories = Categories;

        }
        #endregion

        #region Index
        public async Task<IActionResult> Index(NewsSearchModel sm)
        {
            await InflateCategories();
            return View(sm);
        }
        public async Task<IActionResult> NewListAction(NewsSearchModel sm)
        {
            return ViewComponent("NewsList", sm);
        }
        #endregion

        #region Add
        public async Task<IActionResult> Add()
        {
            await InflateCategories();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(NewsAddEditViewModel news)
        {
            if (!ModelState.IsValid)
            {
                await LogInvalidModelStateToXml(ModelState);
                await InflateCategories();
                return View();
            }

            string fileName = Path.GetFileName(news.Picture.FileName);
            var fileValidationResult = fileManager.ValidateFileSize(news.Picture, 2048, 2097152);

            if (!fileManager.ValidateFileName(fileName))
            {
                TempData["ErrorMessage"] = "Invalid File Name";
                return View(news);
            }
            if (!fileValidationResult.Success)
            {
                TempData["ErrorMessage"] = fileValidationResult.Message;
                return View(news);
            }
            if (news.NewsCategoryID < 0)
            {
                TempData["ErrorMessage"] = "Please Select a Category";
                return View(news);
            }
            #region ImageAddress
            string uniqueFileName = fileManager.ToUniquieFileName(fileName);
            string relativeAddress = fileManager.ToRelativeAddress(uniqueFileName, "NewsImage");
            string physicalAddress = fileManager.ToPhysicalAddress(uniqueFileName, "NewsImage");

            using (FileStream fs = new FileStream(physicalAddress, FileMode.Create))
            {
                await news.Picture.CopyToAsync(fs);
                fs.Close();
            }
            #endregion

            NewsAddEditModel n = new NewsAddEditModel
            {
                ImageUrl = relativeAddress
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
                await InflateCategories();
                return View(news);
            }
            return RedirectToAction("index");
        }
        #endregion

        #region DeleteNews
        [HttpPost]
        public async Task<IActionResult> Delete(int NewsID)
        {
            var news = await newsRepo.Get(NewsID);
            if (news == null)
            {
                TempData["ErrorMessage"] = "News item not found.";
                return RedirectToAction("Index", "News");
            }
            if (!string.IsNullOrEmpty(news.ImageUrl))
            {
                var imagePath = fileManager.ToPhysicalAddress(news.ImageUrl, "NewsImage");//.Substring(1, news.ImageUrl.Length - 1).Replace(@"/", @"\"), ""
                bool isFileRemoved = fileManager.RemoveFile(imagePath);
                if (!isFileRemoved)
                {
                    TempData["ErrorMessage"] = "Failed to delete the image file.";
                    await InflateCategories();
                }
            }
            var op = await newsRepo.Delete(NewsID);

            if (!op.Success)
            {
                TempData["ErrorMessage"] = "Failed to delete the news item.";
                return RedirectToAction("Index", "News");
            }
            return RedirectToAction("Index", "News");
        }
        #endregion

        #region Update 
     
        [Route("News/Update/{NewsID}")]
        public async Task<IActionResult> Update(int NewsID)
        {
            var n = await newsRepo.Get(NewsID);
            var news = new ViewModel.NewsDetailsModel
            {
                NewsID = NewsID,
                NewsCategoryID = n.NewsCategoryID,
                NewsTitle = n.NewsTitle,
                NewsText = n.NewsText,
                ImageUrl = n.ImageUrl,
                RegistrationDate = n.RegistrationDate,
                Slug = n.Slug,
                SmallDescription = n.SmallDescription,
                SortOrder = n.SortOrder,
                VisitCount = n.VisitCount,
                VoteCount = n.VoteCount,
                VoteSumation = n.VoteSumation

            };
            await InflateCategories();
            return View(news);
        }
        [HttpPost]
        [Route("News/Update/{NewsID}")]
        public async Task<IActionResult> Update(NewsAddEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LogInvalidModelStateToXml(ModelState);
                await InflateCategories();
                return View(model);
            }

            var news = await newsRepo.Get(model.NewsID);
            if (news == null)
            {
                return NotFound();
            }

            // Update news fields
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
                string fileName = Path.GetFileName(model.Picture.FileName);

                var fileValidationResult = fileManager.ValidateFileSize(model.Picture, 2048, 2097152);
                if (!fileValidationResult.Success)
                {
                    TempData["ErrorMessage"] = fileValidationResult.Message;
                    await InflateCategories();
                    return View(model);
                }

                if (!fileManager.ValidateFileName(fileName))
                {
                    TempData["ErrorMessage"] = "Invalid file name.";
                    await InflateCategories();
                    return View(model);
                }
                if (!model.Picture.IsValidImageHeader())
                {
                    TempData["ErrorMessage"] = "Invalid image format.";
                    await InflateCategories();
                    return View(model);
                }
                if (!string.IsNullOrEmpty(news.ImageUrl))
                {
                    var oldImagePath = Path.Combine(env.ContentRootPath, "wwwroot", news.ImageUrl.TrimStart('/'));
                    fileManager.RemoveFile(oldImagePath);
                }
                var saveResult = fileManager.SaveFile(model.Picture, "NewsImage");
                if (!saveResult.Success)
                {
                    TempData["ErrorMessage"] = "Failed to save image. " + saveResult.Message;
                    await InflateCategories();
                    return View(model);
                }

                news.ImageUrl = fileManager.ToRelativeAddress(saveResult.Message, "NewsImage");
            }
            var updateResult = await newsRepo.Update(news);
            if (!updateResult.Success)
            {
                TempData["ErrorMessage"] = updateResult.Message;
                await InflateCategories();
                return View(model);
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region DeleteImage
        [HttpPost]
        public async Task<IActionResult> DeleteImage(int NewsID)
        {
            var news = await newsRepo.Get(NewsID);
            if (news == null)
            {
                return Json(new OperationResult().ToFailed("News item not found."));
            }
            if (!string.IsNullOrEmpty(news.ImageUrl) && news.ImageUrl.ToLower() != "~/pics/noimage.svg")
            {
                var imagePath = fileManager.ToPhysicalAddress(news.ImageUrl.Substring(1).Replace("/", "\\"), "");
                bool isFileRemoved = fileManager.RemoveFile(imagePath);
                if (!isFileRemoved)
                {
                    return Json(new OperationResult().ToFailed("Failed to delete the image file."));
                }
            }
            OperationResult op = new OperationResult();
            try
            {
                await newsRepo.RemoveImage(NewsID);
                return Json(op.ToSuccess("Image removed successfully."));
            }
            catch (Exception)
            {
                return Json(op.ToFailed("Image could not be removed."));
            }
        }

        #endregion

    }

}
