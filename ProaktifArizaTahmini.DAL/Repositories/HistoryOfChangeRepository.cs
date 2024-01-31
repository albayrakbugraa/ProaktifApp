using ProaktifArizaTahmini.CORE.Entities;
using ProaktifArizaTahmini.CORE.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.DAL.Repositories
{
    public class HistoryOfChangeRepository : BaseRepository<HistoryOfChange>, IHistoryOfChangeRepository
    {
        private readonly AppDbContext db;

        public HistoryOfChangeRepository(AppDbContext db) : base(db)
        {
        }
    }
}
