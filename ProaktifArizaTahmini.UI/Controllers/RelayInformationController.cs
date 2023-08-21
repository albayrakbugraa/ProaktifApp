using AutoMapper;
using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProaktifArizaTahmini.BLL.Models.DTOs;
using ProaktifArizaTahmini.BLL.Models.RequestModel;
using ProaktifArizaTahmini.BLL.Services.DisturbanceServices;
using ProaktifArizaTahmini.BLL.Services.HistoryOfChangeServices;
using ProaktifArizaTahmini.BLL.Services.RelayInformationServices;
using ProaktifArizaTahmini.BLL.Services.UserLogServices;
using ProaktifArizaTahmini.BLL.Services.UserServices;
using ProaktifArizaTahmini.CORE.Entities;
using ProaktifArizaTahmini.CORE.IRepository;
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
        private readonly IUserLogService userLogService ;
        private readonly IUserService userService;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public RelayInformationController(IRelayInformationService relayInformationService, IMapper mapper, IDisturbanceService disturbanceService, IConfiguration configuration, IHistoryOfChangeService historyOfChangeService, IUserLogService userLogService, IUserService userService)
        {
            this.relayInformationService = relayInformationService;
            this.mapper = mapper;
            this.disturbanceService = disturbanceService;
            this.configuration = configuration;
            this.historyOfChangeService = historyOfChangeService;
            this.userLogService = userLogService;
            this.userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> List(RelayInformationFilterParams relayInformationVM, int? page, int? pageSize)
        {
            int minCharLimit = configuration.GetValue<int>("AppSettings:MinimumCharacterLimit");
            ViewData["ActivePage"] = "RelayInformation";

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
                RestoreFilterTexts(relayInformationVM);
            }
            RestoreCurrentFilterTexts(relayInformationVM);

            if (relayInformationVM.GetType().GetProperties().Any(p => p.GetValue(relayInformationVM) != null))
            {
                relayInformationList = await relayInformationService.FilterList(relayInformationVM);
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
            relayInformationVM.FilterTextKullanici = relayInformationVM.CurrentFilterKullanici;
            relayInformationVM.FilterTextSifre = relayInformationVM.CurrentFilterSifre;
        }

        private void RestoreCurrentFilterTexts(RelayInformationFilterParams relayInformationVM)
        {
            relayInformationVM.CurrentFilterTm = relayInformationVM.FilterTextTm;
            relayInformationVM.CurrentFilterKv = relayInformationVM.FilterTextKv;
            relayInformationVM.CurrentFilterHucre = relayInformationVM.FilterTextHucre;
            relayInformationVM.CurrentFilterFider = relayInformationVM.FilterTextFider;
            relayInformationVM.CurrentFilterIp = relayInformationVM.FilterTextIp;
            relayInformationVM.CurrentFilterRole = relayInformationVM.FilterTextRole;
            relayInformationVM.CurrentFilterKullanici = relayInformationVM.FilterTextKullanici;
            relayInformationVM.CurrentFilterSifre = relayInformationVM.FilterTextSifre;
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RelayInformationDTO model)
        {
            try
            {
                bool result = await relayInformationService.CreateRelayInformation(model);
                if (result)
                {
                    ClaimsPrincipal claimUser = HttpContext.User;
                    string username = claimUser.FindFirstValue(ClaimTypes.NameIdentifier);   
                    var user = await userService.GetUserByUsername(username);
                    await userLogService.CreateData(user);
                    return RedirectToAction(nameof(List));
                }
                return View(model);
            }
            catch
            {
                ViewBag.ErrorMessage = "Bir hata oluştu. Alanları kontrol edin. ";
                return View(model);
            }
        
        }

        public async Task<ActionResult> DeleteAsync(int ID)
        {
            var relayInformation = await relayInformationService.GetRelayInformationByDataId(ID);
            return View(relayInformation);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int ID)
        {
            try
            {
                var relayInformation = await relayInformationService.GetRelayInformationByDataId(ID);
                bool result = await relayInformationService.DeleteRelayInformation(relayInformation.ID);
                if (result)
                {
                    ClaimsPrincipal claimUser = HttpContext.User;
                    string username = claimUser.FindFirstValue(ClaimTypes.NameIdentifier);
                    var user = await userService.GetUserByUsername(username);
                    await userLogService.DeleteData(user);
                    return RedirectToAction(nameof(List));
                }
                return View(relayInformation);
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> Edit(int ID)
        {
            var relayInformation = await relayInformationService.GetRelayInformationByDataId(ID);
            RelayInformationDTO relayInformationDTO = new RelayInformationDTO();
            relayInformationDTO = mapper.Map(relayInformation, relayInformationDTO);
            return View(relayInformationDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int ID, RelayInformationDTO model)
        {
            try
            {
                var data = await relayInformationService.GetRelayInformationByDataId(ID);
                HistoryOfChange historyOfChange = new HistoryOfChange();
                historyOfChange.OldIP = data.IP;
                bool resultRelayInformation = await relayInformationService.UpdateRelayInformation(ID, model);
                bool resultDisturbance = await disturbanceService.UpdateByDataIdList(model);
                historyOfChange.RelayInformationId = ID;
                historyOfChange.ChangedDate = DateTime.Now;
                historyOfChange.NewIP = model.IP;
                bool resultHistory = await historyOfChangeService.Create(historyOfChange);
                if (resultRelayInformation && resultDisturbance && resultHistory) 
                {
                    ClaimsPrincipal claimUser = HttpContext.User;
                    string username = claimUser.FindFirstValue(ClaimTypes.NameIdentifier);
                    var user = await userService.GetUserByUsername(username);
                    await userLogService.UpdateData(user);
                    return RedirectToAction(nameof(List));
                } 
                return View(model);
            }
            catch
            {
                ViewBag.ErrorMessage = "Bir hata oluştu. Alanları kontrol edin. ";
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> ImportExcel(IFormFile excelFile)
        {
            List<RelayInformation> relayInformations = new List<RelayInformation>();
            if (excelFile != null && excelFile.Length > 0)
            {
                relayInformations = ReadDataFromExcel(excelFile);
                bool resultRelayInformation = await relayInformationService.AddDataList(relayInformations);
                ClaimsPrincipal claimUser = HttpContext.User;
                string username = claimUser.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await userService.GetUserByUsername(username);
                await userLogService.ImportExcel(user);
            }
            return RedirectToAction("List");
        }

        public List<RelayInformation> ReadDataFromExcel(IFormFile excelFile)
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
                    data.Password = GetValueOrDefault(worksheet.Cell(row, 9).Value);
                    data.Port = 21;
                    data.Path = GetPathByRoleModel(data.RoleModel);
                    dataList.Add(data);
                }
            }
            return dataList;
        }

        private string GetValueOrDefault(XLCellValue cellValue)
        {
            return cellValue.IsBlank ? "NULL" : cellValue.ToString();
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
            else if (upperCaseRoleModel == "SIEMENS 7SJ80")
            {
                return "Bilinmiyor";
            }
            else if (upperCaseRoleModel == "SIEMENS 7SJ82")
            {
                return "WsbRecordsArea/dynfs/ram/rcd/";
            }
            else
            {
                return "Bilinmiyor";
            }
        }


    }
}