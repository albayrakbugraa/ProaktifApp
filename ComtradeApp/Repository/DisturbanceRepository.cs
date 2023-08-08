using ComtradeApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComtradeApp.Repository
{
    public class DisturbanceRepository : BaseRepository<Disturbance>
    {
        private readonly AppDbContext.AppDbContext db;
        public DisturbanceRepository(AppDbContext.AppDbContext db) : base(db)
        {
            this.db = db;
        }
        public async Task<List<Disturbance>> GetBySftpStatus(bool sftpStatus)
        {
            var disturbances = db.Disturbances.Where(d => d.sFtpStatus == sftpStatus).ToList();
            return disturbances;
        }
    }
}
