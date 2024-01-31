using ComtradeApp.Repository;
using ComtradeApp.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using static Quartz.Logging.OperationName;

namespace ComtradeApp.Job
{
    /// <summary>
    /// Quartz.NET iş (job) tanımlamasını içerir. Quartz.NET, zamanlanmış görevleri veya işleri belirli aralıklarla veya zamanlamalara göre çalıştırmak için kullanılan bir kütüphanedir. İşler, belirli bir görevi gerçekleştiren kod bloklarıdır ve bir zamanlayıcı veya tetikleyici (trigger) tarafından belirli bir sıklıkta veya zaman diliminde çalıştırılırlar.
    /// Bu sınıf IJob arayüzünü uygular, bu nedenle Quartz.NET tarafından çalıştırılabilir bir işi temsil eder.
    /// FtpAndCsvJob, Quartz.NET kullanarak belirli bir süre veya tetikleyiciye göre çalıştırılan bir iş(job) sınıfıdır.Bu iş, aşağıdaki hizmetleri kullanarak FTP'den veri indirme, bu veriyi CSV dosyalarına dönüştürme ve işleme görevlerini gerçekleştirir:
    /// FtpService: FTP sunucusundan veri indirme görevlerini yönetir. İndirilen veriler, belirtilen bir yerel dizine kaydedilir.
    /// CsvConverterService: İndirilen verileri CSV dosyalarına dönüştürme görevlerini yönetir.Hem RMS(root mean square) formatında hem de anlık veri(instant data) formatında dönüştürme işlemlerini yürütür.
    /// RelayInformationRepository: İşlenecek olan röle bilgilerini veritabanından alır. Bu röle bilgileri, hangi rölelerin işleneceğini ve ne şekilde işleneceğini belirlemek için kullanılır.
    /// IConfiguration: Uygulamanın yapılandırma bilgilerine erişim sağlar.Özellikle çoklu iş parçacığı kullanımı ve iş süresi gibi ayarlar bu yapılandırmadan okunur.
    /// [DisallowConcurrentExecution] özniteliği ile işin eş zamanlı çalışmasının engellendiğini belirtir. Yani bu iş, aynı anda yalnızca bir kez çalıştırılabilir.
    /// FtpAndCsvJob, birden fazla iş parçacığını etkili bir şekilde kullanarak büyük miktarda veriyi işlemek için tasarlanmıştır. Özellikle FTP'den veri indirme ve bu veriyi dönüştürme işlemlerini çoklu görevler aracılığıyla aynı anda gerçekleştirerek performansı artırır. Bu sayede işlerin daha hızlı ve paralel bir şekilde işlenmesini sağlar.
    /// </summary>
    [DisallowConcurrentExecution]
    public class FtpAndCsvJob : IJob
    {
        private readonly FtpService ftpService;
        private readonly CsvConverterService csvConverterService;
        private readonly RelayInformationRepository relationsRepository;
        private readonly IConfiguration configuration;

        public FtpAndCsvJob(FtpService ftpService, CsvConverterService csvConverterService, RelayInformationRepository relationsRepository, IConfiguration configuration)
        {
            this.ftpService = ftpService;
            this.csvConverterService = csvConverterService;
            this.relationsRepository = relationsRepository;
            this.configuration = configuration;
        }
        /// <summary>
        /// Belirli bir sayıda aynı anda çalışacak görev sayısını belirleme (bu, çoklu iş parçacığı kullanımını kontrol etmeye yardımcı olur).
        /// Röle bilgilerini parçalara bölmek(chunks) için relationsRepository.GetByStatusInChunks metotunu kullanma. Bu, birden fazla görevin farklı röleler üzerinde çalışmasını sağlar.
        /// Her bir parçayı ayrı bir görev olarak oluşturma ve aynı anda çalıştırma. Her bir görev, ftpService.DownloadCfgAndDatFilesEfCoreAsync, csvConverterService.ConvertDatAndCfgFilesToCsvAsRMSDataAsync ve csvConverterService.ConvertDatAndCfgFilesToCsvAsInstantData metodlarını sırasıyla kullanarak belirli işlemleri gerçekleştirir.
        /// Tüm görevlerin bitmesini beklemek ve ne kadar sürede tamamlandığını ölçmek.
        /// Hata durumlarını ele almak ve Serilog ile hata günlüklemesi yapmak.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                int numberOfTasks = configuration.GetValue<int>("MultiThreading:NumberOfTasks");
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var relays = await relationsRepository.GetByStatusInChunks(true, numberOfTasks);
                JobDataMap jobDataMap = context.JobDetail.JobDataMap;
                string pythonExePath = jobDataMap.GetString("PythonExePath");
                string rmsScript = jobDataMap.GetString("RmsScript");
                string instantScript = jobDataMap.GetString("InstantScript");
                string comtradeFilesPath = jobDataMap.GetString("ComtradeFilesPath");
                string csvFilesPath = jobDataMap.GetString("CsvFilesPath");

                List<Task> tasks = new List<Task>();

                for (int index = 0; index < relays.Count; index++)
                {
                    int currentIndex = index;
                    Task task = Task.Run(async () =>
                    {
                        await ftpService.DownloadCfgAndDatFilesEfCoreAsync(comtradeFilesPath, relays[currentIndex]);
                        await csvConverterService.ConvertDatAndCfgFilesToCsvAsRMSDataAsync(pythonExePath, rmsScript, csvFilesPath);
                        await csvConverterService.ConvertDatAndCfgFilesToCsvAsInstantData(pythonExePath, instantScript, csvFilesPath);
                    });

                    tasks.Add(task);
                }

                await Task.WhenAll(tasks);
                stopwatch.Stop();
                Console.WriteLine($"Geçen Süre: {stopwatch.ElapsedMilliseconds} milisaniye");
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
            }
        }
    }
}
