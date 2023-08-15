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
    public class MyJob : IJob
    {
        private readonly FtpService ftpService;
        private readonly CsvConverterService csvConverterService;
        private readonly sFtpService sFtpService;

        public MyJob(FtpService ftpService, CsvConverterService csvConverterService, sFtpService sFtpService)
        {
            this.ftpService = ftpService;
            this.csvConverterService = csvConverterService;
            this.sFtpService = sFtpService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                Serilog.Log.Debug("\n********************** Uygulama başladı...********************** \n");

                JobDataMap jobDataMap = context.JobDetail.JobDataMap;
                string pythonExePath = jobDataMap.GetString("PythonExePath");
                string rmsScript = jobDataMap.GetString("RmsScript");
                string instantScript = jobDataMap.GetString("InstantScript");
                string comtradeFilesPath = jobDataMap.GetString("ComtradeFilesPath");
                string csvFilesPath = jobDataMap.GetString("CsvFilesPath");
                string host = jobDataMap.GetString("HostName");
                string username = jobDataMap.GetString("Username");
                string password = jobDataMap.GetString("Password");
                string homeDirectory = jobDataMap.GetString("HomeDirectory");
                int port = jobDataMap.GetInt("Port");
                string sshHostKey = jobDataMap.GetString("SshHostKeyFingerprint");

                await ftpService.DownloadCfgAndDatFilesEfCoreAsync(comtradeFilesPath);
                await csvConverterService.ConvertDatAndCfgFilesToCsvAsRMSDataAsync(pythonExePath, rmsScript, csvFilesPath);
                await csvConverterService.ConvertDatAndCfgFilesToCsvAsInstantData(pythonExePath, instantScript, csvFilesPath);
                await sFtpService.PutCsvFiles(host, username, password, homeDirectory, port, sshHostKey);

                
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
            }
            finally
            {
                Serilog.Log.Debug("\n********************** Uygulama kapandı...********************** \n");
                //Serilog.Log.CloseAndFlush();
            }
        }
        }
}
