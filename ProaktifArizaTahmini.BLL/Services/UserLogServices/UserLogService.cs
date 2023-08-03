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

    }
}
