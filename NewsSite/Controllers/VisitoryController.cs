using DataAccess.Services;
using DomainModel.Models;
using DomainModel.ViewModels.Advertisment;
using Framework.Implementations;
using Microsoft.AspNetCore.Mvc;
using NewsSite.FrameworkUI;
using NewsSite.FrameworkUI.Services;


namespace NewsSite.Controllers
{
    public class VisitoryController : Controller
    {
        #region Ctor
        private readonly IAdvertiseVisitoryRepository _advertiseVisitoryRepository;
        private readonly IFileManager _fileManager;
        private readonly IHostEnvironment env;

        public VisitoryController(IAdvertiseVisitoryRepository advertiseVisitoryRepository, IFileManager fileManager,IHostEnvironment hostEnvironment)
        {
            _advertiseVisitoryRepository = advertiseVisitoryRepository;
            _fileManager = fileManager;
            env = hostEnvironment;
        }

        #endregion

        #region Index
        public async Task<IActionResult> Index(AdvertismentSearchModel sm)
        {
            
            return View(sm);
        }
        public async Task<IActionResult> AdvertismentList(AdvertismentSearchModel sm)
        {
            return ViewComponent("AdvertismentList", sm);
        }
        #endregion

        #region Add
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AdvertismentAddEditViewModel advertisement)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new { success = false, message = "Invalid model data", errors = errors });
            }
            if (advertisement.Picture == null || advertisement.Picture.Length == 0)
            {
                return Json(new { success = false, message = "Please upload a valid image." });
            }

            string fileName = Path.GetFileName(advertisement.Picture.FileName);
            var fileValidationResult = _fileManager.ValidateFileSize(advertisement.Picture, 2048, 2097152);

            if (!_fileManager.ValidateFileName(fileName))
            {
                return Json(new { success = false, message = "Invalid File Name" });
            }

            if (!fileValidationResult.Success)
            {
                return Json(new { success = false, message = fileValidationResult.Message });
            }

            #region ImageAddress
            string uniqueFileName = _fileManager.ToUniquieFileName(fileName);
            string relativeAddress = _fileManager.ToRelativeAddress(uniqueFileName, "Visitory/img");
            string physicalAddress = _fileManager.ToPhysicalAddress(uniqueFileName, "Visitory/img");

            try
            {
                using (var fs = new FileStream(physicalAddress, FileMode.Create))
                {
                    await advertisement.Picture.CopyToAsync(fs);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error while saving the image: " + ex.Message });
            }
            #endregion

            var newAd = new AdvertismentAddEditViewModel
            {
                Title = advertisement.Title,
                ImageUrl = relativeAddress,
                IsDefault = advertisement.IsDefault,
                Url = advertisement.Url,
                Alt = advertisement.Alt
            };

            var op = await _advertiseVisitoryRepository.Add(newAd);
            if (!op.Success)
            {
                return Json(new { success = false, message = op.Message });
            }

            // Return success response with message and redirect flag
            return Json(new { success = true, message = "Advertisement added successfully!", redirect = true });
        }

        #endregion
        #region Update 
        public async Task<IActionResult> Update(int ID)
        {
            var adv = await _advertiseVisitoryRepository.Get(ID);
            if (adv == null)
            {
                return NotFound();
            }
            var viewModel = new AdvertismentAddEditViewModel
            {
                AdvertisementID = ID,
                Title = adv.Title,
                ImageUrl = adv.ImageUrl,
                Url = adv.Url,
                Alt = adv.Alt,
                IsDefault = adv.IsDefault
            };
            return View(viewModel);
        }
   
        [HttpPost]
      
        public async Task<IActionResult> Update(AdvertismentAddEditViewModel model)
        {
            var adv = await _advertiseVisitoryRepository.Get(model.AdvertisementID);
            if (adv == null)
            {
                return NotFound();
            }
            adv.AdvertisementID = model.AdvertisementID;
            adv.Title = model.Title; 
            adv.Url = model.Url; 
            adv.Alt = model.Alt; 
            adv.IsDefault = model.IsDefault; 

            if (model.Picture != null)
            {
                string fileName = Path.GetFileName(model.Picture.FileName);

                var fileValidationResult = _fileManager.ValidateFileSize(model.Picture, 2048, 2097152); 
                if (!fileValidationResult.Success)
                {
                    TempData["ErrorMessage"] = fileValidationResult.Message;
                    return View(model);
                }

                if (!_fileManager.ValidateFileName(fileName)) 
                {
                    TempData["ErrorMessage"] = "Invalid file name.";
                    return View(model);
                }

                if (!model.Picture.IsValidImageHeader()) 
                {
                    TempData["ErrorMessage"] = "Invalid image format.";
                    return View(model);
                }
                if (!string.IsNullOrEmpty(adv.ImageUrl))
                {
                    var oldImagePath = Path.Combine(env.ContentRootPath, "wwwroot", adv.ImageUrl.TrimStart('/'));
                    _fileManager.RemoveFile(oldImagePath);
                }

                var saveResult = _fileManager.SaveFile(model.Picture, "Visitory/img"); 
                if (!saveResult.Success)
                {
                    TempData["ErrorMessage"] = "Failed to save image. " + saveResult.Message;
                    return View(model);
                }

                adv.ImageUrl = _fileManager.ToRelativeAddress(saveResult.Message, "Visitory/img"); 
            }

            var op = await _advertiseVisitoryRepository.Update(adv); 
            if (!op.Success)
            {
                TempData["ErrorMessage"] = op.Message;
                return View(model);
            }

            TempData["SuccessMessage"] = "Advertisement updated successfully!";
            return RedirectToAction("Index"); 
        }


    }

    #endregion
}

