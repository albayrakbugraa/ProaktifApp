using AutoMapper;
using ProaktifArizaTahmini.BLL.Models.DTOs;
using ProaktifArizaTahmini.BLL.Models.RequestModel;
using ProaktifArizaTahmini.BLL.Services.UserLogServices;
using ProaktifArizaTahmini.CORE.Entities;
using ProaktifArizaTahmini.CORE.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.BLL.Services.RelayInformationServices
{
    public class RelayInformationService : IRelayInformationService
    {
        private readonly IRelayInformationRepository relayInformationRepository;
        private readonly IMapper mapper;
        private readonly IUserLogService log;

        public RelayInformationService(IRelayInformationRepository relayInformationRepository, IMapper mapper, IUserLogService log)
        {
            this.relayInformationRepository = relayInformationRepository;
            this.mapper = mapper;
            this.log = log;
        }

        public async Task AddDataList(List<RelayInformation> relayInformationList,User user)
        {
            foreach (var item in relayInformationList)
            {
                bool relayInformation = await relayInformationRepository.Any(x => x.TmKvHucre == item.TmKvHucre);
                if (relayInformation) await log.ErrorLog(user, $"Tm_Kv_Hücre : {item.TmKvHucre} bu röle zaten mevcut.", "Excel Tablosundan Veri Girişi", "Aynı Tm_Kv_Hücre röleler tekrar eklenemez.");
                bool isAnotherRelay = item.Path.Equals("Bilinmiyor");
                if (isAnotherRelay) await log.ErrorLog(user, $"Röle Model : {item.RoleModel} Bu röle modeli şuanda eklenemez!", "Excel Tablosundan Veri Girişi", "Röle modeli Schneider veya ABB olmadığı için şuanda bu röleyi ekleyemezsiniz.");
                if (!relayInformation && !isAnotherRelay)
                {
                    var addedData = await relayInformationRepository.Create(item);
                    if(addedData) await log.InformationLog(user,"Excel Tablosundan Veri Girişi",$"Tm_Kv_Hücre : {item.TmKvHucre } IP : {item.IP} Röle Model : {item.RoleModel} Röle eklendi.");
                }
            }
        }

        public async Task<bool> CreateRelayInformation(RelayInformationDTO model)
        {
            var result = await relayInformationRepository.Any(x => x.TmKvHucre == model.TmKvHucre);
            if (!result)
            {
                RelayInformation relayInformation = new RelayInformation();
                relayInformation = mapper.Map(model, relayInformation);
                return await relayInformationRepository.Create(relayInformation);
            }
            return false;
        }

        public async Task<bool> DeleteRelayInformation(int id)
        {
            RelayInformation relayInformation = await relayInformationRepository.GetWhere(x => x.ID == id);
            if (relayInformation == null) return false;
            bool result = relayInformationRepository.Delete(relayInformation);
            return result;
        }

        public async Task<List<RelayInformation>> FilterList(RelayInformationFilterParams filterParams)
        {
            var relayInformations = await relayInformationRepository.GetAll();
            var filteredData = relayInformations.Where(data =>
                    (string.IsNullOrEmpty(filterParams.FilterTextTm) || data.TmNo != null && data.TmNo.ToUpper().Contains(filterParams.FilterTextTm.ToUpper())) &&
                    (string.IsNullOrEmpty(filterParams.FilterTextKv) || data.kV != null && data.kV.ToUpper().Contains(filterParams.FilterTextKv.ToUpper())) &&
                    (string.IsNullOrEmpty(filterParams.FilterTextHucre) || data.HucreNo != null && data.HucreNo.ToUpper().Contains(filterParams.FilterTextHucre.ToUpper())) &&
                    (string.IsNullOrEmpty(filterParams.FilterTextFider) || data.FiderName != null && data.FiderName.ToUpper().Contains(filterParams.FilterTextFider.ToUpper())) &&
                    (string.IsNullOrEmpty(filterParams.FilterTextIp) || data.IP != null && data.IP.ToUpper().Contains(filterParams.FilterTextIp.ToUpper())) &&
                    (string.IsNullOrEmpty(filterParams.FilterTextRole) || data.RoleModel != null && data.RoleModel.ToUpper().Contains(filterParams.FilterTextRole.ToUpper())) &&
                    (string.IsNullOrEmpty(filterParams.FilterTextKullanici) || data.User != null && data.User.ToUpper().Contains(filterParams.FilterTextKullanici.ToUpper())) &&
                    (string.IsNullOrEmpty(filterParams.FilterTextSifre) || data.Password != null && data.Password.ToUpper().Contains(filterParams.FilterTextSifre.ToUpper()))
                )
                .ToList();

            return filteredData;
        }


        public async Task<RelayInformation> GetRelayInformationByDataId(int id)
        {
            var data = await relayInformationRepository.GetWhere(x => x.ID == id);
            return data;
        }

        public async Task<List<RelayInformation>> GetRelayInformations()
        {
            var relayInformationList = await relayInformationRepository.GetFilteredList(
                 selector: x => new RelayInformation
                 {
                     ID = x.ID,
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
            return relayInformationList;
        }

        public async Task<RelayInformation> GetRelayInformationWhere(string tMkVHucre)
        {
            return await relayInformationRepository.GetWhere(x => x.TmKvHucre == tMkVHucre);
        }

        public async Task<bool> UpdateRelayInformation(int id, RelayInformationDTO model)
        {
            RelayInformation relayInformation = await relayInformationRepository.GetWhere(x => x.ID == id);
            relayInformation = mapper.Map(model, relayInformation);
            bool result = relayInformationRepository.Update(relayInformation);
            return result;
        }
    }
}
