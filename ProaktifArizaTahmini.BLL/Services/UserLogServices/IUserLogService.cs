using ProaktifArizaTahmini.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.BLL.Services.UserLogServices
{
    public interface IUserLogService
    {
        Task<bool> LogIn(User user);
        Task<bool> LogOut(User user);
        Task<bool> CreateData(User user);
        Task<bool> UpdateData(User user);
        Task<bool> DeleteData(User user);
        Task<bool> ImportExcel(User user);

    }
}
