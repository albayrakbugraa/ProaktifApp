using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComtradeApp.Models
{
    public class ParametersModel
    {
        public RelayInformation RelayInformation { get; set; }
        public string Host { get; set; }
        public string ComtradeFilesPath { get; set; }
        public List<string> DownloadedFiles { get; set; }
        public List<string> FilesToDownload { get; set; }
        public string DestFolderPath { get; set; }
        public List<string> DownloadedCfgFiles { get; set; }
        public List<string> DownloadedDatFiles { get; set; }
    }
}
