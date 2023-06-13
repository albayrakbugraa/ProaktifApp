using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ProaktifArizaTahmini.BLL.AutoMapper;
using ProaktifArizaTahmini.BLL.Services;
using ProaktifArizaTahmini.CORE.IRepository;
using ProaktifArizaTahmini.DAL;
using ProaktifArizaTahmini.DAL.Repositories;
using System.Configuration;
using ProaktifArizaTahmini.BLL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddServices(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Mapping));
ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Ticari olmayan kullan�m i�in

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
