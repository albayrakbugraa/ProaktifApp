using ComtradeApp.Service;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComtradeApp.Jobs
{
    [DisallowConcurrentExecution]
    public class sFtpJob : IJob
    {
        private readonly sFtpService sFtpService;

        public sFtpJob(sFtpService sFtpService)
        {
            this.sFtpService = sFtpService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobDataMap jobDataMap = context.JobDetail.JobDataMap;
                string host = jobDataMap.GetString("HostName");
                string username = jobDataMap.GetString("Username");
                string password = jobDataMap.GetString("Password");
                string homeDirectory = jobDataMap.GetString("HomeDirectory");
                int port = jobDataMap.GetInt("Port");
                string sshHostKey = jobDataMap.GetString("SshHostKeyFingerprint");

                await sFtpService.PutCsvFiles(host, username, password, homeDirectory, port, sshHostKey);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
            }
        }
    }
}
