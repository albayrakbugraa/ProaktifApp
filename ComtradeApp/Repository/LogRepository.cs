using ComtradeApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComtradeApp.Repository
{
    public class LogRepository : BaseRepository<LogRepository>
    {
        private readonly AppDbContext.AppDbContext db;
        public LogRepository(AppDbContext.AppDbContext db) : base(db)
        {
            this.db = db;
        }
        public async Task<bool> ErrorLog(string message,string exception,string serviceName)
        {
            ServiceLog log = new ServiceLog()
            {
                LogLevel = "Error",
                TimeStamp = DateTime.Now,
                Message = message,
                Exception = exception,
                ServiceName = serviceName
            };
            db.ServiceLogs.Add(log);
            return await db.SaveChangesAsync()>0;
        }

        public async Task<bool> InformationLog(string message, string serviceName)
        {
            ServiceLog log = new ServiceLog()
            {
                LogLevel = "Information",
                TimeStamp = DateTime.Now,
                Message = message,
                Exception = null,
                ServiceName = serviceName
            };
            db.ServiceLogs.Add(log);
            return await db.SaveChangesAsync() > 0;
        }

        public async Task<bool> WarningLog(string message, string serviceName)
        {
            ServiceLog log = new ServiceLog()
            {
                LogLevel = "Warning",
                TimeStamp = DateTime.Now,
                Message = message,
                Exception = null,
                ServiceName = serviceName
            };
            db.ServiceLogs.Add(log);
            return await db.SaveChangesAsync() > 0;
        }

    }
}
