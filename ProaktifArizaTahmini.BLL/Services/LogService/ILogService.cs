using Microsoft.EntityFrameworkCore;
using ProaktifArizaTahmini.BLL.Models.RequestModel;
using ProaktifArizaTahmini.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.BLL.Services.LogService
{
    public interface ILogService
    {
        Task<List<ServiceLog>> GetServiceLogs();
        Task<List<ServiceLog>> FilterList(LogModel logModel);
        Task<List<string>> GetLogLevels();
        Task<List<string>> GetServiceNames();
        
    }
}
