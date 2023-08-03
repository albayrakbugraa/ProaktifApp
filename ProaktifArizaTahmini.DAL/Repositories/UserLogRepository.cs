using ProaktifArizaTahmini.CORE.Entities;
using ProaktifArizaTahmini.CORE.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.DAL.Repositories
{
    public class UserLogRepository : BaseRepository<UserLog>, IUserLogRepository
    {
        private readonly AppDbContext db;
        public UserLogRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }
    }
}
