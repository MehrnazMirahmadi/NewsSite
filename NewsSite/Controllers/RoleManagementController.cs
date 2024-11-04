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
        #region AddRole
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
        #endregion
        #region GET: RoleManagement/Edit/{id}
        public async Task<IActionResult> Edit(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var model = new RoleAddEditModel
            {
                RoleID = role.Id,
                RoleName = role.Name,
                Description = role.Description
            };

            return View(model);
        }

        #endregion
        #region Edit
        // POST: RoleManagement/Edit/{id}
        [HttpPost]
        public async Task<IActionResult> Edit(RoleAddEditModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var role = await roleManager.FindByIdAsync(model.RoleID.ToString());
            if (role == null)
            {
                return NotFound();
            }

            role.Name = model.RoleName;
            role.Description = model.Description;

            var updateResult = await roleManager.UpdateAsync(role);
            if (updateResult.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }
        #endregion
        #region Delete
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var deleteResult = await roleManager.DeleteAsync(role);
            if (deleteResult.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in deleteResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("Delete", new RoleAddEditModel { RoleID = role.Id, RoleName = role.Name, Description = role.Description });
            }
        }
        #endregion
    }
}
