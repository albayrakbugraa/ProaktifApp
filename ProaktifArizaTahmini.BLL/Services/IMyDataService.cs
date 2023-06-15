using ProaktifArizaTahmini.BLL.Models.DTOs;
using ProaktifArizaTahmini.BLL.Models.RequestModel;
using ProaktifArizaTahmini.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.BLL.Services
{
    public interface IMyDataService
    {
        Task<List<MyData>> GetMyDatas();
        Task<bool> AddDataList(List<MyData> myDataList);
        Task<MyData> GetMyDataWhere(string tMkVHucre);
        Task<MyData> GetMyDataByDataId(int id);
        Task<bool> UpdateMyData(int id, MyDataDTO model);
        Task<bool> CreateMyData(MyDataDTO model);
        Task<bool> DeleteMyData(int id);
        Task<List<MyData>> FilteredList(string filterText);
        Task<List<MyData>> FilterList(MyDataFilterParams filterParams);
    }
}
