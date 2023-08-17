using ProaktifArizaTahmini.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace ProaktifArizaTahmini.BLL.Models.RequestModel
{
    public class RelayInformationFilterParams
    {
        public string? FilterTextTm { get; set; }
        public string? FilterTextKv { get; set; }
        public string? FilterTextHucre { get; set; }
        public string? FilterTextFider { get; set; }
        public string? FilterTextIp { get; set; }
        public string? FilterTextRole { get; set; }
        public string? FilterTextKullanici { get; set; }
        public string? FilterTextSifre { get; set; }
        public string? CurrentFilterTm { get; set; }
        public string? CurrentFilterKv { get; set; }
        public string? CurrentFilterHucre { get; set; }
        public string? CurrentFilterFider { get; set; }
        public string? CurrentFilterIp { get; set; }
        public string? CurrentFilterRole { get; set; }
        public string? CurrentFilterKullanici { get; set; }
        public string? CurrentFilterSifre { get; set; }
        public IPagedList<RelayInformation> RelayInformationList { get; set; }
    }
}
