using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewSite.ViewModel.Security;
using Security;

namespace NewSite.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class RoleManagementController : Controller
    {

        private readonly RoleManager<ApplicationRole> roleManager;
        public RoleManagementController(RoleManager<ApplicationRole> roleManager)
        {
            this.roleManager = roleManager;
        }
        public IActionResult Index()
        {

            return View(roleManager.Roles.ToList());
        }
        public IActionResult AddNewRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddNewRole(RoleAddEditModel roleAddEditModel)
        {
            ApplicationRole r = new ApplicationRole
            {
                Description = roleAddEditModel.Description
                ,
                Name = roleAddEditModel.RoleName
            };

            IdentityResult createRoleResult = await roleManager.CreateAsync(r);
            if (createRoleResult.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                foreach (IdentityError err in createRoleResult.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return View();
            }

        }
    }
}
