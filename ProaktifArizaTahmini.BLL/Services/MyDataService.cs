using AutoMapper;
using ProaktifArizaTahmini.BLL.Models.DTOs;
using ProaktifArizaTahmini.BLL.Models.RequestModel;
using ProaktifArizaTahmini.CORE.Entities;
using ProaktifArizaTahmini.CORE.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.BLL.Services
{
    public class MyDataService : IMyDataService
    {
        private readonly IMyDataRepository myDataRepository;
        private readonly IMapper mapper;

        public MyDataService(IMyDataRepository myDataRepository, IMapper mapper)
        {
            this.myDataRepository = myDataRepository;
            this.mapper = mapper;
        }

        public async Task<bool> AddDataList(List<MyData> myDataList)
        {
            return await myDataRepository.AddAllDataList(myDataList);
        }

        public async Task<bool> CreateMyData(MyDataDTO model)
        {
            var result = await myDataRepository.Any(x=>x.TmKvHucre==model.TmKvHucre);
            if (!result)
            {
                MyData myData = new MyData();
                myData = mapper.Map(model, myData);
                return await myDataRepository.Create(myData);
            }
            return false;
        }

        public async Task<bool> DeleteMyData(int id)
        {
            MyData myData = await myDataRepository.GetWhere(x => x.ID == id);
            if (myData == null) return false;
            bool result = myDataRepository.Delete(myData);
            return result;
        }

        public async Task<List<MyData>> FilterList(MyDataFilterParams filterParams)
        {
            var myDatas = await myDataRepository.GetAll();
            var filteredData= myDatas.Where(data =>
                    (string.IsNullOrEmpty(filterParams.FilterTextTm) || (data.TmNo != null && data.TmNo.ToUpper().Contains(filterParams.FilterTextTm.ToUpper()))) &&
                    (string.IsNullOrEmpty(filterParams.FilterTextKv) || (data.kV != null && data.kV.ToUpper().Contains(filterParams.FilterTextKv.ToUpper()))) &&
                    (string.IsNullOrEmpty(filterParams.FilterTextHucre) || (data.HucreNo != null && data.HucreNo.ToUpper().Contains(filterParams.FilterTextHucre.ToUpper()))) &&
                    (string.IsNullOrEmpty(filterParams.FilterTextFider) || (data.FiderName != null && data.FiderName.ToUpper().Contains(filterParams.FilterTextFider.ToUpper()))) &&
                    (string.IsNullOrEmpty(filterParams.FilterTextIp) || (data.IP != null && data.IP.ToUpper().Contains(filterParams.FilterTextIp.ToUpper()))) &&
                    (string.IsNullOrEmpty(filterParams.FilterTextRole) || (data.RoleModel != null && data.RoleModel.ToUpper().Contains(filterParams.FilterTextRole.ToUpper()))) &&
                    (string.IsNullOrEmpty(filterParams.FilterTextKullanici) || (data.User != null && data.User.ToUpper().Contains(filterParams.FilterTextKullanici.ToUpper()))) &&
                    (string.IsNullOrEmpty(filterParams.FilterTextSifre) || (data.Password != null && data.Password.ToUpper().Contains(filterParams.FilterTextSifre.ToUpper())))
                )
                .ToList();

            return filteredData;
        }


        public async Task<MyData> GetMyDataByDataId(int id)
        {
            var data = await myDataRepository.GetWhere(x => x.ID == id);
            return data;
        }

        public async Task<List<MyData>> GetMyDatas()
        {
            var myDataList = await myDataRepository.GetFilteredList(
                 selector: x => new MyData
                 {
                     ID=x.ID,
                     kV = x.kV,
                     IP = x.IP,
                     HucreNo = x.HucreNo,
                     TmNo = x.TmNo,
                     FiderName = x.FiderName,
                     User = x.User,
                     Password = x.Password,
                     RoleModel = x.RoleModel,
                     Disturbances = x.Disturbances,
                     Path = x.Path,
                     Port = x.Port,
                     TmKvHucre = x.TmKvHucre
                 },
                 expression: null,
                 orderBy: x => x.OrderBy(x => x.TmNo)
                 );
            return myDataList;
        }

        public async Task<MyData> GetMyDataWhere(string tMkVHucre)
        {
            return await myDataRepository.GetWhere(x=>x.TmKvHucre== tMkVHucre);
        }

        public async Task<bool> UpdateMyData(int id, MyDataDTO model)
        {
            MyData myData = await myDataRepository.GetWhere(x => x.ID == id);
            myData = mapper.Map(model, myData);
            bool result = myDataRepository.Update(myData);
            return result;
        }        
    }
}
