using AutoMapper;
using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProaktifArizaTahmini.BLL.Models.DTOs;
using ProaktifArizaTahmini.BLL.Models.RequestModel;
using ProaktifArizaTahmini.BLL.Services.DisturbanceServices;
using ProaktifArizaTahmini.BLL.Services.HistoryOfChangeServices;
using ProaktifArizaTahmini.BLL.Services.RelayInformationServices;
using ProaktifArizaTahmini.BLL.Services.UserLogServices;
using ProaktifArizaTahmini.BLL.Services.UserServices;
using ProaktifArizaTahmini.CORE.Entities;
using ProaktifArizaTahmini.CORE.IRepository;
using ProaktifArizaTahmini.UI.Helper;
using ProaktifArizaTahmini.UI.Models;
using System.Formats.Asn1;
using System.Globalization;
using System.Security.Claims;
using X.PagedList;
using static NuGet.Packaging.PackagingConstants;

namespace ProaktifArizaTahmini.UI.Controllers
{
    [Authorize]
    public class RelayInformationController : Controller
    {
        private readonly IRelayInformationService relayInformationService;
        private readonly IDisturbanceService disturbanceService;
        private readonly IHistoryOfChangeService historyOfChangeService;
        private readonly IUserLogService userLogService;
        private readonly IUserService userService;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IMemoryCache memoryCache;

        public RelayInformationController(IRelayInformationService relayInformationService, IMapper mapper, IDisturbanceService disturbanceService, IConfiguration configuration, IHistoryOfChangeService historyOfChangeService, IUserLogService userLogService, IUserService userService, IMemoryCache memoryCache)
        {
            this.relayInformationService = relayInformationService;
            this.mapper = mapper;
            this.disturbanceService = disturbanceService;
            this.configuration = configuration;
            this.historyOfChangeService = historyOfChangeService;
            this.userLogService = userLogService;
            this.userService = userService;
            this.memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> List(RelayInformationFilterParams relayInformationVM, int? page, int? pageSize)
        {
            string path = configuration.GetValue<string>("AppSettings:Path"); // JS için projenin alt klasör yolu.
            ViewBag.Path = path;
            int minCharLimit = configuration.GetValue<int>("AppSettings:MinimumCharacterLimit");
            ViewData["ActivePage"] = "RelayInformation";

            int? duplicateRelaysCount = memoryCache.Get<int>("DuplicateRelaysCount");
            int? incompatibleRelaysCount = memoryCache.Get<int>("IncompatibleRelaysCount");
            int? blankRowRelaysCount = memoryCache.Get<int>("BlankRowRelaysCount");
            string? excelInputError = memoryCache.Get<string>("ExcelInputError");
            ViewBag.DuplicateRelaysCount = duplicateRelaysCount;
            ViewBag.IncompatibleRelaysCount = incompatibleRelaysCount;
            ViewBag.BlankRowRelaysCount = blankRowRelaysCount;
            ViewBag.ExcelInputError = excelInputError;

            int defaultPageSize = 20;
            ViewBag.PageSize = pageSize ?? defaultPageSize;
            List<RelayInformation> relayInformationList = new List<RelayInformation>();

            var properties = relayInformationVM.GetType().GetProperties().Where(p => !p.Name.StartsWith("Current"));

            if (properties.Any(p => p.GetValue(relayInformationVM) != null && p.GetValue(relayInformationVM).ToString().Length < minCharLimit))
            {
                ViewBag.ErrorMessage = $"Minimum {minCharLimit} karakter gereklidir.";
                ClearInvalidProperties(relayInformationVM, minCharLimit);
            }
            else if (properties.Any(p => p.GetValue(relayInformationVM) != null))
            {
                page = 1;
            }
            else
            {
                relayInformationList = await relayInformationService.GetRelayInformations();
                foreach (var item in relayInformationList)
                {
                    item.User = Encryption.Encrypt(item.User);
                }
                RestoreFilterTexts(relayInformationVM);
            }
            RestoreCurrentFilterTexts(relayInformationVM);

            if (relayInformationVM.GetType().GetProperties().Any(p => p.GetValue(relayInformationVM) != null))
            {
                relayInformationList = await relayInformationService.FilterList(relayInformationVM);
                foreach (var item in relayInformationList)
                {
                    item.User = Encryption.Encrypt(item.User);
                }
            }

            RelayInformationFilterParams filterParams = mapper.Map<RelayInformationFilterParams>(relayInformationVM);
            int pageNumber = page ?? 1;
            IPagedList<RelayInformation> relayInformationPagedList = new PagedList<RelayInformation>(relayInformationList, pageNumber, (int)ViewBag.PageSize);
            filterParams.RelayInformationList = relayInformationPagedList;

            return View(filterParams);
        }

        private void ClearInvalidProperties(RelayInformationFilterParams relayInformationVM, int minCharLimit)
        {
            var properties = relayInformationVM.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(string))
                {
                    var value = property.GetValue(relayInformationVM) as string;
                    if (value != null && value.Length < minCharLimit)
                    {
                        property.SetValue(relayInformationVM, null);
                    }
                }
            }
        }

