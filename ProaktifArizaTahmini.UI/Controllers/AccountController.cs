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

namespace ProaktifArizaTahmini.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService userService;

        public AccountController(IUserService userService)
        {
            this.userService = userService;
        }

        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var cryptPassword = Encryption.Encrypt(loginModel.Password);
            if (new ActiveDirectory().IsAuthenticate(loginModel))
            {
                var user = await userService.GetUser(loginModel.Username, cryptPassword);
                if (user != null)
                {
                    if (user.IsActive == null || !user.IsActive.Value)
                    {
                        ModelState.AddModelError("State", "Kullanıcınızın Aktifleştirilmesi İçin Gerekli İşlem Yapıldı En Kısa Sürede Size Dönüş Yapılacaktır");
                        loginModel.LoginResult = LoginResult.WaitingActivated;
                        return View();
                    }
                    user.LastLoginDate = DateTime.Now;
                    bool result = await userService.UpdateUser(user);
                    loginModel.LoginResult = LoginResult.LoginSuccess;

                    List<Claim> claims = new List<Claim>() {
                    new Claim(ClaimTypes.NameIdentifier, loginModel.Username)
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

                    return RedirectToAction("Index", "Home");
                }
                else if (await userService.GetChangedUser(loginModel.Username, loginModel.Password) != null)
                {
                    var changedUser = await userService.GetChangedUser(loginModel.Username, loginModel.Password);
                    if (changedUser.IsActive == null || !changedUser.IsActive.Value)
                    {
                        ModelState.AddModelError("State",
                            "Kullanıcınızın Aktifleştirilmesi İçin Gerekli İşlem Yapıldı En Kısa Sürede Size Dönüş Yapılacaktır");
                        loginModel.LoginResult = LoginResult.WaitingActivated;
                        return View();
                    }
                    changedUser.Password = cryptPassword;//user.Password;
                    changedUser.LastLoginDate = DateTime.Now;
                    bool result = await userService.UpdateUser(changedUser);
                    loginModel.LoginResult = LoginResult.LoginSuccess;

                    List<Claim> claims = new List<Claim>() {
                    new Claim(ClaimTypes.NameIdentifier, loginModel.Username)
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

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    if (!await userService.CheckUserByUsername(loginModel.Username))
                    {
                        //İlk Kez Giriş Yapıldığı İçin Yeni Bir Kullanıcı Oluşturuyoruz
                        var domainUser = new ActiveDirectory().IsActiveDirectory(loginModel);
                        domainUser.IsActive = false;
                        domainUser.UserTypeId = (int?)UserTypeNames.Domain;
                        domainUser.Password = cryptPassword;
                        await userService.CreateUser(domainUser);
                        loginModel.LoginResult = LoginResult.WaitingActivated;
                        ModelState.AddModelError("State", "Kullanıcınızın Aktifleştirilmesi İçin Gerekli İşlem Yapıldı En Kısa Sürede Size Dönüş Yapılacaktır");
                        return View();
                    }
                }
            }
            else
            {
                //Domainde bu kullanıcı yoksa ve sistemde varsa bu kullanıcıyı siliyoruz
                var user = await userService.GetUser(loginModel.Username, cryptPassword);
                if (user != null)
                {
                    if (user.UserTypeId != (int?)UserTypeNames.Domain)
                    {
                        if (ModelState.IsValid)
                        {
                            if (user.IsActive == null || !user.IsActive.Value)
                            {
                                ModelState.AddModelError("State", "Kullanıcınızın Aktifleştirilmesi İçin Gerekli İşlem Yapıldı En Kısa Sürede Size Dönüş Yapılacaktır");
                                loginModel.LoginResult = LoginResult.WaitingActivated;
                                return View();
                            }
                            else
                            {
                                user.Password = cryptPassword;
                                user.LastLoginDate = DateTime.Now;
                                bool result = await userService.UpdateUser(user);
                                loginModel.LoginResult = LoginResult.LoginSuccess;
                                List<Claim> claims = new List<Claim>() {
                                    new Claim(ClaimTypes.NameIdentifier, loginModel.Username)
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

                                return RedirectToAction("Index", "Home");
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
            return RedirectToAction("Login", "Account");
        }
        
    }
}
