using ProaktifArizaTahmini.UI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.BLL.Models.RequestModel
{
    public class LoginModel
    {
        [Display(Name = "Kullanıcı Adı")]
        [Required(ErrorMessage = "Kullanıcı adı boş bırakılamaz.")]
        public string Username { get; set; }

        [Display(Name = "Şifre")]
        [Required(ErrorMessage = "Şifre boş bırakılamaz.")]
        public string Password { get; set; }
        public string Domain { get; set; }
        public bool RememberMe { get; set; }
        public List<string> Groups { get; set; }
        public List<DomainGroup> DomainGroups { get; set; }
        public LoginResult LoginResult { get; set; }
    }
    public enum LoginResult
    {
        LoginSuccess,
        LoginFailed,
        WaitingActivated,

    }
}

