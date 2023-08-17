using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ProaktifArizaTahmini.BLL.Models.RequestModel;
using ProaktifArizaTahmini.BLL.Services.DisturbanceServices;
using ProaktifArizaTahmini.CORE.Entities;
using System.Configuration;
using System.IO.Compression;
using System.Text;
using X.PagedList;

namespace ProaktifArizaTahmini.UI.Controllers
{
    //[Authorize]
    public class DisturbanceController : Controller
    {
        private readonly IDisturbanceService disturbanceService;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public DisturbanceController(IDisturbanceService disturbanceService, IMapper mapper, IConfiguration configuration)
        {
            this.disturbanceService = disturbanceService;
            this.mapper = mapper;
            this.configuration = configuration;
        }
        public async Task<IActionResult> List(DisturbanceFilterParams disturbanceVM, int? page, int? pageSize)
        {
            int minCharLimit = configuration.GetValue<int>("AppSettings:MinimumCharacterLimit");
            ViewData["ActivePage"] = "Disturbance";

            int defaultPageSize = 20;
            ViewBag.PageSize = pageSize ?? defaultPageSize;
            List<Disturbance> relayInformationList = new List<Disturbance>();

            var properties = disturbanceVM.GetType().GetProperties().Where(p => !p.Name.StartsWith("Current"));

            foreach (var property in properties)
            {
                // Özellik adı "time" ile bitiyorsa veya içeriyorsa doğrulamayı atla
                if (property.Name.Contains("time", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var propertyValue = property.GetValue(disturbanceVM)?.ToString();
                if (propertyValue != null && propertyValue.Length < minCharLimit)
                {
                    // Minimum karakter sınırını içeren bir hata mesajı gösterin.
                    ViewBag.ErrorMessage = $"Minimum {minCharLimit} karakter gereklidir.";
                    ClearInvalidProperties(disturbanceVM, minCharLimit);
                }
            }
            if (properties.Any(p => p.GetValue(disturbanceVM) != null) && page==null)
            {
                page = 1;
            }
            else
            {
                relayInformationList = await disturbanceService.GetDisturbances();
                RestoreFilterTexts(disturbanceVM);
            }
            RestoreCurrentFilterTexts(disturbanceVM);

            if (disturbanceVM.GetType().GetProperties().Any(p => p.GetValue(disturbanceVM) != null))
            {
                relayInformationList = await disturbanceService.FilterList(disturbanceVM);
            }

            DisturbanceFilterParams filterParams = mapper.Map<DisturbanceFilterParams>(disturbanceVM);
            int pageNumber = (page ?? 1);
            IPagedList<Disturbance> relayInformationPagedList = new PagedList<Disturbance>(relayInformationList.OrderByDescending(x => x.FaultTimeStart), pageNumber, (int)ViewBag.PageSize);
            filterParams.DisturbanceListVM = relayInformationPagedList;
            return View(filterParams);
        }

        private void ClearInvalidProperties(DisturbanceFilterParams disturbanceVM, int minCharLimit)
        {
            var properties = disturbanceVM.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(string))
                {
                    var value = property.GetValue(disturbanceVM) as string;
                    if (value != null && value.Length < minCharLimit)
                    {
                        property.SetValue(disturbanceVM, null);
                    }
                }
            }
        }

        private void RestoreFilterTexts(DisturbanceFilterParams disturbanceVM)
        {
            disturbanceVM.FilterTextTm = disturbanceVM.CurrentFilterTm;
            disturbanceVM.FilterTextKv = disturbanceVM.CurrentFilterKv;
            disturbanceVM.FilterTextHucre = disturbanceVM.CurrentFilterHucre;
            disturbanceVM.FilterTextFider = disturbanceVM.CurrentFilterFider;
            disturbanceVM.FilterTextIp = disturbanceVM.CurrentFilterIp;
            disturbanceVM.FilterTextRole = disturbanceVM.CurrentFilterRole;
            disturbanceVM.FilterFaultTimeStart = DateTime.Parse(disturbanceVM.CurrentFaultTimeStart);
            disturbanceVM.FilterFaultTimeEnd = DateTime.Parse(disturbanceVM.CurrentFaultTimeEnd);
        }

        private void RestoreCurrentFilterTexts(DisturbanceFilterParams disturbanceVM)
        {
            disturbanceVM.CurrentFilterTm = disturbanceVM.FilterTextTm;
            disturbanceVM.CurrentFilterKv = disturbanceVM.FilterTextKv;
            disturbanceVM.CurrentFilterHucre = disturbanceVM.FilterTextHucre;
            disturbanceVM.CurrentFilterFider = disturbanceVM.FilterTextFider;
            disturbanceVM.CurrentFilterIp = disturbanceVM.FilterTextIp;
            disturbanceVM.CurrentFilterRole = disturbanceVM.FilterTextRole;
            disturbanceVM.CurrentFaultTimeStart = disturbanceVM.FilterFaultTimeStart.ToString("yyyy-MM-ddTHH:mm");
            disturbanceVM.CurrentFaultTimeEnd = disturbanceVM.FilterFaultTimeEnd.ToString("yyyy-MM-ddTHH:mm");
        }

        [HttpPost]
        public async Task<IActionResult> DownloadFilesFromDatabase(int ID)
        {
            var disturbance = await disturbanceService.GetById(ID);
            string comtradeName = disturbance.ComtradeName;
            string cfgFile = disturbance.CfgFileData;
            byte[] datFile = disturbance.DatFileData;
            //string cfgFile = await disturbanceService.GetcfgFile(ID);
            //byte[] datFile = await disturbanceService.GetDatFile(ID);
            if (string.IsNullOrEmpty(cfgFile) || datFile == null || datFile.Length == 0)
            {
                return NotFound();
            }
            var memoryStream = new MemoryStream();
            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                // .cfg dosyasını zip arşivine ekleyin
                var cfgEntry = zipArchive.CreateEntry(comtradeName+".cfg", CompressionLevel.Optimal);
                using (var cfgEntryStream = cfgEntry.Open())
                using (var cfgMemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(cfgFile)))
                {
                    cfgMemoryStream.CopyTo(cfgEntryStream);
                }

                // .dat dosyasını zip arşivine ekleyin
                var datEntry = zipArchive.CreateEntry(comtradeName+".dat", CompressionLevel.Optimal);
                using (var datEntryStream = datEntry.Open())
                using (var datMemoryStream = new MemoryStream(datFile))
                {
                    datMemoryStream.CopyTo(datEntryStream);
                }
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            return File(memoryStream, "application/zip", "Comtrade-Files.zip");
        }

    }
}
