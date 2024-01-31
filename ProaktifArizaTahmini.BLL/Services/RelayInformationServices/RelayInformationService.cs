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

        public async Task<(List<RelayInformation> duplicateRelays, List<RelayInformation> incompatibleRelays, List<RelayInformation> blankRowRelays)> AddDataList(List<RelayInformation> relayInformationList, User user)
        {
            List<RelayInformation> duplicateRelays = new List<RelayInformation>();
            List<RelayInformation> incompatibleRelays = new List<RelayInformation>();
            List<RelayInformation> blankRowRelays = new List<RelayInformation>();

            foreach (var item in relayInformationList)
            {
                if (string.IsNullOrEmpty(item.User) || string.IsNullOrEmpty(item.Password) || string.IsNullOrEmpty(item.Path) || string.IsNullOrEmpty(item.IP) || string.IsNullOrEmpty(item.TmNo) || string.IsNullOrEmpty(item.kV) || string.IsNullOrEmpty(item.HucreNo))
                {
                    blankRowRelays.Add(item);
                    await log.ErrorLog(user, "Boş alanlar mevcut.", "Excel Tablosundan Veri Girişi", "Röle bilgilerinde boş alanlar mevcut.");
                    continue;
                }
                bool relayInformation = await relayInformationRepository.Any(x => x.TmKvHucre == item.TmKvHucre);
                if (relayInformation)
                {
                    duplicateRelays.Add(item);
                    await log.ErrorLog(user, $"Tm_Kv_Hücre : {item.TmKvHucre} bu röle zaten mevcut.", "Excel Tablosundan Veri Girişi", "Aynı Tm_Kv_Hücre röleler tekrar eklenemez.");
                    continue;
                }

                bool isAnotherRelay = item.Path.Equals("Bilinmiyor");
                if (isAnotherRelay)
                {
                    incompatibleRelays.Add(item);
                    await log.ErrorLog(user, $"Röle Model : {item.RoleModel} Bu röle modeli şuanda eklenemez!", "Excel Tablosundan Veri Girişi", "Röle modeli Schneider veya ABB olmadığı için şuanda bu röleyi ekleyemezsiniz.");
                    continue;
                }
                item.Status = true;
                var addedData = await relayInformationRepository.Create(item);
                if (addedData)
                {
                    await log.InformationLog(user, "Excel Tablosundan Veri Girişi", $"Tm_Kv_Hücre : {item.TmKvHucre} IP : {item.IP} Röle Model : {item.RoleModel} Röle eklendi.");
                }
            }

            return (duplicateRelays, incompatibleRelays, blankRowRelays);
        }


        public async Task<bool> CreateRelayInformation(RelayInformationDTO model)
        {
            var result = await relayInformationRepository.Any(x => x.TmKvHucre == model.TmKvHucre);
            if (!result)
            {
                RelayInformation relayInformation = new RelayInformation();
                relayInformation = mapper.Map(model, relayInformation);
                relayInformation.Status = true;
                return await relayInformationRepository.Create(relayInformation);
            }
            return false;
        }

        public async Task<bool> DeleteRelayInformation(int id)
        {
            RelayInformation relayInformation = await relayInformationRepository.GetWhere(x => x.ID == id);
            if (relayInformation == null) return false;
            relayInformation.Status = false;
            bool result = relayInformationRepository.Update(relayInformation);
            return result;
        }

        public async Task<List<RelayInformation>> FilterList(RelayInformationFilterParams filterParams)
        {
            var relayInformations = await relayInformationRepository.GetAll();
            var filteredData = relayInformations.Where(data =>
                     data.Status == true &&
                    (string.IsNullOrEmpty(filterParams.FilterTextTm) || data.TmNo != null && data.TmNo.ToUpper().Contains(filterParams.FilterTextTm.ToUpper())) &&
                    (string.IsNullOrEmpty(filterParams.FilterTextKv) || data.kV != null && data.kV.ToUpper().Contains(filterParams.FilterTextKv.ToUpper())) &&
                    (string.IsNullOrEmpty(filterParams.FilterTextHucre) || data.HucreNo != null && data.HucreNo.ToUpper().Contains(filterParams.FilterTextHucre.ToUpper())) &&
                    (string.IsNullOrEmpty(filterParams.FilterTextFider) || data.FiderName != null && data.FiderName.ToUpper().Contains(filterParams.FilterTextFider.ToUpper())) &&
                    (string.IsNullOrEmpty(filterParams.FilterTextIp) || data.IP != null && data.IP.ToUpper().Contains(filterParams.FilterTextIp.ToUpper())) &&
                    (string.IsNullOrEmpty(filterParams.FilterTextRole) || data.RoleModel != null && data.RoleModel.ToUpper().Contains(filterParams.FilterTextRole.ToUpper()))
                )
                .ToList();

            return filteredData;
        }


        public async Task<RelayInformation> GetRelayInformationById(int id)
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
                     TmKvHucre = x.TmKvHucre,
                     Status = x.Status
                 },
                 expression: x => x.Status == true,
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
            relayInformation.Status = true;
            bool result = relayInformationRepository.Update(relayInformation);
            return result;
        }

    }
}
