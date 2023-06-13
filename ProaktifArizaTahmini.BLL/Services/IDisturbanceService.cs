using ProaktifArizaTahmini.BLL.Models.RequestModel;
using ProaktifArizaTahmini.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.BLL.Services
{
    public interface IDisturbanceService
    {
        Task<List<Disturbance>> GetDisturbances();
        Task<List<Disturbance>> FilterByFaultTime(DateTime FaultStartDate,DateTime FaultEndDate);
        Task<List<Disturbance>> FilteredList(string filterText);
        Task<List<Disturbance>> FilterList(DisturbanceFilterParams filterParams);
    }
}
