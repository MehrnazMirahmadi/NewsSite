using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewSite.ViewModel.Security; // فضای نام برای مدل‌های مربوط به امنیت
using Microsoft.AspNetCore.Identity; // فضای نام برای مدیریت هویت و اعتبارسنجی کاربران
using Microsoft.AspNetCore.Mvc; // فضای نام برای کنترلرهای MVC
using Microsoft.AspNetCore.Mvc.Rendering; // فضای نام برای کار با لیست‌های انتخابی
using Security; // فضای نام برای مدل‌های امنیتی

namespace NewSite.Controllers
{
    public class UserManagementController : Controller
    {
        // تعریف متغیرهای مربوط به UserManager، RoleManager و SignInManager
        private readonly UserManager<Security.ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private Security.SecurityContext db;

        // سازنده کنترلر که وابستگی‌ها را از طریق تزریق وابستگی دریافت می‌کند
        public UserManagementController(UserManager<Security.ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, Security.SecurityContext db)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.db = db;
        }

        // متد خصوصی برای دریافت نام نقش بر اساس شناسه نقش
        private string GetRoleNameByRoleID(int RoleID)
        {
            return db.Roles.SingleOrDefault(x => x.Id == RoleID).Name; // جستجو و بازگشت نام نقش
        }

        // متد برای نمایش لیست کاربران
        public IActionResult Index()
        {
            // استفاده از LINQ برای جمع‌آوری اطلاعات کاربران به همراه نقش‌هایشان
            var q = from u in db.Users
                    join ur in db.UserRoles
                       on u.Id equals ur.UserId
                    join r in db.Roles
                       on ur.RoleId equals r.Id
                    select new UserListItem
                    {
                        RoleName = r.Name,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        UserName = u.UserName,
                        UserID = u.Id
                    };
            return View(q.ToList()); // ارسال لیست کاربران به نمای View
        }

        // متد برای نمایش فرم افزودن کاربر جدید
        public IActionResult AddNewUser()
        {
            InflateRoles(); // بارگذاری نقش‌ها برای نمایش در فرم
            return View();
        }

        // متد خصوصی برای بارگذاری نقش‌ها به عنوان SelectList برای نمایش در فرم
        private void InflateRoles()
        {
            List<RoleListItem> roles = roleManager.Roles.Select(x => new RoleListItem
            {
                RoleID = x.Id,
                RoleName = x.Name
            }).ToList();
            SelectList roleList = new SelectList(roles, "RoleID", "RoleName");
            ViewBag.roleList = roleList; // قرار دادن لیست نقش‌ها در ViewBag برای استفاده در View
        }

