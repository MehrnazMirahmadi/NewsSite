using DataAccess.Repositories;
using DataAccess.Services;
using DomainModel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NewsSite.FrameworkUI;
using NewsSite.FrameworkUI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

#region DBContext
// Configure database context for NewsDBContext
string cnnstr = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection string is not configured.");
builder.Services.AddDbContext<NewsDBContext>(options =>
{
    options.UseSqlServer(cnnstr);
}, ServiceLifetime.Scoped);

// Configure database context for SecurityContext
string securityConnectionString = builder.Configuration.GetConnectionString("SecurityConnectionString") ?? throw new InvalidOperationException("SecurityConnectionString is not configured.");
builder.Services.AddDbContext<Security.SecurityContext>(options =>
{
    options.UseSqlServer(securityConnectionString);
}, ServiceLifetime.Scoped);
#endregion

#region Identity
// Configure Identity
builder.Services.AddIdentity<Security.ApplicationUser, Security.ApplicationRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Lockout.MaxFailedAccessAttempts = 10;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
})
.AddEntityFrameworkStores<Security.SecurityContext>()
.AddDefaultTokenProviders();

// Configure Authentication cookie for Forms Authentication
builder.Services.ConfigureApplicationCookie(options =>
{
   
    options.Cookie.Name = ".MyAuthCookie";// نام کوکی
    options.Cookie.HttpOnly = true; // کوکی فقط برای HTTP است
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // مدت زمان انقضا
    options.SlidingExpiration = true; // تمدید زمان انقضا به صورت خودکار
    options.LoginPath = "/Account/Login"; // مسیر صفحه ورود
    options.LogoutPath = "/Account/Logout"; // مسیر صفحه خروج
    options.AccessDeniedPath = "/Account/AccessDenied"; // مسیر دسترسی ممنوع
});
#endregion

#region IoC
// Register repositories and services in the IoC container
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<INewsCategoryRepository, NewsCategoryRepository>();
builder.Services.AddScoped<IFileManager, FileManager>();
builder.Services.AddScoped<IAdvertiseVisitoryRepository, AdvertiseVisitoryRepository>();
builder.Services.AddScoped<IMenuVisitoryRepository, MenuVisitoryRepository>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Adds HSTS header with a default max-age of 30 days for production
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
