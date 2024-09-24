using DataAccess.Repositories;
using DataAccess.Services;
using DomainModel.Models;
using Microsoft.EntityFrameworkCore;
using NewsSite.FrameworkUI;
using NewsSite.FrameworkUI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
#region DBContext
builder.Services.AddDbContext<NewsDBContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));
#endregion
#region IoC
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<INewsCategoryRepository, NewsCategoryRepository>();
builder.Services.AddScoped<IFileManager,FileManager>();
builder.Services.AddScoped<IAdvertiseVisitoryRepository, AdvertiseVisitoryRepository>();
builder.Services.AddScoped<IMenuVisitoryRepository, MenuVisitoryRepository>();
#endregion
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
