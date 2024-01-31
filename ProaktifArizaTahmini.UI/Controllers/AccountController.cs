using Microsoft.AspNetCore.Mvc;
using ProaktifArizaTahmini.BLL.Models.RequestModel;
using ProaktifArizaTahmini.BLL.Services.UserServices;
using ProaktifArizaTahmini.CORE.Enums;
using ProaktifArizaTahmini.UI.Helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Runtime.InteropServices;
using ProaktifArizaTahmini.UI.Models;
using System.Diagnostics;
using ProaktifArizaTahmini.BLL.Services.UserLogServices;

namespace ProaktifArizaTahmini.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService userService;
        private readonly IUserLogService userLogService;
        private readonly IConfiguration configuration;

        public AccountController(IUserService userService, IUserLogService userLogService, IConfiguration configuration)
        {
            this.userService = userService;
            this.userLogService = userLogService;
            this.configuration = configuration;
        }

        public async Task<IActionResult> Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (claimUser.Identity.IsAuthenticated)
            {
                string username = claimUser.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await userService.GetUserByUsername(username);
                await userLogService.LogIn(user);
                return RedirectToAction("List", "RelayInformation");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var domainGroupsSection = configuration.GetSection("AppSettings:DomainGroups");
            var domainGroups = domainGroupsSection.Get<List<DomainGroup>>();
            loginModel.DomainGroups = domainGroups;

            if (string.IsNullOrEmpty(loginModel.Username) || string.IsNullOrEmpty(loginModel.Password))
            {
                ModelState.AddModelError("Error", "Kullanıcı adı ve şifre girmelisiniz.");
                loginModel.LoginResult = LoginResult.LoginFailed;
                return View();
            }

            if (new ActiveDirectory().IsAuthenticate(loginModel))
            {
                var user = await userService.GetUserByUsername(loginModel.Username);
                if (user != null)
                {
                    if (user.IsActive == null || !user.IsActive.Value)
                    {
                        ModelState.AddModelError("State", "Kullanıcınız aktif değildir !");
                        loginModel.LoginResult = LoginResult.WaitingActivated;
                        return View();
                    }
                    user.LastLoginDate = DateTime.Now;
                    bool result = await userService.UpdateUser(user);
                    loginModel.LoginResult = LoginResult.LoginSuccess;

                    List<Claim> claims = new List<Claim>() {
                    new Claim(ClaimTypes.NameIdentifier, user.Username),
                    new Claim(ClaimTypes.Name,user.Name),
                    //new Claim(ClaimTypes.Surname,user.Surname),
                     };

                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                        CookieAuthenticationDefaults.AuthenticationScheme);

                    AuthenticationProperties properties = new AuthenticationProperties()
                    {
                        AllowRefresh = true,
                        IsPersistent = loginModel.RememberMe
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), properties);
                    await userLogService.LogIn(user);
                    return RedirectToAction("List", "RelayInformation");
                }
                else
                {
                    if (!await userService.CheckUserByUsername(loginModel.Username))
                    {
                        //İlk Kez Giriş Yapıldığı İçin Yeni Bir Kullanıcı Oluşturuyoruz
                        var domainUser = new ActiveDirectory().IsActiveDirectory(loginModel);
                        if (domainUser == null)
                        {
                            ModelState.AddModelError("Error", "Bu Kullanıcı Adı ve Şifreye Ait Herhangi Bir Kayıt Bulunamadı");
                            loginModel.LoginResult = LoginResult.LoginFailed;
                            return View();
                        }
                        domainUser.IsActive = true;
                        domainUser.UserTypeId = (int?)UserTypeNames.Domain;
                        domainUser.LastLoginDate = DateTime.Now;
                        bool createResult = await userService.CreateUser(domainUser);
                        loginModel.LoginResult = LoginResult.LoginSuccess;

                        List<Claim> claims = new List<Claim>() {
                        new Claim(ClaimTypes.NameIdentifier, domainUser.Username),
                        new Claim(ClaimTypes.Name,domainUser.Name),
                        //new Claim(ClaimTypes.Surname,domainUser.Surname),
                         };

                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                            CookieAuthenticationDefaults.AuthenticationScheme);

                        AuthenticationProperties properties = new AuthenticationProperties()
                        {
                            AllowRefresh = true,
                            IsPersistent = loginModel.RememberMe
                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity), properties);
                        var loginUser = await userService.GetUserByUsername(loginModel.Username);
                        await userLogService.LogIn(loginUser);
                        return RedirectToAction("List", "RelayInformation");
                    }
                }
            }
            else
            {
                //Domainde bu kullanıcı yoksa ve sistemde varsa bu kullanıcıyı siliyoruz
                var user = await userService.GetUserByUsername(loginModel.Username);
                if (user != null)
                {
                    if (user.UserTypeId != (int?)UserTypeNames.Domain)
                    {
                        if (ModelState.IsValid)
                        {
                            if (user.IsActive == null || !user.IsActive.Value)
                            {
                                ModelState.AddModelError("State", "Kullanıcınız aktif değildir !");
                                loginModel.LoginResult = LoginResult.WaitingActivated;
                                return View();
                            }
                            else
                            {
                                user.LastLoginDate = DateTime.Now;
                                bool result = await userService.UpdateUser(user);
                                loginModel.LoginResult = LoginResult.LoginSuccess;

                                List<Claim> claims = new List<Claim>() {
                                new Claim(ClaimTypes.NameIdentifier, user.Username),
                                new Claim(ClaimTypes.Name,user.Name),
                                //new Claim(ClaimTypes.Surname,user.Surname),
                                 };

                                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                                    CookieAuthenticationDefaults.AuthenticationScheme);

                                AuthenticationProperties properties = new AuthenticationProperties()
                                {
                                    AllowRefresh = true,
                                    IsPersistent = loginModel.RememberMe
                                };

                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                    new ClaimsPrincipal(claimsIdentity), properties);
                                await userLogService.LogIn(user);
                                return RedirectToAction("List", "RelayInformation");
                            }
                        }
                    }
                    else
                    {
                        bool result = await userService.DeleteUser(user.ID);
                        ModelState.AddModelError("Error", "Bu Kullanıcı Adı ve Şifreye Ait Herhangi Bir Kayıt Bulunamadı");
                    }
                }

                loginModel.LoginResult = LoginResult.LoginFailed;
                ModelState.AddModelError("Error", "Bu Kullanıcı Adı ve Şifreye Ait Herhangi Bir Kayıt Bulunamadı");
            }
            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal claimUser = HttpContext.User;
            string username = claimUser.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userService.GetUserByUsername(username);
            await userLogService.LogOut(user);
            return RedirectToAction("Login", "Account");
        }

    }
}
