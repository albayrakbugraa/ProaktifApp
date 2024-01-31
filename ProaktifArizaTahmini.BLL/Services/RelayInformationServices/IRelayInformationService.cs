using ProaktifArizaTahmini.BLL.Models.DTOs;
using ProaktifArizaTahmini.BLL.Models.RequestModel;
using ProaktifArizaTahmini.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.BLL.Services.RelayInformationServices
{
    /// <summary>
    /// RelayInformation Service için bir arayüz (interface) tanımlar.
    /// </summary>
    public interface IRelayInformationService
    {
        /// <summary>
        /// Mevcut röle bilgilerinin bir listesini döndürmeyi amaçlar.
        /// </summary>
        /// <returns>Röle bilgilerinin tamamını liste olarak döner.</returns>
        Task<List<RelayInformation>> GetRelayInformations();

        /// <summary>
        /// Eklenen verileri kontrol eder, belirli koşullara göre işlemler gerçekleştirir, bu işlemleri raporlar ve üç farklı liste kullanarak işlem sonuçlarını döner. Bu işlem, röle bilgilerinin toplu bir şekilde eklenmesi ve veri bütünlüğünün korunması için kullanılabilir.
        /// </summary>
        /// <param name="relayInformationList">RelayInformation türünden oluşan bir liste (List) dir. Bu listede röle bilgilerinin bulunduğu nesneler bulunur.</param>
        /// <param name="user">User türünden bir kullanıcı nesnesini temsil eder. Bu kullanıcı, işlemin kim tarafından yapıldığını belirlemek ve ilgili kullanıcıya ait hata ve bilgi logları oluşturmak için kullanılır.</param>
        /// <returns>DuplicateRelays (Tekrarlayan röleler), IncompatibleRelays (Uyumsuz röleler) ve BlankRowRelays (boş satırlı röleler).</returns>
        Task<(List<RelayInformation> duplicateRelays, List<RelayInformation> incompatibleRelays, List<RelayInformation> blankRowRelays)> AddDataList(List<RelayInformation> relayInformationList, User user);

        /// <summary>
        /// Bu metot, belirli bir tMkVHucre değerine sahip röle bilgisini döndürmeyi amaçlar.
        /// </summary>
        /// <param name="tMkVHucre">Röleye ait TmKvHücre.</param>
        /// <returns>Bir RelayInformation nesnesi döner.</returns>
        Task<RelayInformation> GetRelayInformationWhere(string tMkVHucre);

        /// <summary>
        /// Belirli bir röle bilgisini kimlik (ID) değerine göre almayı amaçlar.
        /// </summary>
        /// <param name="id">İstenen Rölenin kimliğini temsil eder.</param>
        /// <returns>Bir RelayInformation nesnesi döner.</returns>
        Task<RelayInformation> GetRelayInformationById(int id);

        /// <summary>
        /// Belirli bir röle bilgisini güncellemeyi amaçlar.
        /// </summary>
        /// <param name="id">Güncellenecek Rölenin kimliğini temsil eder.</param>
        /// <param name="model">Röle bilgisini güncellemek için kullanılacak yeni verileri içerir.</param>
        /// <returns>İşlem başarıyla tamamlandığında true, aksi takdirde false değeri döndürülür.</returns>
        Task<bool> UpdateRelayInformation(int id, RelayInformationDTO model);

        /// <summary>
        /// Yeni bir röle bilgisi oluşturmayı amaçlar.
        /// </summary>
        /// <param name="model">Röle bilgisini oluşturmak için kullanılacak yeni verileri içerir.</param>
        /// <returns>İşlem başarıyla tamamlandığında true, aksi takdirde false değeri döndürülür.</returns>
        Task<bool> CreateRelayInformation(RelayInformationDTO model);

        /// <summary>
        /// Belirli bir röle bilgisini silmeyi amaçlar.
        /// </summary>
        /// <param name="id">Silenecek rölenin kimliğini temsil eder.</param>
        /// <returns>İşlem başarıyla tamamlandığında true, aksi takdirde false değeri döndürülür.</returns>
        Task<bool> DeleteRelayInformation(int id);

        /// <summary>
        /// Bu metot, filterParams parametresine göre filtrelenmiş röle bilgilerinin bir listesini döndürmeyi amaçlar. Filtreleme kriterleri, filterParams nesnesi içinde tanımlanır.
        /// </summary>
        /// <param name="filterParams">Bu parametre, röle bilgilerini belirli kriterlere göre filtrelemek için kullanılır.</param>
        /// <returns>Filtrelenmiş röle bilgilerinin bir listesini döndürür.</returns>
        Task<List<RelayInformation>> FilterList(RelayInformationFilterParams filterParams);
    }
}