        private void RestoreFilterTexts(RelayInformationFilterParams relayInformationVM)
        {
            relayInformationVM.FilterTextTm = relayInformationVM.CurrentFilterTm;
            relayInformationVM.FilterTextKv = relayInformationVM.CurrentFilterKv;
            relayInformationVM.FilterTextHucre = relayInformationVM.CurrentFilterHucre;
            relayInformationVM.FilterTextFider = relayInformationVM.CurrentFilterFider;
            relayInformationVM.FilterTextIp = relayInformationVM.CurrentFilterIp;
            relayInformationVM.FilterTextRole = relayInformationVM.CurrentFilterRole;
        }

        private void RestoreCurrentFilterTexts(RelayInformationFilterParams relayInformationVM)
        {
            relayInformationVM.CurrentFilterTm = relayInformationVM.FilterTextTm;
            relayInformationVM.CurrentFilterKv = relayInformationVM.FilterTextKv;
            relayInformationVM.CurrentFilterHucre = relayInformationVM.FilterTextHucre;
            relayInformationVM.CurrentFilterFider = relayInformationVM.FilterTextFider;
            relayInformationVM.CurrentFilterIp = relayInformationVM.FilterTextIp;
            relayInformationVM.CurrentFilterRole = relayInformationVM.FilterTextRole;
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RelayInformationDTO model)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            string username = claimUser.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userService.GetUserByUsername(username);
            try
            {
                model.Password = Encryption.Encrypt(model.Password);
                bool result = await relayInformationService.CreateRelayInformation(model);
                if (result)
                {
                    await userLogService.CreateData(user);
                    await userLogService.InformationLog(user, "Yeni Veri Girişi", $"Tm_Kv_Hücre : {model.TmKvHucre} IP : {model.IP} Röle Model : {model.RoleModel} Röle eklendi.");
                    return RedirectToAction(nameof(List));
                }
                return View(model);
            }
            catch (Exception ex)
            {
                await userLogService.ErrorLog(user, ex.ToString(), "Yeni Veri Girişi", "Alanları kontrol edin.");
                ViewBag.ErrorMessage = "Bir hata oluştu. Alanları kontrol edin. ";
                return View(model);
            }

        }

        public async Task<ActionResult> DeleteAsync(int ID)
        {
            var relayInformation = await relayInformationService.GetRelayInformationById(ID);
            relayInformation.User = Encryption.Encrypt(relayInformation.User);
            return View(relayInformation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int ID)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            string username = claimUser.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userService.GetUserByUsername(username);
            try
            {
                var relayInformation = await relayInformationService.GetRelayInformationById(ID);
                bool result = await relayInformationService.DeleteRelayInformation(relayInformation.ID);
                if (result)
                {
                    await userLogService.DeleteData(user);
                    await userLogService.InformationLog(user, "Veri Silme", $"Tm_Kv_Hücre : {relayInformation.TmKvHucre} IP : {relayInformation.IP} Röle Model : {relayInformation.RoleModel} Röle silindi.");
                    return RedirectToAction(nameof(List));
                }
                return View(relayInformation);
            }
            catch (Exception ex)
            {
                await userLogService.ErrorLog(user, ex.ToString(), "Veri Silme", "Veri silme başarısız.");
                return View();
            }
        }

        public async Task<ActionResult> Edit(int ID)
        {
            var relayInformation = await relayInformationService.GetRelayInformationById(ID);
            RelayInformationDTO relayInformationDTO = new RelayInformationDTO();
            relayInformationDTO = mapper.Map(relayInformation, relayInformationDTO);
            relayInformationDTO.Password = Encryption.Decrypt(relayInformationDTO.Password);
            return View(relayInformationDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int ID, RelayInformationDTO model)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            string username = claimUser.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userService.GetUserByUsername(username);
            try
            {
                bool resultHistory = true;
                var data = await relayInformationService.GetRelayInformationById(ID);
                if (model.IP != data.IP)
                {
                    HistoryOfChange historyOfChange = new HistoryOfChange();
                    historyOfChange.OldIP = data.IP;
                    historyOfChange.RelayInformationId = ID;
                    historyOfChange.ChangedDate = DateTime.Now;
                    historyOfChange.NewIP = model.IP;
                    resultHistory = await historyOfChangeService.Create(historyOfChange);
                }
                model.Password = Encryption.Encrypt(model.Password);
                bool resultRelayInformation = await relayInformationService.UpdateRelayInformation(ID, model);
                bool resultDisturbance = await disturbanceService.UpdateByDataIdList(model);

                if (resultRelayInformation && resultDisturbance && resultHistory)
                {
                    await userLogService.UpdateData(user);
                    await userLogService.InformationLog(user, "Veri Güncelleme", $"Tm_Kv_Hücre : {model.TmKvHucre} IP : {model.IP} Röle Model : {model.RoleModel} Röle güncellendi.");
                    return RedirectToAction(nameof(List));
                }
                return View(model);
            }
            catch (Exception ex)
            {
                await userLogService.ErrorLog(user, ex.ToString(), "Veri Güncelleme", "Alanları kontrol edin.");
                ViewBag.ErrorMessage = "Bir hata oluştu. Alanları kontrol edin. ";
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> ImportExcel(IFormFile excelFile)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            string username = claimUser.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userService.GetUserByUsername(username);
            List<RelayInformation> relayInformations = new List<RelayInformation>();
            List<RelayInformation> duplicateRelays = new List<RelayInformation>();
            List<RelayInformation> incompatibleRelays = new List<RelayInformation>();
            List<RelayInformation> blankRowRelays = new List<RelayInformation>();
            string excelInputError = "";



            if (excelFile != null && excelFile.Length > 0)
            {
                // Dosya uzantısını kontrol etme
                if (Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        relayInformations = await ReadDataFromExcel(excelFile);
                        var result = await relayInformationService.AddDataList(relayInformations, user);
                        duplicateRelays = result.duplicateRelays;
                        incompatibleRelays = result.incompatibleRelays;
                        blankRowRelays = result.blankRowRelays;
                        await userLogService.ImportExcel(user);
                    }
                    catch (Exception ex)
                    {
                        await userLogService.ErrorLog(user, ex.ToString(), "Excel İmport", "Hata oluştu.");
                    }
                }
                else
                {
                    // Eğer dosya uzantısı .xlsx değilse hata mesajı döndür
                    excelInputError = "Lütfen sadece Excel (.xlsx) dosyalarını yükleyin.";

                }
            }
            else
            {
                // Dosya yüklenmediyse hata mesajı döndür
                excelInputError = "Lütfen bir dosya seçin.";
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60) // 60 saniye boyunca sakla
            };

            memoryCache.Set("DuplicateRelaysCount", duplicateRelays.Count, cacheEntryOptions);
            memoryCache.Set("IncompatibleRelaysCount", incompatibleRelays.Count, cacheEntryOptions);
            memoryCache.Set("BlankRowRelaysCount", blankRowRelays.Count, cacheEntryOptions);
            memoryCache.Set("ExcelInputError", excelInputError, cacheEntryOptions);

            return RedirectToAction("List");
        }

        public async Task<List<RelayInformation>> ReadDataFromExcel(IFormFile excelFile)
        {
            List<RelayInformation> dataList = new List<RelayInformation>();

            using (var workbook = new XLWorkbook(excelFile.OpenReadStream()))
            {
                var worksheet = workbook.Worksheet(1); // İlk çalışma sayfasını seçin

                int rowCount = worksheet.RowsUsed().Count();

                for (int row = 2; row <= rowCount; row++) // İlk satırı başlık olarak kabul edin, bu yüzden 2'den başlıyoruz
                {
                    RelayInformation data = new RelayInformation();
                    data.TmNo = GetValueOrDefault(worksheet.Cell(row, 1).Value);
                    data.kV = GetValueOrDefault(worksheet.Cell(row, 2).Value);
                    data.HucreNo = GetValueOrDefault(worksheet.Cell(row, 3).Value);
                    data.TmKvHucre = $"{data.TmNo}_{data.kV}_{data.HucreNo}";
                    data.FiderName = GetValueOrDefault(worksheet.Cell(row, 5).Value);
                    data.IP = GetValueOrDefault(worksheet.Cell(row, 6).Value);
                    data.RoleModel = GetValueOrDefault(worksheet.Cell(row, 7).Value);
                    data.User = GetValueOrDefault(worksheet.Cell(row, 8).Value);
                    string password = GetValueOrDefault(worksheet.Cell(row, 9).Value);
                    password = string.IsNullOrEmpty(password) ? "" : Encryption.Encrypt(password);
                    data.Password = password;
                    data.Port = 21;
                    data.Path = GetPathByRoleModel(data.RoleModel);
                    dataList.Add(data);
                }
            }
            return dataList;
        }

        private string GetValueOrDefault(XLCellValue cellValue)
        {
            return cellValue.IsBlank ? string.Empty : cellValue.ToString();
        }



        private string GetPathByRoleModel(string roleModel)
        {
            string upperCaseRoleModel = roleModel.ToUpper();

            if (upperCaseRoleModel.Contains("ABB") || upperCaseRoleModel.Contains("REC") || upperCaseRoleModel.Contains("REF"))
            {
                return "COMTRADE/";
            }
            else if (upperCaseRoleModel.Contains("ION") || upperCaseRoleModel.Contains("SCHNEIDER"))
            {
                return "COMTRADE_1/";
            }
            else if (string.IsNullOrEmpty(upperCaseRoleModel))
            {
                return null;
            }
            else
            {
                return "Bilinmiyor";
            }
        }

        [HttpPost]
        public IActionResult DecryptData(string text)
        {
            string decryptedText = Encryption.Decrypt(text);
            var result = new
            {
                decrypt = decryptedText,
            };
            return Json(result);
        }


    }
}