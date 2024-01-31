using ComtradeApp.AppDbContext;
using ComtradeApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;

namespace ComtradeApp.Repository
{
    /// <summary>
    /// BaseRepository<RelayInformation> sınıfından türetilmiş bir sınıftır. Bu sınıf, RelayInformation türünden nesnelerin veritabanı işlemlerini yapmak için kullanılır.
    /// </summary>
    public class RelayInformationRepository : BaseRepository<RelayInformation>
    {
        private readonly DbContextFactory dbContextFactory;

        public RelayInformationRepository(DbContextFactory dbContextFactory) : base(dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }
        /// <summary>
        /// Bu metot, veritabanında RelayInformation nesnelerini, belirli bir durum (true veya false) ile filtreleyerek alır. Yani status parametresi ile belirtilen durumu taşıyan nesneleri çeker ve bunları bir liste olarak döndürür.
        /// </summary>
        /// <param name="status">Bir mantıksal değeri (true veya false) temsil eden bir parametredir. Bu parametre, RelayInformation nesnelerinin durumunu belirtir. Genellikle "aktif" veya "pasif" gibi durumları temsil etmek için kullanılır.</param>
        /// <returns>Asenkron bir metottur ve sonucu List<RelayInformation> türünde döndürür.</returns>
        public async Task<List<RelayInformation>> GetByStatus(bool status)
        {
            using (var dbContext = dbContextFactory.CreateDbContext())
            {
                return await dbContext.RelayInformations
                    .Where(d => d.Status == status)
                    .ToListAsync();
            }
        }
        /// <summary>
        /// Bu metot, belirli bir durumu taşıyan RelayInformation nesnelerini parçalara böler. Özellikle büyük bir veri kümesini daha küçük parçalara bölmek gerektiğinde kullanışlıdır. Status parametresi ile belirtilen durumu taşıyan nesneleri çeker ve bu nesneleri belirtilen sayıda parçaya böler. Her iç içe geçmiş liste, bir parçayı temsil eder ve bu parçaların toplam sayısı, numberOfChunks parametresi tarafından belirlenir.
        /// </summary>
        /// <param name="status">Bir mantıksal değeri (true veya false) temsil eden bir parametredir. Bu parametre, RelayInformation nesnelerinin durumunu belirtir. Genellikle "aktif" veya "pasif" gibi durumları temsil etmek için kullanılır.</param>
        /// <param name="numberOfChunks">Nesnelerin kaç parçaya bölüneceğini belirler.Her parça, bir List<RelayInformation> içinde bulunur ve bu parçaları içeren bir List döndürür.</param>
        /// <returns>Bu metot da asenkron bir metottur ve sonucu List<List<RelayInformation>> türünde döndürür.</returns>
        public async Task<List<List<RelayInformation>>> GetByStatusInChunks(bool status, int numberOfChunks)
        {
            using (var dbContext = dbContextFactory.CreateDbContext())
            {
                var relayList = await dbContext.RelayInformations
                    .Where(d => d.Status == status)
                    .ToListAsync();

                int totalItems = relayList.Count;
                int itemsPerChunk = totalItems / numberOfChunks;
                int remainder = totalItems % numberOfChunks;

                List<List<RelayInformation>> chunks = new List<List<RelayInformation>>();

                int currentIndex = 0;
                for (int i = 0; i < numberOfChunks; i++)
                {
                    int chunkSize = itemsPerChunk + (i < remainder ? 1 : 0);
                    List<RelayInformation> chunk = relayList.Skip(currentIndex).Take(chunkSize).ToList();
                    chunks.Add(chunk);
                    currentIndex += chunkSize;
                }

                return chunks;
            }
        }
    }
}
