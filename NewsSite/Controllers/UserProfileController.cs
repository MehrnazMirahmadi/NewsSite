using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Security;

namespace NewSite.Controllers
{
    //[Authorize(Roles = "SiteUser")]
    public class UserProfileController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
       
        public UserProfileController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
           //if (User!=null&& User.Identity.IsAuthenticated) { 
           //     string un = User.Identity.Name;
           // }
            return View();
        }
    }
}