        // متد POST برای ثبت کاربر جدید
        [HttpPost]
        public async Task<IActionResult> AddNewUser(UserAddEditModel uservm)
        {
            // ایجاد یک شی کاربر جدید
            ApplicationUser user = new ApplicationUser
            {
                Email = uservm.Email,
                FirstName = uservm.FirstName,
                LastName = uservm.LastName,
                UserName = uservm.Email
            };

            // ثبت کاربر جدید در پایگاه داده
            IdentityResult userRegistrationResult = await userManager.CreateAsync(user, uservm.Password);
            IdentityResult makeUserMemberofRoleResult = null;
            if (userRegistrationResult.Succeeded)
            {
                // یافتن کاربر بر اساس نام کاربری
                var u = await userManager.FindByNameAsync(uservm.Email);
                var RoleName = db.Roles.SingleOrDefault(x => x.Id == uservm.RoleID).Name; // یافتن نام نقش
                makeUserMemberofRoleResult = await userManager.AddToRoleAsync(u, RoleName); // افزودن کاربر به نقش
            }

            // بررسی موفقیت عملیات ثبت و افزودن به نقش
            if (userRegistrationResult.Succeeded && makeUserMemberofRoleResult != null && makeUserMemberofRoleResult.Succeeded)
            {
                return RedirectToAction("Index"); // بازگشت به لیست کاربران
            }
            else
            {
                // مدیریت خطاها در صورت عدم موفقیت
                foreach (var err in userRegistrationResult.Errors)
                {
                    ModelState.AddModelError("", err.Description); // افزودن خطا به ModelState
                }
                if (makeUserMemberofRoleResult != null && !makeUserMemberofRoleResult.Succeeded)
                {
                    foreach (var err in makeUserMemberofRoleResult.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                }
            }

            return RedirectToAction("Index"); // در نهایت بازگشت به لیست کاربران
        }

        // متد برای حذف کاربر بر اساس شناسه کاربر
        public async Task<IActionResult> DeleteUser(int UserID)
        {
            var userName = db.Users.SingleOrDefault(x => x.Id == UserID).UserName; // یافتن نام کاربر بر اساس شناسه

            var CurrentUser = await userManager.FindByNameAsync(userName); // یافتن کاربر از طریق UserManager

            // جمع‌آوری نقش‌های کاربر برای حذف آنها
            var UserRoles = (from u in db.Users
                             join ur in db.UserRoles
          on u.Id equals ur.UserId
                             join r in db.Roles on ur.RoleId equals r.Id
                             select new { RoleName = r.Name, UserName = u.UserName }).ToList();

            // حذف کاربر از تمام نقش‌ها
            foreach (var item in UserRoles)
            {
                await userManager.RemoveFromRoleAsync(CurrentUser, item.RoleName);
            }
            IdentityResult RemoveRsult = await userManager.DeleteAsync(CurrentUser); // حذف کاربر

            // بررسی موفقیت حذف کاربر
            if (RemoveRsult.Succeeded)
            {
                return RedirectToAction("Index"); // بازگشت به لیست کاربران
            }
            else
            {
                // مدیریت خطا در صورت عدم موفقیت حذف
                foreach (var err in RemoveRsult.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return View(); // نمایش نمای فعلی با خطا
            }
        }

        // متد برای نمایش فرم ویرایش کاربر بر اساس شناسه
        public async Task<IActionResult> EditUser(int UserID)
        {
            InflateRoles(); // بارگذاری نقش‌ها
            var user = (from u in db.Users
                        join ur in db.UserRoles
     on u.Id equals ur.UserId
                        join r in db.Roles on ur.RoleId equals r.Id
                        select new UserAddEditModel
                        {
                            Email = u.Email,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            RoleID = ur.RoleId,
                            UserID = u.Id
                        }).SingleOrDefault(x => x.UserID == UserID); // جستجو کاربر با شناسه مشخص

            return View(user); // ارسال کاربر به نمای ویرایش
        }

        // متد POST برای به‌روزرسانی اطلاعات کاربر
        [HttpPost]
        public async Task<IActionResult> UpdateUser(UserAddEditModel newUserAddeditModel)
        {
            ApplicationUser oldApplicationUser = await userManager.FindByNameAsync(newUserAddeditModel.Email); // یافتن کاربر قدیمی

            // جستجوی اطلاعات کاربر قدیمی برای مقایسه
            var oldUserAddEditModel = (from u in db.Users
                                       join ur in db.UserRoles
                    on u.Id equals ur.UserId
                                       join r in db.Roles on ur.RoleId equals r.Id
                                       select new UserAddEditModel
                                       {
                                           Email = u.Email,
                                           FirstName = u.FirstName,
                                           LastName = u.LastName,
                                           RoleID = ur.RoleId,
                                           UserID = u.Id
                                       }).SingleOrDefault(x => x.UserID == newUserAddeditModel.UserID);

            // بررسی تغییر نقش و به‌روزرسانی آن در صورت لزوم
            if (oldUserAddEditModel.RoleID != newUserAddeditModel.RoleID)
            {
                string oldRoleName = GetRoleNameByRoleID(oldUserAddEditModel.RoleID);
                string newRoleName = GetRoleNameByRoleID(newUserAddeditModel.RoleID);
                await userManager.RemoveFromRoleAsync(oldApplicationUser, oldRoleName); // حذف کاربر از نقش قدیمی
                await userManager.AddToRoleAsync(oldApplicationUser, newRoleName); // افزودن کاربر به نقش جدید
            }

            // به‌روزرسانی اطلاعات کاربر
            oldApplicationUser.FirstName = newUserAddeditModel.FirstName;
            oldApplicationUser.LastName = newUserAddeditModel.LastName;
            await userManager.CheckPasswordAsync(oldApplicationUser, newUserAddeditModel.Password); // بررسی رمز عبور

            IdentityResult result = await userManager.UpdateAsync(oldApplicationUser); // به‌روزرسانی کاربر در پایگاه داده

            // بررسی موفقیت به‌روزرسانی
            if (result.Succeeded)
            {
                return RedirectToAction("Index"); // بازگشت به لیست کاربران
            }
            else
            {
                // مدیریت خطاها در صورت عدم موفقیت
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }

            return View(); // نمایش نمای ویرایش در صورت وجود خطا
        }
    }
}
