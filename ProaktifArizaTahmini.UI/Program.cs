using ProaktifArizaTahmini.BLL.AutoMapper;
using ProaktifArizaTahmini.BLL;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddServices(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Mapping));
builder.Services.AddMemoryCache();
builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
    });
builder.Services.Configure<CookiePolicyOptions>(opt =>
{
    opt.Secure = CookieSecurePolicy.Always;
});
builder.Services.AddLogging(log =>
{
    log.ClearProviders();
    log.AddFile(builder.Configuration["Logging:LogFilePath"].ToString(), LogLevel.Information);
});
var app = builder.Build();

// Configure the HTTP request pipeline.

//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

app.UseExceptionHandler("/Home/Error");
// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
app.UseHsts();

app.UseStatusCodePagesWithReExecute("Home/Error");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCookiePolicy();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
