using ProaktifArizaTahmini.BLL.Models.DTOs;
using ProaktifArizaTahmini.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.BLL.Services
{
    public interface IHistoryOfChangeService
    {
        Task<bool> Create(HistoryOfChange model);
    }
}
