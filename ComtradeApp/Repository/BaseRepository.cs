using ComtradeApp.AppDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ComtradeApp.Repository
{
    /// <summary>
    ///  Entity Framework Core veritabanı bağlamı ile çalışan genel (generic) bir veritabanı işlemci sınıfını temsil eder. Bu sınıf, CRUD (Oluştur, Oku, Güncelle, Sil) işlemlerini gerçekleştirmek için kullanılır ve genel bir yapıya sahiptir, bu nedenle farklı veri türleri için kullanılabilir.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseRepository<T> where T : class
    {
        private readonly DbContextFactory dbContextFactory;

        public BaseRepository(DbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }
        /// <summary>
        /// Belirli bir koşulu sağlayan bir öğe olup olmadığını kontrol eder. Örneğin, veritabanında bir koşulu karşılayan herhangi bir öğenin olup olmadığını belirlemek için kullanılabilir.
        /// </summary>
        /// <param name="expression">Belirli bir koşulu ifade eden bir işlev veya lambda ifadesi olmalıdır. </param>
        /// <returns>
        /// Eğer koşulu karşılayan bir öğe varsa, yani en az bir öğe bu koşulu sağlıyorsa, metot true döner.
        /// Eğer koşulu karşılayan hiçbir öğe yoksa, yani hiçbir öğe bu koşulu sağlamıyorsa, metot false döner.
        /// </returns>
        public async Task<bool> Any(Expression<Func<T, bool>> expression)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                return await context.Set<T>().AnyAsync(expression);
            }
        }
        /// <summary>
        ///  Yeni bir varlık oluşturur ve bu varlığı veritabanına ekler.
        /// </summary>
        /// <param name="entity">Veritabanı tablosundaki veya bir veri modelindeki bir nesneyi ifade eder.</param>
        /// <returns>İşlemin başarılı olup olmadığını belirten bir boolean değerdir</returns>
        public async Task<bool> Create(T entity)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                context.Set<T>().Add(entity);
                return await context.SaveChangesAsync() > 0;
            }
        }
        /// <summary>
        /// Belirli bir varlığı veritabanından siler.
        /// </summary>
        /// <param name="entity">Veritabanı tablosundaki veya bir veri modelindeki bir nesneyi ifade eder.</param>
        /// <returns> Silme işlemi başarılıysa true, aksi takdirde false döndürür.</returns>
        public bool Delete(T entity)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                context.Set<T>().Remove(entity);
                return context.SaveChanges() > 0;
            }
        }
        /// <summary>
        /// Tüm veritabanı öğelerini getirir.
        /// </summary>
        /// <returns> Bir liste olarak döndürür</returns>
        public async Task<List<T>> GetAll()
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                return await context.Set<T>().ToListAsync();
            }
        }
        /// <summary>
        /// Bu metod, belirli bir koşulu karşılayan öğeler arasından yalnızca ilk öğeyi (veya koşulu sağlayan ilk öğeyi) seçer.Bu metod, belirli bir sorgu sonucunda yalnızca bir öğeyi almak istediğinizde kullanışlıdır.
        /// </summary>
        /// <typeparam name="TResult">Bir metotun dönüş değeri veya bir sınıfın içeriği için kullanılır.</typeparam>
        /// <param name="selector">Öğelerin hangi alanlarının seçileceğini belirler. Örneğin, bir nesnenin Name ve Age alanlarını seçmek için x => new { x.Name, x.Age } şeklinde bir ifade kullanılabilir.</param>
        /// <param name="expression">Öğelerin seçimi için bir filtre koşulu belirler. Yalnızca bu koşulu karşılayan öğeler seçilir.</param>
        /// <param name="orderBy">Sonuçların nasıl sıralanacağını belirler. Örneğin, sonuçları belirli bir sıralamada almak için kullanılabilir.</param>
        /// <param name="includes">İlişkisel verilerin yüklenip yüklenmeyeceğini belirler. Örneğin, öğelerle ilişkili başka nesneleri yüklemek için kullanılabilir.</param>
        /// <returns>Döndürülen sonuç, TResult olarak ifade edilen bir generic türdür ve yalnızca tek bir öğeyi temsil eder.</returns>
        public async Task<TResult> GetFilteredFirstOrDefault<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> expression, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                IQueryable<T> query = context.Set<T>();
                if (includes != null) query = includes(query);
                if (expression != null) query = query.Where(expression);
                if (orderBy != null) return await orderBy(query).Select(selector).FirstOrDefaultAsync();
                else return await query.Select(selector).FirstOrDefaultAsync();
            }
        }
        /// <summary>
        /// Bu metod, belirli bir koşulu karşılayan tüm öğeleri seçer ve bir liste olarak döndürür.Bu metod, belirli bir koşulu karşılayan tüm öğeleri almak ve bu öğeleri bir liste olarak işlemek istediğinizde kullanışlıdır.
        /// </summary>
        /// <typeparam name="TResult">Bir metotun dönüş değeri veya bir sınıfın içeriği için kullanılır.</typeparam>
        /// <param name="selector">Öğelerin hangi alanlarının seçileceğini belirler. Örneğin, bir nesnenin Name ve Age alanlarını seçmek için x => new { x.Name, x.Age } şeklinde bir ifade kullanılabilir.</param>
        /// <param name="expression">Öğelerin seçimi için bir filtre koşulu belirler. Yalnızca bu koşulu karşılayan öğeler seçilir.</param>
        /// <param name="orderBy">Sonuçların nasıl sıralanacağını belirler. Örneğin, sonuçları belirli bir sıralamada almak için kullanılabilir.</param>
        /// <param name="includes">İlişkisel verilerin yüklenip yüklenmeyeceğini belirler. Örneğin, öğelerle ilişkili başka nesneleri yüklemek için kullanılabilir.</param>
        /// <returns>Döndürülen sonuç, List<TResult> olarak ifade edilen bir generic liste türünü temsil eder ve belirli bir koşulu karşılayan tüm öğeleri içerir.</returns>
        public async Task<List<TResult>> GetFilteredList<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> expression, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                IQueryable<T> query = context.Set<T>();
                if (includes != null) query = includes(query);
                if (expression != null) query = query.Where(expression);
                if (orderBy != null) return await orderBy(query).Select(selector).ToListAsync();
                else return await query.Select(selector).ToListAsync();
            }
        }
        /// <summary>
        /// Belirli bir koşulu karşılayan bir öğeyi veya nesneyi veritabanından çekmek için kullanılır.
        /// T generic türü, veritabanındaki bir tabloyu veya nesneyi temsil eder. 
        /// </summary>
        /// <param name="expression">Öğelerin seçimi için bir filtre koşulu belirler. Yalnızca bu koşulu karşılayan öğeler seçilir.</param>
        /// <param name="includes">Veritabanı ilişkilendirmelerini (joins) yapmak için kullanılır. Özellikle, ilişkilendirilmiş nesnelerin özelliklerini çekmek istediğinizde bu parametre kullanışlıdır.</param>
        /// <returns>Seçilen öğe (veya öğeler) döndürülür. Bu, belirtilen koşulu karşılayan öğeyi veya öğeleri içeren bir nesne veya liste olarak geri döner.</returns>
        public async Task<T> GetWhere(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                var query = context.Set<T>().AsQueryable();
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
                return await query.FirstOrDefaultAsync(expression);
            }
        }
        /// <summary>
        /// Bir veritabanı tablosundaki (veya veri modelindeki) belirli bir varlığı güncellemek veya değiştirmek için kullanılan bir metoddur. 
        /// </summary>
        /// <param name="entity">Veritabanı tablosundaki veya bir veri modelindeki bir nesneyi ifade eder.</param>
        /// <returns> Güncelleme işlemi başarılıysa true, aksi takdirde false döndürür.</returns>
        public bool Update(T entity)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                context.Set<T>().Update(entity);
                return context.SaveChanges() > 0;
            }
        }
    }
}
