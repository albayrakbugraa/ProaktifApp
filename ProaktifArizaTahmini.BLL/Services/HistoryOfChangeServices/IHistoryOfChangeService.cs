using ProaktifArizaTahmini.BLL.Models.DTOs;
using ProaktifArizaTahmini.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.BLL.Services.HistoryOfChangeServices
{
    /// <summary>
    ///HistoryOfChange Service için bir arayüz (interface) tanımlar.
    /// </summary>
    public interface IHistoryOfChangeService
    {
        /// <summary>
        ///  Bir değişiklik geçmişi kaydı oluşturmayı amaçlar. 
        /// </summary>
        /// <param name="model">Bu değişiklik geçmişi kaydının temsil edildiği bir nesnedir.</param>
        /// <returns> İşlem sonucunu bool değeriyle döndürür.</returns>
        Task<bool> Create(HistoryOfChange model);
    }
}
