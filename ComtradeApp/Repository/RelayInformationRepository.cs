using ComtradeApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComtradeApp.Repository
{
    public class RelayInformationRepository : BaseRepository<RelayInformation>
    {
        public RelayInformationRepository(AppDbContext.AppDbContext db) : base(db)
        {
        }
    }
}
