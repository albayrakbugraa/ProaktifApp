using ProaktifArizaTahmini.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.BLL.Services.UserServices
{
    public interface IUserService
    {
        Task<bool> CheckUserByUsername(string username);
        Task<User> GetChangedUser(string username, string password);
        Task<User> GetUser(string username,string password);
        Task<bool> UpdateUser(User user);
        Task<bool> CreateUser(User domainUser);
        Task<bool> DeleteUser(int ID);
        Task<User> GetUserByUsername(string username);
    }
}
