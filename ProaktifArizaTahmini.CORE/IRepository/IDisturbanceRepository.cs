using ProaktifArizaTahmini.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.CORE.IRepository
{
    public interface IDisturbanceRepository : IBaseRepository<Disturbance>
    {
        Task<List<Disturbance>> FilteredList(string filterText);
        Task<List<Disturbance>> GetDisturbancesById(int Id);
    }
}
