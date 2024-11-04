using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewSite.ViewModel.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Security;

namespace NewSite.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly UserManager<Security.ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private Security.SecurityContext db;
        public UserManagementController(UserManager<Security.ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, Security.SecurityContext db)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.db = db;
        }
        private string GetRoleNameByRoleID(int RoleID)
        {
            return db.Roles.SingleOrDefault(x => x.Id == RoleID).Name;
        }
        public IActionResult Index()
        {
            var q = from u in db.Users
                    join ur in db.UserRoles
 on u.Id equals ur.UserId
                    join r in db.Roles
on ur.RoleId equals r.Id
                    select new UserListItem
                    {
                        RoleName = r.Name
                        ,
                        FirstName = u.FirstName
                        ,
                        LastName = u.LastName
                        ,
                        UserName = u.UserName
                        ,
                        UserID = u.Id
                    };
            return View(q.ToList());

        }

        public IActionResult AddNewUser()
        {
            InflateRoles();
            return View();
        }

        private void InflateRoles()
        {
            List<RoleListItem> roles = roleManager.Roles.Select(x => new RoleListItem
            {
                RoleID = x.Id
    ,
                RoleName = x.Name
            }).ToList();
            SelectList roleList = new SelectList(roles, "RoleID", "RoleName");
            ViewBag.roleList = roleList;
        }

        [HttpPost]
        public async Task<IActionResult> AddNewUser(UserAddEditModel uservm)
        {

            ApplicationUser user = new ApplicationUser
            {
                Email = uservm.Email
                ,
                FirstName = uservm.FirstName
                ,
                LastName = uservm.LastName
                ,
                UserName = uservm.Email
            };
           

            IdentityResult userRegistrationResult = await userManager.CreateAsync(user, uservm.Password);
            IdentityResult makeUserMemberofRoleResult = null;
            if (userRegistrationResult.Succeeded)
            {

                var u = await userManager.FindByNameAsync(uservm.Email);
                var RoleName = db.Roles.SingleOrDefault(x => x.Id == uservm.RoleID).Name;
                makeUserMemberofRoleResult = await userManager.AddToRoleAsync(u, RoleName);
            }
            if (userRegistrationResult.Succeeded && makeUserMemberofRoleResult != null && makeUserMemberofRoleResult.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var err in userRegistrationResult.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                if (makeUserMemberofRoleResult != null && !makeUserMemberofRoleResult.Succeeded)
                {
                    foreach (var err in makeUserMemberofRoleResult.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                }
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> DeleteUser(int UserID)
        {
            var userName = db.Users.SingleOrDefault(x => x.Id == UserID).UserName;

            var CurrentUser = await userManager.FindByNameAsync(userName);

            var UserRoles = (from u in db.Users
                             join ur in db.UserRoles
          on u.Id equals ur.UserId
                             join r in db.Roles on ur.RoleId equals r.Id
                             select new { RoleName = r.Name, UserName = u.UserName }).ToList();
            foreach (var item in UserRoles)
            {
                await userManager.RemoveFromRoleAsync(CurrentUser, item.RoleName);
            }
            IdentityResult RemoveRsult = await userManager.DeleteAsync(CurrentUser);
            if (RemoveRsult.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var err in RemoveRsult.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return View();
            }

        }
        public async Task<IActionResult> EditUser(int UserID)
        {
            InflateRoles();
            var user = (from u in db.Users
                        join ur in db.UserRoles
     on u.Id equals ur.UserId
                        join r in db.Roles on ur.RoleId equals r.Id
                        select new UserAddEditModel
                        {
                            Email = u.Email
                            ,
                            FirstName = u.FirstName
                            ,
                            LastName = u.LastName
                            ,
                            RoleID = ur.RoleId
                            ,
                            UserID = u.Id
                        }).SingleOrDefault(x => x.UserID == UserID);
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateUser(UserAddEditModel newUserAddeditModel)
        {
            ApplicationUser oldApplicationUser = await userManager.FindByNameAsync(newUserAddeditModel.Email);


            var oldUserAddEditModel = (from u in db.Users
                                       join ur in db.UserRoles
                    on u.Id equals ur.UserId
                                       join r in db.Roles on ur.RoleId equals r.Id
                                       select new UserAddEditModel
                                       {
                                           Email = u.Email
                                           ,
                                           FirstName = u.FirstName
                                           ,
                                           LastName = u.LastName
                                           ,
                                           RoleID = ur.RoleId
                                           ,
                                           UserID = u.Id
                                       }).SingleOrDefault(x => x.UserID == newUserAddeditModel.UserID);

            if (oldUserAddEditModel.RoleID != newUserAddeditModel.RoleID)
            {
                string oldRoleName = GetRoleNameByRoleID(oldUserAddEditModel.RoleID);
                string newRoleName = GetRoleNameByRoleID(newUserAddeditModel.RoleID);
                await userManager.RemoveFromRoleAsync(oldApplicationUser, oldRoleName);
                await userManager.AddToRoleAsync(oldApplicationUser, newRoleName);

            }
            oldApplicationUser.FirstName = newUserAddeditModel.FirstName;
            oldApplicationUser.LastName = newUserAddeditModel.LastName;
            await userManager.CheckPasswordAsync(oldApplicationUser, newUserAddeditModel.Password); 

            IdentityResult result = await userManager.UpdateAsync(oldApplicationUser);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");

            }
            else
            {
                foreach (IdentityError err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return View();
            }
        }







    }
}
