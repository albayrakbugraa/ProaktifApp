using ProaktifArizaTahmini.CORE.Entities;
using ProaktifArizaTahmini.CORE.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.BLL.Services.UserLogServices
{
    public class UserLogService : IUserLogService
    {
        private readonly IUserLogRepository userLogRepository;

        public UserLogService(IUserLogRepository userLogRepository)
        {
            this.userLogRepository = userLogRepository;
        }

        public async Task<bool> CreateData(User user)
        {
            var log = new UserLog()
            {
                User = user,
                UserId = user.ID,
                LogDate = DateTime.Now,
                MethodName = "Yeni Veri Girişi",
                Name = user.Name,
                Surname = user.Surname,
                Username = user.Username
            };
            return await userLogRepository.Create(log);
        }

        public async Task<bool> DeleteData(User user)
        {
            var log = new UserLog()
            {
                User = user,
                UserId = user.ID,
                LogDate = DateTime.Now,
                MethodName = "Veri Silme",
                Name = user.Name,
                Surname = user.Surname,
                Username = user.Username
            };
            return await userLogRepository.Create(log);
        }

        public async Task<bool> ImportExcel(User user)
        {
            var log = new UserLog()
            {
                User = user,
                UserId = user.ID,
                LogDate = DateTime.Now,
                MethodName = "Excel Verileri Yükleme",
                Name = user.Name,
                Surname = user.Surname,
                Username = user.Username
            };
            return await userLogRepository.Create(log);
        }

        public async Task<bool> LogIn(User user)
        {
            var log = new UserLog()
            {
                User = user,
                UserId = user.ID,
                LogDate = DateTime.Now,
                MethodName = "Kullanıcı Girişi",
                Name = user.Name,
                Surname = user.Surname,
                Username = user.Username
            };
            return await userLogRepository.Create(log);
        }

        public async Task<bool> LogOut(User user)
        {
            var log = new UserLog()
            {
                User = user,
                UserId = user.ID,
                LogDate = DateTime.Now,
                MethodName = "Kullanıcı Çıkışı",
                Name = user.Name,
                Surname = user.Surname,
                Username = user.Username
            };
            return await userLogRepository.Create(log);
        }

        public async Task<bool> UpdateData(User user)
        {
            var log = new UserLog()
            {
                User = user,
                UserId = user.ID,
                LogDate = DateTime.Now,
                MethodName = "Veri Güncelleme",
                Name = user.Name,
                Surname = user.Surname,
                Username = user.Username
            };
            return await userLogRepository.Create(log);
        }
    }
}
