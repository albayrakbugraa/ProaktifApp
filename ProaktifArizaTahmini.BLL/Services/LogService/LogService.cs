using ProaktifArizaTahmini.BLL.Models.RequestModel;
using ProaktifArizaTahmini.CORE.Entities;
using ProaktifArizaTahmini.CORE.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.BLL.Services.LogService
{
    public class LogService : ILogService
    {
        private readonly ILogReposiory logReposiory;

        public LogService(ILogReposiory logReposiory)
        {
            this.logReposiory = logReposiory;
        }

        public async Task<List<ServiceLog>> FilterList(LogModel logModel)
        {
            var serviceLogs = await logReposiory.GetAll();

            var filteredData = serviceLogs.Where(data =>
                    (string.IsNullOrEmpty(logModel.FilterLogLevel) || data.LogLevel != null && data.LogLevel.ToUpper().Contains(logModel.FilterLogLevel.ToUpper())) &&
                    (string.IsNullOrEmpty(logModel.FilterException) || data.Exception != null && data.Exception.ToUpper().Contains(logModel.FilterException.ToUpper())) &&
                    (string.IsNullOrEmpty(logModel.FilterServiceName) || data.ServiceName != null && data.ServiceName.ToUpper().Contains(logModel.FilterServiceName.ToUpper())) &&
                    (string.IsNullOrEmpty(logModel.FilterMessage) || data.Message != null && data.Message.ToUpper().Contains(logModel.FilterMessage.ToUpper())) &&
                    data.TimeStamp >= logModel.FilterFaultTimeStart && data.TimeStamp <= logModel.FilterFaultTimeEnd
                )
                .ToList();

            return filteredData;
        }

        public async Task<List<string>> GetLogLevels()
        {
            return await logReposiory.GetUniqueLogLevels();
        }

        public async Task<List<ServiceLog>> GetServiceLogs()
        {
            var serviceLogs = await logReposiory.GetFilteredList(
                 selector: x => new ServiceLog
                 {
                     ID= x.ID,
                     LogLevel = x.LogLevel,
                     Exception = x.Exception,
                     Message = x.Message,
                     ServiceName = x.ServiceName,
                     TimeStamp = x.TimeStamp,
                     ThreadID = x.ThreadID
                 },
                 expression : null,
                 orderBy: x => x.OrderByDescending(x => x.TimeStamp)
                 );
            return serviceLogs;
        }

        public async Task<List<string>> GetServiceNames()
        {
            return await logReposiory.GetUniqueServiceNames();
        }
    }
}
