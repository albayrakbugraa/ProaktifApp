using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ProaktifArizaTahmini.BLL.Models.RequestModel;
using ProaktifArizaTahmini.BLL.Services;
using ProaktifArizaTahmini.CORE.Entities;
using System.IO.Compression;
using X.PagedList;

namespace ProaktifArizaTahmini.UI.Controllers
{
    public class DisturbanceController : Controller
    {
        private readonly IDisturbanceService disturbanceService;
        private readonly IMapper mapper;

        public DisturbanceController(IDisturbanceService disturbanceService, IMapper mapper)
        {
            this.disturbanceService = disturbanceService;
            this.mapper = mapper;
        }
        public async Task<IActionResult> Deneme(DisturbanceFilterParams disturbanceVM, int? page, int? pageSize)
        {
            ViewData["ActivePage"] = "Disturbance";
            int defaultPageSize = 20;
            ViewBag.PageSize = pageSize ?? defaultPageSize;
            List<Disturbance> myDataList = new List<Disturbance>();
            var properties = disturbanceVM.GetType().GetProperties().Where(p => !p.Name.StartsWith("Current"));
            if (properties.Any(p => p.GetValue(disturbanceVM) != null) && page==null)
            {
                page = 1;
            }
            else
            {
                myDataList = await disturbanceService.GetDisturbances();
                disturbanceVM.FilterTextTm = disturbanceVM.CurrentFilterTm;
                disturbanceVM.FilterTextKv = disturbanceVM.CurrentFilterKv;
                disturbanceVM.FilterTextHucre = disturbanceVM.CurrentFilterHucre;
                disturbanceVM.FilterTextFider = disturbanceVM.CurrentFilterFider;
                disturbanceVM.FilterTextIp = disturbanceVM.CurrentFilterIp;
                disturbanceVM.FilterTextRole = disturbanceVM.CurrentFilterRole;
                disturbanceVM.FilterFaultTimeStart = DateTime.Parse(disturbanceVM.CurrentFaultTimeStart);
                disturbanceVM.FilterFaultTimeEnd = DateTime.Parse(disturbanceVM.CurrentFaultTimeEnd);

            }
            disturbanceVM.CurrentFilterTm = disturbanceVM.FilterTextTm;
            disturbanceVM.CurrentFilterKv = disturbanceVM.FilterTextKv;
            disturbanceVM.CurrentFilterHucre = disturbanceVM.FilterTextHucre;
            disturbanceVM.CurrentFilterFider = disturbanceVM.FilterTextFider;
            disturbanceVM.CurrentFilterIp = disturbanceVM.FilterTextIp;
            disturbanceVM.CurrentFilterRole = disturbanceVM.FilterTextRole;
            disturbanceVM.CurrentFaultTimeStart = disturbanceVM.FilterFaultTimeStart.ToString("yyyy-MM-ddTHH:mm");
            disturbanceVM.CurrentFaultTimeEnd = disturbanceVM.FilterFaultTimeEnd.ToString("yyyy-MM-ddTHH:mm");

            if (disturbanceVM != null && disturbanceVM.GetType().GetProperties().Any(p => p.GetValue(disturbanceVM) != null))
            {
                myDataList = await disturbanceService.FilterList(disturbanceVM);
            }
            DisturbanceFilterParams filterParams = new DisturbanceFilterParams();
            filterParams = mapper.Map(disturbanceVM, filterParams);
            int pageNumber = (page ?? 1);
            IPagedList<Disturbance> myDataPagedList = new PagedList<Disturbance>(myDataList.OrderByDescending(x => x.FaultTime), pageNumber, (int)ViewBag.PageSize);
            filterParams.DisturbanceListVM = myDataPagedList;
            return View(filterParams);
        }
        public async Task<IActionResult> List(string filterText, string currentFilter, int? page, int? pageSize)
        {
            int defaultPageSize = 20;
            ViewBag.PageSize = pageSize ?? defaultPageSize;
            List<Disturbance> disturbanceList = new List<Disturbance>();
            if (filterText != null)
            {
                page = 1;
            }
            else
            {
                disturbanceList = await disturbanceService.GetDisturbances();
                filterText = currentFilter;
                ViewBag.DataCount = disturbanceList.Count;
            }
            ViewBag.CurrentFilter = filterText;

            if (!String.IsNullOrEmpty(filterText))
            {
                disturbanceList = await disturbanceService.FilteredList(filterText.ToUpper());
                ViewBag.DataCount = disturbanceList.Count;
            }
            int pageNumber = (page ?? 1);
            return View(disturbanceList.ToPagedList(pageNumber, (int)ViewBag.PageSize));
        }
        [HttpPost]
        public async Task<IActionResult> List(DateTime FaultStartDate, DateTime FaultEndDate, int page = 1, int pageSize = 10)
        {
            if (FaultEndDate == DateTime.MinValue)
            {
                var endDate = DateTime.MaxValue;
                var filteredDisturbances = await disturbanceService.FilterByFaultTime(FaultStartDate, endDate);
                var value = filteredDisturbances.ToPagedList(page, pageSize);
                return View(value);
            }
            else
            {
                var filteredDisturbances = await disturbanceService.FilterByFaultTime(FaultStartDate, FaultEndDate);
                var value = filteredDisturbances.ToPagedList(page, pageSize);
                return View(value);
            }

        }

        [HttpPost]
        [Route("disturbances/filterpagesize")]
        public IActionResult PageSize(DateTime FaultStartDate, DateTime FaultEndDate, int page = 1, int pageSize = 10)
        {
            return RedirectToAction("List", new { FaultStartDate, FaultEndDate, page, pageSize });
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

    }
}
