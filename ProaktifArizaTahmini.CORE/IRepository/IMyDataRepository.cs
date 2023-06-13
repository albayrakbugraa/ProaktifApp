using ProaktifArizaTahmini.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.CORE.IRepository
{
    public interface IMyDataRepository : IBaseRepository<MyData>
    {
        Task<bool> AddAllDataList(List<MyData> myDataList);
        Task<List<MyData>> FilteredList(string filterText);
    }
}
