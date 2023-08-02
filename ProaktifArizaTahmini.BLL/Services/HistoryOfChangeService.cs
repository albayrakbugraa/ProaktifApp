using AutoMapper;
using ProaktifArizaTahmini.CORE.Entities;
using ProaktifArizaTahmini.CORE.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.BLL.Services
{
    public class HistoryOfChangeService : IHistoryOfChangeService
    {

        private readonly IHistoryOfChangeRepository historyOfChangeRepository;
        private readonly IMapper mapper;

        public HistoryOfChangeService(IHistoryOfChangeRepository historyOfChangeRepository, IMapper mapper)
        {
            this.historyOfChangeRepository = historyOfChangeRepository;
            this.mapper = mapper;
        }

        public async Task<bool> Create(HistoryOfChange model)
        {
            HistoryOfChange historyOfChange = new HistoryOfChange();
            historyOfChange = mapper.Map(model, historyOfChange);
            return await historyOfChangeRepository.Create(historyOfChange);
        }
    }
}
