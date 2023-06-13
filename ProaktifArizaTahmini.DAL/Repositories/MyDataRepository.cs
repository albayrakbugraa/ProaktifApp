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
    public class MyDataRepository : BaseRepository<MyData>, IMyDataRepository
    {
        private readonly AppDbContext db;
        public MyDataRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public async Task<bool> AddAllDataList(List<MyData> myDataList)
        {
            foreach (var item in myDataList)
            {
                bool myData = db.MyDatas.Any(x => x.TmKvHucre == item.TmKvHucre);

                if (!myData)
                {
                    var addedData = db.MyDatas.Add(item).Entity;
                }

                await db.SaveChangesAsync();
            }
            return true;
        }

        public Task<List<MyData>> FilteredList(string filterText)
        {
            var filteredData = db.MyDatas.Where(d => d.AvcilarTM.Contains(filterText)
                                                            || d.kV.Contains(filterText)
                                                            || d.HucreNo.Contains(filterText)
                                                            || d.FiderName.Contains(filterText)
                                                            || d.IP.Contains(filterText)
                                                            || d.RoleModel.Contains(filterText)
                                                            || d.User.Contains(filterText)
                                                            || d.Password.Contains(filterText))
                                                .ToListAsync();
            return filteredData;
        }

    }
}
