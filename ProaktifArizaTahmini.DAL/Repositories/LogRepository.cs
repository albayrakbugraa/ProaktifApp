using Microsoft.EntityFrameworkCore;
using ProaktifArizaTahmini.CORE.Entities;
using ProaktifArizaTahmini.CORE.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.DAL.Repositories
{
    public class LogRepository : BaseRepository<ServiceLog> , ILogReposiory
    {
        private readonly AppDbContext db;
        public LogRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public async Task<List<string>> GetUniqueLogLevels()
        {
            return await db.ServiceLogs.Select(log => log.LogLevel).Distinct().ToListAsync();
        }

        public async Task<List<string>> GetUniqueServiceNames()
        {
            return await db.ServiceLogs.Select(log => log.ServiceName).Distinct().ToListAsync();
        }
    }
}
