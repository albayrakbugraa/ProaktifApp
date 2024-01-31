using ProaktifArizaTahmini.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace ProaktifArizaTahmini.BLL.Models.RequestModel
{
    public class DisturbanceFilterParams
    {
        public DisturbanceFilterParams()
        {
            CurrentFaultTimeStart = FilterFaultTimeStart.ToString("yyyy-MM-ddTHH:mm");
            CurrentFaultTimeEnd = FilterFaultTimeEnd.ToString("yyyy-MM-ddTHH:mm");
        }
        public string? FilterTextTm { get; set; }
        public string? FilterTextKv { get; set; }
        public string? FilterTextHucre { get; set; }
        public string? FilterTextFider { get; set; }
        public string? FilterTextIp { get; set; }
        public string? FilterTextRole { get; set; }
        public DateTime FilterFaultTimeStart { get; set; } = DateTime.Now.AddDays(-14);
        public DateTime FilterFaultTimeEnd { get; set; } = DateTime.Now;
        public string? CurrentFilterTm { get; set; }
        public string? CurrentFilterKv { get; set; }
        public string? CurrentFilterHucre { get; set; }
        public string? CurrentFilterFider { get; set; }
        public string? CurrentFilterIp { get; set; }
        public string? CurrentFilterRole { get; set; }
        public string CurrentFaultTimeStart { get; set; }
        public string CurrentFaultTimeEnd { get; set; }
        public IPagedList<Disturbance> DisturbanceListVM { get; set; }
    }
}
