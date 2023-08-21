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
    public class RelayInformationRepository : BaseRepository<RelayInformation>, IRelayInformationRepository
    {
        private readonly AppDbContext db;
        public RelayInformationRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

    }
}
