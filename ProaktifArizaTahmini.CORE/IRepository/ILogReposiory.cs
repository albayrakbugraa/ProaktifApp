using Microsoft.EntityFrameworkCore;
using ProaktifArizaTahmini.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.CORE.IRepository
{
    public interface ILogReposiory : IBaseRepository<ServiceLog>
    {
        Task<List<string>> GetUniqueLogLevels();        
        Task<List<string>> GetUniqueServiceNames();        
    }
}
