using ProaktifArizaTahmini.BLL.Models.DTOs;
using ProaktifArizaTahmini.BLL.Models.RequestModel;
using ProaktifArizaTahmini.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.BLL.Services.DisturbanceServices
{
    /// <summary>
    ///  Disturbance service için bir arayüz (interface) tanımlar. 
    /// </summary>
    public interface IDisturbanceService
    {
        /// <summary>
        /// Arızaların bir listesini almayı amaçlar.
        /// </summary>
        /// <returns>List<Disturbance> türünde bir dizi döndürür.</returns>
        Task<List<Disturbance>> GetDisturbances();

        /// <summary>
        /// Belirli bir filtreleme parametresine göre arızaların bir listesini almayı hedefler. 
        /// </summary>
        /// <param name="filterParams">Bu parametre, arızaları belirli kriterlere göre filtrelemek için kullanılır.</param>
        /// <returns>List<Disturbance> türünde bir dizi olarak döndürür.</returns>
        Task<List<Disturbance>> FilterList(DisturbanceFilterParams filterParams);

        /// <summary>
        /// Belirli bir RelayInformationDTO türünde veri ile ilgili arıza verilerini güncellemeyi amaçlar. 
        /// </summary>
        /// <param name="relayInformation">Bu parametre, güncellenecek arıza verilerini içerir.</param>
        /// <returns> İşlem sonucunu boolean değeri döndürür.</returns>
        Task<bool> UpdateByDataIdList(RelayInformationDTO relayInformation);

        /// <summary>
        /// Belirli bir kimlik (ID) değerine göre yapılandırma dosyasının içeriğini almayı hedefler. 
        /// </summary>
        /// <param name="id"> Bu parametre, istenen yapılandırma dosyasının kimliğini temsil eder.</param>
        /// <returns>Bir metin dizesi (string) olarak döndürür.</returns>
        Task<string> GetcfgFile(int id);

        /// <summary>
        /// Belirli bir kimlik (ID) değerine göre veri dosyasının içeriğini almayı amaçlar.
        /// </summary>
        /// <param name="id"> Bu parametre, istenen veri dosyasının kimliğini temsil eder.</param>
        /// <returns>Sonucu bir byte dizisi (byte[]) olarak döndürür.</returns>
        Task<byte[]> GetDatFile(int id);

        /// <summary>
        /// Belirli bir kimlik (ID) değerine göre bir arıza kaydını almayı amaçlar
        /// </summary>
        /// <param name="id">Bu parametre, istenen arıza kaydının kimliğini temsil eder.</param>
        /// <returns>Sonucu Disturbance türünde bir nesne olarak döndürür.</returns>
        Task<Disturbance> GetById(int id);
    }
}
