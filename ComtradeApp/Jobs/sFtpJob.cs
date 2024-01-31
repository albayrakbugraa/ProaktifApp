using ComtradeApp.Service;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComtradeApp.Jobs
{
    /// <summary>
    /// Quartz.NET iş (job) tanımlamasını içerir. Quartz.NET, zamanlanmış görevleri veya işleri belirli aralıklarla veya zamanlamalara göre çalıştırmak için kullanılan bir kütüphanedir. İşler, belirli bir görevi gerçekleştiren kod bloklarıdır ve bir zamanlayıcı veya tetikleyici (trigger) tarafından belirli bir sıklıkta veya zaman diliminde çalıştırılırlar.
    /// Bu sınıf IJob arayüzünü uygular, bu nedenle Quartz.NET tarafından çalıştırılabilir bir işi temsil eder.
    /// [DisallowConcurrentExecution] özniteliği ile işin eş zamanlı çalışmasının engellendiğini belirtir. Yani bu iş, aynı anda yalnızca bir kez çalıştırılabilir.
    /// Belirtilen sFTP sunucusuna CSV dosyalarını yüklemek için "sFtpService" hizmetini kullanır.
    /// </summary>
    [DisallowConcurrentExecution]
    public class sFtpJob : IJob
    {
        private readonly sFtpService sFtpService;

        public sFtpJob(sFtpService sFtpService)
        {
            this.sFtpService = sFtpService;
        }
        /// <summary>
        /// IJob arayüzünden uygulanan bir metottur ve işin ana mantığını içerir. Bu metot, Quartz.NET tarafından işi çalıştırmak için çağrılır.
        /// Execute metodu, IJobExecutionContext nesnesi ile çalışır. Bu nesne, işin çalıştırılması sırasında çeşitli bilgilere ve parametrelere erişim sağlar.
        /// JobDataMap nesnesi, işin parametrelerini içerir. Bu parametreler, işin çalıştırılmasında kullanılır.
        /// "sFtpService" sınıfının PutCsvFiles metodu, bu parametrelerle çağrılır. Yani, iş bu metodu kullanarak sFTP sunucusuna CSV dosyalarını yükler.
        /// </summary>
        /// <param name="context">Quartz.NET kütüphanesinin bir parçası olan bir işin (job) çalıştırılması sırasında işle ilgili bilgilere ve işin parametrelerine erişim sağlayan bir nesnedir.</param>
        /// <returns></returns>
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
