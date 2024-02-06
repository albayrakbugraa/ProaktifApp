using ProaktifArizaTahmini.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace ProaktifArizaTahmini.BLL.Models.RequestModel
{
    public class LogModel
    {
        public LogModel()
        {
            CurrentFaultTimeStart = FilterFaultTimeStart.ToString("yyyy-MM-ddTHH:mm");
            CurrentFaultTimeEnd = FilterFaultTimeEnd.ToString("yyyy-MM-ddTHH:mm");
        }
        public string CurrentFaultTimeStart { get; set; }
        public string CurrentFaultTimeEnd { get; set; }
        public string? CurrentLogLevel { get; set; }
        public string? CurrentServiceName { get; set; }
        public string? CurrentMessage { get; set; }
        public string? CurrentException { get; set; }
        public string? FilterLogLevel { get; set; }
        public string? FilterServiceName{ get; set; }
        public string? FilterMessage{ get; set; }
        public string? FilterException{ get; set; }
        public DateTime FilterFaultTimeStart { get; set; } = DateTime.Now.AddDays(-14);
        public DateTime FilterFaultTimeEnd { get; set; } = DateTime.Now;
        public IPagedList<ServiceLog> ServiceLogs { get; set; }
    }
}
