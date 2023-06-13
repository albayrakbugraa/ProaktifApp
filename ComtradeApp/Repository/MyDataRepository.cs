using ComtradeApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComtradeApp.Repository
{
    public class MyDataRepository : BaseRepository<MyData>
    {
        public MyDataRepository(AppDbContext.AppDbContext db) : base(db)
        {
        }
    }
}
