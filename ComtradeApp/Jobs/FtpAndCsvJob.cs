using ComtradeApp.Repository;
using ComtradeApp.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComtradeApp.Job
{
    [DisallowConcurrentExecution]
    public class FtpAndCsvJob : IJob
    {
        private readonly FtpService ftpService;
        private readonly CsvConverterService csvConverterService;

        public FtpAndCsvJob(FtpService ftpService, CsvConverterService csvConverterService)
        {
            this.ftpService = ftpService;
            this.csvConverterService = csvConverterService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobDataMap jobDataMap = context.JobDetail.JobDataMap;
                string pythonExePath = jobDataMap.GetString("PythonExePath");
                string rmsScript = jobDataMap.GetString("RmsScript");
                string instantScript = jobDataMap.GetString("InstantScript");
                string comtradeFilesPath = jobDataMap.GetString("ComtradeFilesPath");
                string csvFilesPath = jobDataMap.GetString("CsvFilesPath");

                await ftpService.DownloadCfgAndDatFilesEfCoreAsync(comtradeFilesPath);
                await csvConverterService.ConvertDatAndCfgFilesToCsvAsRMSDataAsync(pythonExePath, rmsScript, csvFilesPath);
                await csvConverterService.ConvertDatAndCfgFilesToCsvAsInstantData(pythonExePath, instantScript, csvFilesPath);      
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
            }
        }
    }
}
