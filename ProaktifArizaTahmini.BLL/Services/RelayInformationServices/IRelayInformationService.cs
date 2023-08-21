using ProaktifArizaTahmini.BLL.Models.DTOs;
using ProaktifArizaTahmini.BLL.Models.RequestModel;
using ProaktifArizaTahmini.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.BLL.Services.RelayInformationServices
{
    public interface IRelayInformationService
    {
        Task<List<RelayInformation>> GetRelayInformations();
        Task AddDataList(List<RelayInformation> relayInformationList,User user);
        Task<RelayInformation> GetRelayInformationWhere(string tMkVHucre);
        Task<RelayInformation> GetRelayInformationByDataId(int id);
        Task<bool> UpdateRelayInformation(int id, RelayInformationDTO model);
        Task<bool> CreateRelayInformation(RelayInformationDTO model);
        Task<bool> DeleteRelayInformation(int id);
        Task<List<RelayInformation>> FilterList(RelayInformationFilterParams filterParams);
    }
}
