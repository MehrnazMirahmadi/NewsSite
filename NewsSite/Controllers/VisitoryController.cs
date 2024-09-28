using DataAccess.Services;
using DomainModel.Models;
using DomainModel.ViewModels.Advertisment;
using Microsoft.AspNetCore.Mvc;
using NewsSite.FrameworkUI.Services;
using Framework.Implementations;
using DomainModel.ViewModels;
using DomainModel.ViewModels.News;
using Microsoft.AspNetCore.Mvc.Rendering;
using NewsSite.ViewModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Xml.Linq;
using System.Xml;
using DomainModel.Comon;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace NewsSite.Controllers
{
    public class VisitoryController : Controller
    {
        private readonly IAdvertiseVisitoryRepository _advertiseVisitoryRepository;
        private readonly IFileManager _fileManager;

        public VisitoryController(IAdvertiseVisitoryRepository advertiseVisitoryRepository, IFileManager fileManager)
        {
            _advertiseVisitoryRepository = advertiseVisitoryRepository;
            _fileManager = fileManager;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            var result = await _advertiseVisitoryRepository.GetAll();
            return View(result);
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

            // Validate the uploaded file
            if (advertisement.Picture == null || advertisement.Picture.Length == 0)
            {
                TempData["ErrorMessage"] = "Please upload a valid image.";
                return View(advertisement);
            }

            string fileName = Path.GetFileName(advertisement.Picture.FileName);
            var fileValidationResult = _fileManager.ValidateFileSize(advertisement.Picture, 2048, 2097152); // Min: 2KB, Max: 2MB

            if (!_fileManager.ValidateFileName(fileName))
            {
                TempData["ErrorMessage"] = "Invalid File Name";
                return View(advertisement);
            }

            if (!fileValidationResult.Success)
            {
                TempData["ErrorMessage"] = fileValidationResult.Message;
                return View(advertisement);
            }

            #region ImageAddress
            string uniqueFileName = _fileManager.ToUniquieFileName(fileName);
            string relativeAddress = _fileManager.ToRelativeAddress(uniqueFileName, "Visitory");
            string physicalAddress = _fileManager.ToPhysicalAddress(uniqueFileName, "Visitory");

            try
            {
                using (var fs = new FileStream(physicalAddress, FileMode.Create))
                {
                    await advertisement.Picture.CopyToAsync(fs);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error while saving the image: " + ex.Message;
                return View(advertisement);
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
                TempData["ErrorMessage"] = op.Message;
                return View(advertisement);
            }

            return Json(op);
        }
        #endregion
    }
}
