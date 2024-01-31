using ComtradeApp.AppDbContext;
using ComtradeApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Serilog;
using System;
using System.Threading.Tasks;

namespace ComtradeApp.Repository
{
    /// <summary>
    /// Bu LogRepository sınıfı,uygulamanın loglama işlemlerini yönetmek için kullanılır.Üç farklı loglama işlemi sunar: hata (error), bilgi (information), ve uyarı (warning) loglaması. Uygulamanın çalışma süreçlerini izlemek ve hata tespiti yapmak için önemlidir. Loglama, uygulama hatalarını ve davranışlarını anlamak, sorunları tanımlamak ve gidermek için kullanılır.
    /// </summary>
    public class LogRepository
    {
        private readonly DbContextFactory dbContextFactory;

        public LogRepository(DbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }
        /// <summary>
        /// Bu metot, hata seviyesinde loglama yapar. Hata mesajı (message), hata detayları (exception), hizmet adı (serviceName), ve iş parçacığı kimliği (threadId) alır. Bu loglar genellikle uygulamada beklenmeyen hataların takibi için kullanılır.
        /// </summary>
        /// <param name="message">Hata mesajı.</param>
        /// <param name="exception">Hata detayları.</param>
        /// <param name="serviceName">Hizmet adı.</param>
        /// <param name="threadId">İş parçacığı kimliği.</param>
        /// <returns>Eğer kayıt başarılı bir şekilde eklenirse metot true dönecek, aksi takdirde false dönecektir.</returns>
        public async Task<bool> ErrorLog(string message, string exception, string serviceName, int threadId)
        {
            using (var dbContext = dbContextFactory.CreateDbContext())
            {
                ServiceLog log = new ServiceLog()
                {
                    LogLevel = "Error",
                    TimeStamp = DateTime.Now,
                    Message = message,
                    Exception = exception,
                    ServiceName = serviceName,
                    ThreadID = threadId
                };
                dbContext.ServiceLogs.Add(log);
                return await dbContext.SaveChangesAsync() > 0;
            }
        }
        /// <summary>
        /// Bu metot, bilgi seviyesinde loglama yapar. Bilgi mesajı (message), hizmet adı (serviceName), ve iş parçacığı kimliği (threadId) alır. Genellikle uygulamanın çalışma durumu hakkında bilgi vermek için kullanılır.
        /// </summary>
        /// <param name="message">Bilgi mesajı.</param>
        /// <param name="serviceName">Hizmet adı.</param>
        /// <param name="threadId">İş parçacığı kimliği.</param>
        /// <returns>Eğer kayıt başarılı bir şekilde eklenirse metot true dönecek, aksi takdirde false dönecektir.</returns>
        public async Task<bool> InformationLog(string message, string serviceName, int threadId)
        {
            using (var dbContext = dbContextFactory.CreateDbContext())
            {
                ServiceLog log = new ServiceLog()
                {
                    LogLevel = "Information",
                    TimeStamp = DateTime.Now,
                    Message = message,
                    Exception = null,
                    ServiceName = serviceName,
                    ThreadID = threadId
                };
                dbContext.ServiceLogs.Add(log);
                return await dbContext.SaveChangesAsync() > 0;
            }
        }
        /// <summary>
        /// Bu metot, uyarı seviyesinde loglama yapar. Uyarı mesajı (message), hizmet adı (serviceName), ve iş parçacığı kimliği (threadId) alır. Uygulamanın normal işleyişinde potansiyel sorunları işaret etmek için kullanılır.
        /// </summary>
        /// <param name="message">Uyarı mesajı.</param>
        /// <param name="serviceName">Hizmet adı.</param>
        /// <param name="threadId">İş parçacığı kimliği.</param>
        /// <returns>Eğer kayıt başarılı bir şekilde eklenirse metot true dönecek, aksi takdirde false dönecektir.</returns>
        public async Task<bool> WarningLog(string message, string serviceName, int threadId)
        {
            using (var dbContext = dbContextFactory.CreateDbContext())
            {
                ServiceLog log = new ServiceLog()
                {
                    LogLevel = "Warning",
                    TimeStamp = DateTime.Now,
                    Message = message,
                    Exception = null,
                    ServiceName = serviceName,
                    ThreadID = threadId
                };
                dbContext.ServiceLogs.Add(log);
                return await dbContext.SaveChangesAsync() > 0;
            }
        }
    }
}
