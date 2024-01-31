using ComtradeApp.AppDbContext;
using ComtradeApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComtradeApp.Repository
{
    /// <summary>
    /// Bu sınıf, BaseRepository<Disturbance> sınıfından türetilmiştir, bu da BaseRepository sınıfının genel işlevselliğini miras alır ve özelleştirebilir.
    /// </summary>
    public class DisturbanceRepository : BaseRepository<Disturbance>
    {
        private readonly DbContextFactory dbContextFactory;

        public DisturbanceRepository(DbContextFactory dbContextFactory) : base(dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }
        /// <summary>
        /// Bu metod, belirli bir SFTP (Secure File Transfer Protocol) durumuna göre "Disturbance" kayıtlarını getirir. sftpStatus adlı bir parametre alır, bu parametre ile filtreleme yapar ve veritabanındaki ilgili kayıtları seçer. Sonuç olarak, bir liste olarak dönen bu kayıtları döndürür.
        /// </summary>
        /// <param name="sftpStatus">Bu parametre true ise, SFTP durumu doğrulandı anlamına gelir ve true olan kayıtları getirecektir. Aynı şekilde, false ise SFTP durumu doğrulanmadı anlamına gelir ve false olan kayıtları getirecektir.</param>
        /// <returns>Bir liste olarak kayıtları döndürür.</returns>
        public List<Disturbance> GetBySftpStatus(bool sftpStatus)
        {
            using (var dbContext = dbContextFactory.CreateDbContext())
            {
                return dbContext.Disturbances.Where(d => d.sFtpStatus == sftpStatus).ToList();
            }
        }
    }
}
