using ComtradeApp.AppDbContext;
using ComtradeApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ProaktifArizaTahmini.CORE.Entities;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ComtradeApp.Repository
{
    /// <summary>
    /// BaseRepository<HistoryOfChange> sınıfından türetilmiş bir sınıftır  bu da BaseRepository sınıfının genel işlevselliğini miras alır ve özelleştirebilir.
    /// </summary>
    public class HistoryOfChangeRepository : BaseRepository<HistoryOfChange>
    {
        private readonly DbContextFactory dbContextFactory;

        public HistoryOfChangeRepository(DbContextFactory dbContextFactory) : base(dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }
        /// <summary>
        /// Bu metot, belirli bir RelayInformation nesnesinin kimlik (ID) numarasına göre HistoryOfChange nesnesini getirir. Veritabanındaki HistoryOfChanges tablosunu sorgular ve veriyi istenen sırayla sıralar. Sonrasında en yeni değişikliği içeren kaydı alır. Bu metot, geçmiş değişiklikleri kontrol etmek ve ilgili geçmiş değişikliğe erişmek için kullanılır.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Veritabanındaki bir RelayInformation nesnesinin belirli bir tarihçe değişikliğini getirir ve bu tarihçe değişikliği HistoryOfChange türünde döndürür. Eğer belirtilen id ile ilişkilendirilmiş bir tarihçe değişikliği yoksa, null dönebilir</returns>
        public async Task<HistoryOfChange> GetByRelayInformationId(int id)
        {
            using (var dbContext = dbContextFactory.CreateDbContext())
            {
                return await dbContext.HistoryOfChanges
                    .Where(x => x.RelayInformationId == id)
                    .OrderByDescending(x => x.ChangedDate)
                    .FirstOrDefaultAsync();
            }
        }
        /// <summary>
        /// Bu metot, RelayInformation nesnesi ve bir dosya yolu (comtradeFilesPath) alır. İlgili RelayInformation için tarihçe değişikliklerini kontrol eder, eski IP adreslerini ve yeni IP adreslerini alır ve bu değişikliklere göre dosya klasörlerini günceller. Özellikle IP adresi değiştiğinde, eski klasör adını yeni klasör adına taşır. Bu metot, dosyaların ve klasörlerin düzenlenmesi gerektiği senaryolarda kullanılır.
        /// </summary>
        /// <param name="relayInformation"></param>
        /// <param name="comtradeFilesPath"></param>
        /// <returns>Belirli bir RelayInformation nesnesinin klasör adlarını IP adresi değişikliklerine göre günceller, ancak kendisi herhangi bir değer döndürmez.</returns>
        public async Task UpdateFolderNames(RelayInformation relayInformation, string comtradeFilesPath)
        {
            using (var dbContext = dbContextFactory.CreateDbContext())
            {
                var ipChanges = await dbContext.HistoryOfChanges
                    .Where(x => x.RelayInformationId == relayInformation.ID)
                    .OrderBy(x => x.ChangedDate)
                    .ToListAsync();

                foreach (var item in ipChanges)
                {
                    string oldIP = item.OldIP;
                    string newIP = item.NewIP;
                    string oldFolderPath = Path.Combine(comtradeFilesPath, $"{oldIP}-{relayInformation.TmKvHucre}");
                    string newFolderPath = Path.Combine(comtradeFilesPath, $"{newIP}-{relayInformation.TmKvHucre}");

                    if (Directory.Exists(oldFolderPath))
                    {
                        Directory.Move(oldFolderPath, newFolderPath);
                    }
                }
            }
        }
        /// <summary>
        /// İkinci bir sürümü olan bu metot, bir Disturbance nesnesi ve bir CSV dosyaları yolu (csvFilesPath) alır. Benzer şekilde, tarihçe değişikliklerini kontrol eder ve dosya klasörlerini günceller, bu sefer Disturbance nesnesine göre. Özellikle arıza verileri için dosya düzenlemesi gerektiğinde kullanılır.
        /// </summary>
        /// <param name="disturbance"></param>
        /// <param name="csvFilesPath"></param>
        /// <returns>Belirli bir Disturbance nesnesinin klasör adlarını IP adresi değişikliklerine göre günceller, ancak kendisi herhangi bir değer döndürmez.</returns>
        public async Task UpdateFolderNames(Disturbance disturbance, string csvFilesPath)
        {
            using (var dbContext = dbContextFactory.CreateDbContext())
            {
                var ipChanges = await dbContext.HistoryOfChanges
                    .OrderBy(x => x.ChangedDate)
                    .ToListAsync();

                foreach (var item in ipChanges)
                {
                    string oldIP = item.OldIP;
                    string newIP = item.NewIP;
                    string oldFolderPath = Path.Combine(csvFilesPath, $"{oldIP}-{disturbance.TmKvHucre}");
                    string newFolderPath = Path.Combine(csvFilesPath, $"{newIP}-{disturbance.TmKvHucre}");

                    if (Directory.Exists(oldFolderPath))
                    {
                        Directory.Move(oldFolderPath, newFolderPath);
                    }
                }
            }
        }
    }
}
