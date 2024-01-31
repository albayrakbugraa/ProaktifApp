using ProaktifArizaTahmini.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.BLL.Services.UserServices
{
    /// <summary>
    /// User Services için bir arayüz (interface) olan IUserService'yi tanımlar. 
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Bu metot, verilen kullanıcı adının veritabanında mevcut olup olmadığını kontrol eder. 
        /// </summary>
        /// <param name="username">Metoda gelen kullanıcı adını temsil eder.</param>
        /// <returns>True, kullanıcı adının mevcut olduğunu; false, kullanıcı adının mevcut olmadığını gösterir.</returns>
        Task<bool> CheckUserByUsername(string username);

        /// <summary>
        /// Veritabanındaki bir kullanıcının bilgilerini güncellemek için kullanılır. 
        /// </summary>
        /// <param name="user">>Güncellenmiş bir kullanıcıyı temsil eden User nesnesidir. </param>
        /// <returns>İşlem başarıyla tamamlandığında true, aksi takdirde false değeri döndürülür.</returns>
        Task<bool> UpdateUser(User user);

        /// <summary>
        /// Yeni bir kullanıcı oluşturmak için kullanılır. 
        /// </summary>
        /// <param name="domainUser">Yeni bir kullanıcı oluşturulurken kullanılan kullanıcı bilgilerini temsil eden bir User türünden nesnedir. </param>
        /// <returns>İşlem başarıyla tamamlandığında true, aksi takdirde false değeri döndürülür.</returns>
        Task<bool> CreateUser(User domainUser);

        /// <summary>
        ///  Vveritabanından bir kullanıcı hesabını silmek için kullanılır. 
        /// </summary>
        /// <param name="ID">Silinecek kullanıcının kimliğini (ID) temsil eder.</param>
        /// <returns>İşlem başarıyla tamamlandığında true, aksi takdirde false değeri döndürülür.</returns>
        Task<bool> DeleteUser(int ID);

        /// <summary>
        /// Verilen kullanıcı adına sahip bir kullanıcıyı veritabanından almak için kullanılır. 
        /// </summary>
        /// <param name="username">Kullanıcı adını temsil eden bir dize (string) alır.</param>
        /// <returns>Bulunan User nesnesi geri döndürülür.</returns>
        Task<User> GetUserByUsername(string username);
    }
}
