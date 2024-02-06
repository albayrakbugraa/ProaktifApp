using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using ProaktifArizaTahmini.BLL.Models.RequestModel;
using ProaktifArizaTahmini.BLL.Services.DisturbanceServices;
using ProaktifArizaTahmini.BLL.Services.LogService;
using ProaktifArizaTahmini.BLL.Services.RelayInformationServices;
using ProaktifArizaTahmini.CORE.Entities;
using System.Configuration;
using X.PagedList;

namespace ProaktifArizaTahmini.UI.Controllers
{
    [Authorize]
    public class LogController : Controller
    {
        private readonly ILogService logService;
        private readonly IMapper mapper;

        public LogController(ILogService logService, IMapper mapper)
        {
            this.logService = logService;
            this.mapper = mapper;
        }

        //public async Task<IActionResult> Index(int? page, int? pageSize)
        //{
        //    ViewData["ActivePage"] = "Logs";
        //    int defaultPageSize = 20;
        //    ViewBag.PageSize = pageSize ?? defaultPageSize;
        //    var serviceLogs = await logService.GetServiceLogs();
        //    int pageNumber = page ?? 1;
        //    IPagedList<ServiceLog> serviceLogs1 = new PagedList<ServiceLog>(serviceLogs, pageNumber, (int)ViewBag.PageSize);
        //    LogModel model = new LogModel();
        //    model.ServiceLogs = serviceLogs1;
        //    return View(model);
        //}

        public async Task<IActionResult> Index(LogModel logModel, int? page, int? pageSize)
        {
            ViewBag.LogLevels = await GetLogLevelSelectList();
            ViewBag.ServiceNames = await GetServiceNameSelectList();
            ViewData["ActivePage"] = "Logs";
            int defaultPageSize = 20;
            ViewBag.PageSize = pageSize ?? defaultPageSize;
            List<ServiceLog> serviceLogList = new List<ServiceLog>();

            var properties = logModel.GetType().GetProperties().Where(p => !p.Name.StartsWith("Current"));

            foreach (var property in properties)
            {
                // Özellik adı "time" ile bitiyorsa veya içeriyorsa doğrulamayı atla
                if (property.Name.Contains("time", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
            }
            if (properties.Any(p => p.GetValue(logModel) != null) && page == null)
            {
                page = 1;
            }
            else
            {
                serviceLogList = await logService.GetServiceLogs();
                RestoreFilterTexts(logModel);
            }
            RestoreCurrentFilterTexts(logModel);

            if (logModel.GetType().GetProperties().Any(p => p.GetValue(logModel) != null))
            {
                serviceLogList = await logService.FilterList(logModel);
            }

            LogModel filterParams = mapper.Map<LogModel>(logModel);
            int pageNumber = (page ?? 1);
            IPagedList<ServiceLog> logsPagedList = new PagedList<ServiceLog>(serviceLogList.OrderByDescending(x => x.TimeStamp), pageNumber, (int)ViewBag.PageSize);
            filterParams.ServiceLogs = logsPagedList;
            return View(filterParams);
        }

        public async Task<List<SelectListItem>> GetLogLevelSelectList()
        {
            var logLevels = await logService.GetLogLevels();

            List<SelectListItem> selectList = new List<SelectListItem>();
            foreach (var logLevel in logLevels)
            {
                selectList.Add(new SelectListItem { Text = logLevel, Value = logLevel });
            }
            return selectList;
        }
        public async Task<List<SelectListItem>> GetServiceNameSelectList()
        {
            var serviceNames = await logService.GetServiceNames();

            List<SelectListItem> selectList = new List<SelectListItem>();
            foreach (var name in serviceNames)
            {
                selectList.Add(new SelectListItem { Text = name, Value = name });
            }
            return selectList;
        }

        private void ClearInvalidProperties(LogModel logModel, int minCharLimit)
        {
            var properties = logModel.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(string))
                {
                    var value = property.GetValue(logModel) as string;
                    if (value != null && value.Length < minCharLimit)
                    {
                        property.SetValue(logModel, null);
                    }
                }
            }
        }

        private void RestoreFilterTexts(LogModel logModel)
        {
            logModel.FilterLogLevel = logModel.CurrentLogLevel;
            logModel.FilterServiceName = logModel.CurrentServiceName;
            logModel.FilterMessage = logModel.CurrentMessage;
            logModel.FilterException = logModel.CurrentException;
            logModel.FilterFaultTimeStart = DateTime.Parse(logModel.CurrentFaultTimeStart);
            logModel.FilterFaultTimeEnd = DateTime.Parse(logModel.CurrentFaultTimeEnd);
        }

        private void RestoreCurrentFilterTexts(LogModel logModel)
        {
            logModel.CurrentLogLevel = logModel.FilterLogLevel;
            logModel.CurrentServiceName = logModel.FilterServiceName;
            logModel.CurrentMessage = logModel.FilterMessage;
            logModel.CurrentException = logModel.FilterException;
            logModel.CurrentFaultTimeStart = logModel.FilterFaultTimeStart.ToString("yyyy-MM-ddTHH:mm");
            logModel.CurrentFaultTimeEnd = logModel.FilterFaultTimeEnd.ToString("yyyy-MM-ddTHH:mm");
        }
    }
}
