using ProaktifArizaTahmini.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.BLL.Services.UserLogServices
{
    /// <summary>
    /// UserLog Services için bir arayüz (interface) tanımlar. 
    /// </summary>
    public interface IUserLogService
    {
        /// <summary>
        /// Bu metot, bir kullanıcının giriş (login) işlemini kaydetmeyi amaçlar. Yani, kullanıcının oturum açma işlemi gerçekleştirdiğinde bu log kaydı oluşturulur.
        /// </summary>
        /// <param name="user">User nesnesi alır. Bu nesne, giriş yapan kullanıcıyı temsil eder.</param>
        /// <returns>İşlem başarıyla tamamlandığında true, aksi takdirde false değeri döndürülür.</returns>
        Task<bool> LogIn(User user);

        /// <summary>
        /// Bu metot, bir kullanıcının oturumunu kapatma (logout) işlemini kaydetmeyi amaçlar. Yani, kullanıcı oturumunu kapatıp çıkış yaptığında bu log kaydı oluşturulur.
        /// </summary>
        /// <param name="user">User nesnesi alır. Bu nesne, oturumu kapatmış olan kullanıcıyı temsil eder.</param>
        /// <returns>Bu nesne, oturumu kapatmış olan kullanıcıyı temsil eder.</returns>
        Task<bool> LogOut(User user);

        /// <summary>
        /// Bu metot, veri oluşturma işlemini kaydetmeyi amaçlar. Yani, bir kullanıcı yeni veri oluşturduğunda bu log kaydı oluşturulur.
        /// </summary>
        /// <param name="user">User nesnesi alır. Bu nesne, veri oluşturan kullanıcıyı temsil eder.</param>
        /// <returns>İşlem başarıyla tamamlandığında true, aksi takdirde false değeri döndürülür.</returns>
        Task<bool> CreateData(User user);

        /// <summary>
        /// Bu metot, veri güncelleme işlemini kaydetmeyi amaçlar. Yani, bir kullanıcı mevcut veriyi güncellediğinde bu log kaydı oluşturulur.
        /// </summary>
        /// <param name="user">User nesnesi alır. Bu nesne, veriyi güncelleyen kullanıcıyı temsil eder.</param>
        /// <returns>İşlem başarıyla tamamlandığında true, aksi takdirde false değeri döndürülür.</returns>
        Task<bool> UpdateData(User user);

        /// <summary>
        /// Bu metot, veri silme işlemini kaydetmeyi amaçlar. Yani, bir kullanıcı mevcut veriyi sildiğinde bu log kaydı oluşturulur.
        /// </summary>
        /// <param name="user">User nesnesi alır. Bu nesne, veriyi silen kullanıcıyı temsil eder.</param>
        /// <returns>İşlem başarıyla tamamlandığında true, aksi takdirde false değeri döndürülür.</returns>
        Task<bool> DeleteData(User user);

        /// <summary>
        /// Bu metot, Excel dosyasını içe aktarma işlemini kaydetmeyi amaçlar. Yani, bir kullanıcı Excel dosyasını uygulamaya içe aktardığında bu log kaydı oluşturulur.
        /// </summary>
        /// <param name="user">User nesnesi alır. Bu nesne, Excel dosyası içe aktaran kullanıcıyı temsil eder.</param>
        /// <returns>İşlem başarıyla tamamlandığında true, aksi takdirde false değeri döndürülür.</returns>
        Task<bool> ImportExcel(User user);

        /// <summary>
        /// Bu metot, uygulama içinde meydana gelen hataları ve istisnai durumları kaydetmeyi amaçlar. Hata türü, metot adı ve hata mesajı gibi bilgiler bu log kaydına dahil edilir.
        /// </summary>
        /// <param name="user">Bu parametre hangi kullanıcıyla ilişkilendirilmesi gerektiğini belirtir.</param>
        /// <param name="exception">Hata mesajı atanır, bu mesaj genellikle istisnai durumların ayrıntılarını içerir.</param>
        /// <param name="method">Hatanın meydana geldiği metodun adı atanır.</param>
        /// <param name="message">Hata ile ilgili mesaj atanır, bu mesaj hatanın açıklamasını içerir.</param>
        /// <returns>İşlem başarıyla tamamlandığında true, aksi takdirde false değeri döndürülür.</returns>
        Task<bool> ErrorLog(User user, string exception, string method, string message);

        /// <summary>
        /// Bu metot, uygulama içinde belirli olayları ve işlemleri kaydetmek ve izlemek için kullanılır.
        /// </summary>
        /// <param name="user">Bu parametre hangi kullanıcıyla ilişkilendirilmesi gerektiğini belirtir.</param>
        /// <param name="method"> Logun hangi metod tarafından oluşturulduğu atanır.</param>
        /// <param name="message">Log mesajı atanır, bu mesaj bilgi logunun açıklamasını içerir.</param>
        /// <returns>İşlem başarıyla tamamlandığında true, aksi takdirde false değeri döndürülür.</returns>
        Task<bool> InformationLog(User user, string method, string message);

    }
}
