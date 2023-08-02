using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
    public class DisturbanceService : IDisturbanceService
    {
        private readonly IDisturbanceRepository disturbanceRepository;
        public DisturbanceService(IDisturbanceRepository disturbanceRepository)
        {
            this.disturbanceRepository = disturbanceRepository;
        }

        public async Task<string> GetcfgFile(int id)
        {
            var disturbance = await disturbanceRepository.GetWhere(d=>d.ID == id);
            string cfgFile = disturbance.CfgFileData;
            return cfgFile;
        }
        public async Task<byte[]> GetDatFile(int id)
        {
            byte[] datFile = null;
            var disturbance = await disturbanceRepository.GetWhere(d => d.ID == id);
            datFile = disturbance.DatFileData;
            return datFile;
        }

        public async Task<List<Disturbance>> FilterList(DisturbanceFilterParams filterParams)
        {
            var myDatas = await disturbanceRepository.GetAll();
            var filteredData = myDatas.Where(data =>
                    (string.IsNullOrEmpty(filterParams.FilterTextTm) || (data.TmNo != null && data.TmNo.ToUpper().Contains(filterParams.FilterTextTm.ToUpper()))) &&
                    (string.IsNullOrEmpty(filterParams.FilterTextKv) || (data.kV != null && data.kV.ToUpper().Contains(filterParams.FilterTextKv.ToUpper()))) &&
                    (string.IsNullOrEmpty(filterParams.FilterTextHucre) || (data.HucreNo != null && data.HucreNo.ToUpper().Contains(filterParams.FilterTextHucre.ToUpper()))) &&
                    (string.IsNullOrEmpty(filterParams.FilterTextFider) || (data.FiderName != null && data.FiderName.ToUpper().Contains(filterParams.FilterTextFider.ToUpper()))) &&
                    (string.IsNullOrEmpty(filterParams.FilterTextIp) || (data.IP != null && data.IP.ToUpper().Contains(filterParams.FilterTextIp.ToUpper()))) &&
                    (string.IsNullOrEmpty(filterParams.FilterTextRole) || (data.RoleModel != null && data.RoleModel.ToUpper().Contains(filterParams.FilterTextRole.ToUpper()))) &&
                    (data.FaultTime >= filterParams.FilterFaultTimeStart && data.FaultTime <= filterParams.FilterFaultTimeEnd)
                )
                .ToList();

            return filteredData;
        }

        public async Task<List<Disturbance>> GetDisturbances()
        {
            var disturbanceList = await disturbanceRepository.GetFilteredList(
                 selector: x => new Disturbance
                 {
                     FaultTime = x.FaultTime,
                     TmNo = x.TmNo,
                     CfgFilePath = x.CfgFilePath,
                     DatFilePath = x.DatFilePath,
                     FiderName = x.FiderName,
                     HucreNo = x.HucreNo,
                     IP = x.IP,
                     RoleModel = x.RoleModel,
                     TmKvHucre = x.TmKvHucre
                 },
                 expression: x=>x.Status==true,
                 orderBy: x => x.OrderByDescending(x => x.FaultTime)
                 );
            return disturbanceList;
        }

        public async Task<bool> UpdateByDataIdList(MyDataDTO myData)
        {
            var disturbances = await disturbanceRepository.GetDisturbancesById(myData.ID);
            foreach (var item in disturbances)
            {
                item.IP = myData.IP;
                item.TmNo = myData.TmNo;
                item.HucreNo = myData.HucreNo;
                item.FiderName = myData.FiderName;
                item.RoleModel = myData.RoleModel;
                item.TmKvHucre = myData.TmKvHucre;
                item.kV = myData.kV;

                var result = disturbanceRepository.Update(item);
                if (!result)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<Disturbance> GetById(int id)
        {
            var disturbance = await disturbanceRepository.GetWhere(x => x.ID == id);
            return disturbance;
        }
    }
}
