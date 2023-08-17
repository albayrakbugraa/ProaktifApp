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

        public async Task<bool> AddAllDataList(List<RelayInformation> relayInformationList)
        {
            foreach (var item in relayInformationList)
            {
                bool relayInformation = db.RelayInformations.Any(x => x.TmKvHucre == item.TmKvHucre);

                if (!relayInformation)
                {
                    var addedData = db.RelayInformations.Add(item).Entity;
                }
                await db.SaveChangesAsync();
            }
            return true;
        }

    }
}
