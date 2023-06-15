using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ProaktifArizaTahmini.BLL.Models.DTOs;
using ProaktifArizaTahmini.BLL.Models.RequestModel;
using ProaktifArizaTahmini.BLL.Services;
using ProaktifArizaTahmini.CORE.Entities;
using ProaktifArizaTahmini.UI.Models;
using System.Formats.Asn1;
using System.Globalization;
using X.PagedList;
using static NuGet.Packaging.PackagingConstants;

namespace ProaktifArizaTahmini.UI.Controllers
{
    public class MyDataController : Controller
    {
        private readonly IMyDataService myDataService;
        private readonly IDisturbanceService disturbanceService;
        private readonly IMapper mapper;

        public MyDataController(IMyDataService myDataService, IMapper mapper, IDisturbanceService disturbanceService)
        {
            this.myDataService = myDataService;
            this.mapper = mapper;
            this.disturbanceService = disturbanceService;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        public async Task<IActionResult> List(MyDataFilterParams myDataVM, int? page, int? pageSize)
        {
            ViewData["ActivePage"] = "MyData";
            int defaultPageSize = 20;
            ViewBag.PageSize = pageSize ?? defaultPageSize;
            List<MyData> myDataList = new List<MyData>();
            var properties = myDataVM.GetType().GetProperties().Where(p => !p.Name.StartsWith("Current"));
            if (myDataVM != null && properties.Any(p => p.GetValue(myDataVM) != null))
            {
                page = 1;
            }
            else
            {
                myDataList = await myDataService.GetMyDatas();
                if (myDataVM != null)
                {
                    myDataVM.FilterTextTm = myDataVM.CurrentFilterTm;
                    myDataVM.FilterTextKv = myDataVM.CurrentFilterKv;
                    myDataVM.FilterTextHucre = myDataVM.CurrentFilterHucre;
                    myDataVM.FilterTextFider = myDataVM.CurrentFilterFider;
                    myDataVM.FilterTextIp = myDataVM.CurrentFilterIp;
                    myDataVM.FilterTextRole = myDataVM.CurrentFilterRole;
                    myDataVM.FilterTextKullanici = myDataVM.CurrentFilterKullanici;
                    myDataVM.FilterTextSifre = myDataVM.CurrentFilterSifre;
                }
            }
            if (myDataVM != null)
            {
                myDataVM.CurrentFilterTm = myDataVM.FilterTextTm;
                myDataVM.CurrentFilterKv = myDataVM.FilterTextKv;
                myDataVM.CurrentFilterHucre = myDataVM.FilterTextHucre;
                myDataVM.CurrentFilterFider = myDataVM.FilterTextFider;
                myDataVM.CurrentFilterIp = myDataVM.FilterTextIp;
                myDataVM.CurrentFilterRole = myDataVM.FilterTextRole;
                myDataVM.CurrentFilterKullanici = myDataVM.FilterTextKullanici;
                myDataVM.CurrentFilterSifre = myDataVM.FilterTextSifre;
            }
            if (myDataVM != null && myDataVM.GetType().GetProperties().Any(p => p.GetValue(myDataVM) != null))
            {
                myDataList = await myDataService.FilterList(myDataVM);
            }
            MyDataFilterParams filterParams = new MyDataFilterParams();
            filterParams=mapper.Map(myDataVM, filterParams);
            int pageNumber = (page ?? 1);
            IPagedList<MyData> myDataPagedList = new PagedList<MyData>(myDataList, pageNumber, (int)ViewBag.PageSize);
            filterParams.MyDataList=myDataPagedList;
            return View(filterParams);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(MyDataDTO model)
        {
            try
            {
                bool result = await myDataService.CreateMyData(model);
                if (result)
                {
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
            var myData = await myDataService.GetMyDataByDataId(ID);
            return View(myData);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int ID)
        {
            try
            {
                var myData = await myDataService.GetMyDataByDataId(ID);
                bool result = await myDataService.DeleteMyData(myData.ID);
                if (result) return RedirectToAction(nameof(List));
                return View(myData);
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> Edit(int ID)
        {
            var myData = await myDataService.GetMyDataByDataId(ID);
            MyDataDTO myDataDTO = new MyDataDTO();
            myDataDTO = mapper.Map(myData, myDataDTO);
            return View(myDataDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int ID, MyDataDTO model)
        {
            try
            {
                bool resultMyData = await myDataService.UpdateMyData(ID, model);
                bool resultDisturbance = await disturbanceService.UpdateByDataIdList(model);
                if (resultMyData && resultDisturbance) return RedirectToAction(nameof(List));
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
            List<MyData> myDatas = new List<MyData>();
            if (excelFile != null && excelFile.Length > 0)
            {
                myDatas = ReadDataFromExcel(excelFile);
                bool resultMyData = await myDataService.AddDataList(myDatas);                
            }
            return RedirectToAction("List", myDatas);

        }
        private List<MyData> ReadDataFromExcel(IFormFile excelFile)
        {
            List<MyData> dataList = new List<MyData>();

            using (var package = new ExcelPackage(excelFile.OpenReadStream()))
            {
                var worksheet = package.Workbook.Worksheets[0]; // İlk çalışma sayfasını seçin (indeks 0'dan başlar)

                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++) // İlk satırı başlık olarak kabul edin, bu yüzden 2'den başlıyoruz
                {
                    MyData data = new MyData();                   
                    data.AvcilarTM = GetValueOrDefault<string>(worksheet.Cells[row, 1]?.Value);
                    data.kV = GetValueOrDefault<string>(worksheet.Cells[row, 2]?.Value);
                    data.HucreNo = GetValueOrDefault<string>(worksheet.Cells[row, 3]?.Value);
                    //data.TmKvHucre = GetValueOrDefault<string>(worksheet.Cells[row, 4]?.Value);
                    data.FiderName = GetValueOrDefault<string>(worksheet.Cells[row, 5]?.Value);
                    data.IP = GetValueOrDefault<string>(worksheet.Cells[row, 6]?.Value);
                    data.RoleModel = GetValueOrDefault<string>(worksheet.Cells[row, 7]?.Value);
                    data.User = GetValueOrDefault<string>(worksheet.Cells[row, 8]?.Value);
                    data.Password = GetValueOrDefault<string>(worksheet.Cells[row, 9]?.Value);
                    data.Port = 21;
                    if (data.RoleModel == "REF 615" || data.RoleModel == "ABB REF 615" || data.RoleModel == "REF 616")
                    {
                        data.Path = "COMTRADE/";
                    }
                    else if (data.RoleModel == "SIEMENS 7SJ80")
                    {
                        data.Path = "Bilinmiyor";
                    }
                    else if (data.RoleModel == "SIEMENS 7SJ82")
                    {
                        data.Path = "WsbRecordsArea/dynfs/ram/rcd/";
                    }
                    else
                    {
                        data.Path = "Bilinmiyor";
                    }
                    dataList.Add(data);
                }
            }
            return dataList;
        }
        private T GetValueOrDefault<T>(object value, T defaultValue = default(T))
        {
            if (value != null && value != DBNull.Value)
            {
                if (typeof(T) == typeof(string) && (value.GetType() == typeof(double) || value.GetType() == typeof(int)))
                {
                    return (T)(object)value.ToString();
                }

                return (T)value;
            }

            if (typeof(T) == typeof(string))
            {
                return (T)(object)"NULL";
            }

            return defaultValue;
        }

    }
}