using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.BLL.Models.RequestModel
{
    public class LoginModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public bool RememberMe { get; set; }
        public LoginResult LoginResult { get; set; }
    }
    public enum LoginResult
    {
        LoginSuccess,
        LoginFailed,
        WaitingActivated,

    }
}

