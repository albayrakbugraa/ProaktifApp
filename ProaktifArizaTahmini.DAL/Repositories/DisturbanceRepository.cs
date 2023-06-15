using Microsoft.EntityFrameworkCore;
using ProaktifArizaTahmini.CORE.Entities;
using ProaktifArizaTahmini.CORE.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.DAL.Repositories
{
    public class DisturbanceRepository : BaseRepository<Disturbance>, IDisturbanceRepository
    {
        private readonly AppDbContext db;
        public DisturbanceRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }
        public Task<List<Disturbance>> FilteredList(string filterText)
        {
            var filteredData = db.Disturbances.Where(d => d.AvcilarTM.Contains(filterText)
                                                            || d.kV.Contains(filterText)
                                                            || d.HucreNo.Contains(filterText)
                                                            || d.FiderName.Contains(filterText)
                                                            || d.IP.Contains(filterText)
                                                            || d.RoleModel.Contains(filterText)
                                                            )
                                                .ToListAsync();
            return filteredData;
        }

        public Task<List<Disturbance>> GetDisturbancesById(int Id)
        {
            var disturbances = db.Disturbances.Where(d => d.MyDataId == Id).ToListAsync();
            return disturbances;
        }
    }
}
