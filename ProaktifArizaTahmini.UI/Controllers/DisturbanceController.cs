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
            List<Disturbance> myDataList = new List<Disturbance>();

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
                myDataList = await disturbanceService.GetDisturbances();
                RestoreFilterTexts(disturbanceVM);
            }
            RestoreCurrentFilterTexts(disturbanceVM);

            if (disturbanceVM.GetType().GetProperties().Any(p => p.GetValue(disturbanceVM) != null))
            {
                myDataList = await disturbanceService.FilterList(disturbanceVM);
            }

            DisturbanceFilterParams filterParams = mapper.Map<DisturbanceFilterParams>(disturbanceVM);
            int pageNumber = (page ?? 1);
            IPagedList<Disturbance> myDataPagedList = new PagedList<Disturbance>(myDataList.OrderByDescending(x => x.FaultTimeStart), pageNumber, (int)ViewBag.PageSize);
            filterParams.DisturbanceListVM = myDataPagedList;
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
        public IActionResult DownloadFiles(string filePath1, string filePath2)
        {
            // Dosyaların varlığını kontrol edin
            if (!System.IO.File.Exists(filePath1) || !System.IO.File.Exists(filePath2))
            {
                return NotFound();
            }

            // Dosyaların adlarını alın
            string fileName1 = Path.GetFileName(filePath1);
            string fileName2 = Path.GetFileName(filePath2);

            // İki dosyayı kullanıcıya indirin
            var memoryStream1 = new MemoryStream();
            using (var fileStream1 = new FileStream(filePath1, FileMode.Open))
            {
                fileStream1.CopyTo(memoryStream1);
            }

            var memoryStream2 = new MemoryStream();
            using (var fileStream2 = new FileStream(filePath2, FileMode.Open))
            {
                fileStream2.CopyTo(memoryStream2);
            }

            // İndirilecek dosyaların MIME türünü belirleyin
            var mimeType = "application/octet-stream";

            // İki dosyayı birleştirip kullanıcıya indirme işlemi gerçekleştirin
            var combinedMemoryStream = new MemoryStream();
            using (var zipArchive = new ZipArchive(combinedMemoryStream, ZipArchiveMode.Create, true))
            {
                // İlk dosyayı ZIP arşivine ekleyin
                var entry1 = zipArchive.CreateEntry(fileName1);
                using (var entryStream1 = entry1.Open())
                {
                    memoryStream1.Seek(0, SeekOrigin.Begin);
                    memoryStream1.CopyTo(entryStream1);
                }

                // İkinci dosyayı ZIP arşivine ekleyin
                var entry2 = zipArchive.CreateEntry(fileName2);
                using (var entryStream2 = entry2.Open())
                {
                    memoryStream2.Seek(0, SeekOrigin.Begin);
                    memoryStream2.CopyTo(entryStream2);
                }
            }

            combinedMemoryStream.Seek(0, SeekOrigin.Begin);
            return File(combinedMemoryStream, mimeType, "comtrade.zip");
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
