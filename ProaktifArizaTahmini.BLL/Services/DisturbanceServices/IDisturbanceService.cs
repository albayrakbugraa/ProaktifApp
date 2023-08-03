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
    public interface IDisturbanceService
    {
        Task<List<Disturbance>> GetDisturbances();
        Task<List<Disturbance>> FilterList(DisturbanceFilterParams filterParams);
        Task<bool> UpdateByDataIdList(MyDataDTO myData);
        Task<string> GetcfgFile(int id);
        Task<byte[]> GetDatFile(int id);
        Task<Disturbance> GetById(int id);
    }
}
