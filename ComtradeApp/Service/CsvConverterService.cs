using ComtradeApp.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComtradeApp.Service
{
    /// <summary>
    /// Bu sınıfın temel amacı, belirli veri türlerini içeren .dat ve .cfg dosyalarını Python betiği aracılığıyla CSV (Comma-Separated Values) formatına dönüştürmek ve sonuçları kaydetmektir. 
    /// </summary>
    public class CsvConverterService
    {
        private readonly DisturbanceRepository disturbanceRepository;
        private readonly HistoryOfChangeRepository historyOfChangeRepository;
        private readonly LogRepository log;
        public CsvConverterService(DisturbanceRepository disturbanceRepository, HistoryOfChangeRepository historyOfChangeRepository, LogRepository log)
        {
            this.disturbanceRepository = disturbanceRepository;
            this.historyOfChangeRepository = historyOfChangeRepository;
            this.log = log;
        }

        /// <summary>
        /// Bu metot .dat ve .cfg dosyalarını alır, bunları Python betiği kullanarak  RMS (Root Mean Square) verilerini içeren CSV formatına dönüştürür ve sonuçları verilen bir yol altında kaydeder.
        /// Bu metot, önce bir log kaydı oluşturur ve ardından DisturbanceRepository ile veritabanından arıza kaydı (disturbance) verilerini alır. Alınan veriler üzerinde döngü oluşturur ve her bir bozulma verisi için şu işlemleri gerçekleştirir:
        /// İlgili .dat ve .cfg dosyalarının yollarını oluşturur.
        /// Belirli bir CSV dosyasının ismini oluşturur.
        /// CSV dosyasının daha önce oluşturulup oluşturulmadığını kontrol eder.
        /// Eğer CSV dosyası daha önce oluşturulmamışsa, Python betiği kullanarak.dat ve .cfg dosyalarını dönüştürür.
        /// Dönüştürme işlemi hata içeriyorsa, bu hataları kaydeder.
        /// Dönüşen CSV dosyasının yolunu günceller ve bu işlem hata içermezse veritabanını günceller.
        /// </summary>
        /// <param name="pythonExePath">Python yürütücüsünün yolunu içerir.</param>
        /// <param name="script">Kullanılacak Python betiğinin yolunu içerir.</param>
        /// <param name="csvFilesPath">CSV dosyalarının kaydedileceği dizin yolu.</param>
        /// <returns>Bu metot, async ve asenkron bir işlem gerçekleştirir, ancak herhangi bir değer döndürmez. Bu tür metotlar, işlemleri başlatmak, bitirmek veya asenkron olarak yürütmek için kullanılır, ancak bir sonuç döndürmezler.</returns>
        public async Task ConvertDatAndCfgFilesToCsvAsRMSDataAsync(string pythonExePath, string script, string csvFilesPath)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            log.InformationLog("RMS formatında CSV dönüştürücü başladı.", "CSV Dönüştürücü", threadId);

            Serilog.Log.Information("RMS formatında CSV dönüştürücü başladı..");
            try
            {
                var disturbances = await disturbanceRepository.GetAll();
                foreach (var item in disturbances)
                {
                    var historyOfChange = await historyOfChangeRepository.GetByRelayInformationId(item.ID);
                    if (historyOfChange != null && item.IP == historyOfChange.NewIP)
                    {
                        await historyOfChangeRepository.UpdateFolderNames(item, csvFilesPath);
                    }
                    string tmNoFolderName = item.TmNo;
                    string hucreNoFolderName = item.HucreNo;
                    string destFolderPath = Path.Combine(csvFilesPath, $"{item.IP}-{item.TmKvHucre}");
                    string format = "ddMMyy,HHmmssfff";
                    string dateTime = item.FaultTimeStart.ToString(format);
                    string cfgFile = item.CfgFilePath;
                    string datFile = item.DatFilePath;
                    string csvName = destFolderPath + $"\\{dateTime},{item.HucreNo} {item.TmNo},{item.IP},RMS.csv";

                    if (!File.Exists(csvName))
                    {
                        if (!Directory.Exists(destFolderPath))
                        {
                            Directory.CreateDirectory(destFolderPath);
                        }
                        try
                        {
                            var psi = new ProcessStartInfo();
                            psi.FileName = pythonExePath;
                            psi.Arguments = $"\"{script}\" \"{cfgFile}\" \"{datFile}\" \"{csvName}\"";
                            psi.UseShellExecute = false;
                            psi.CreateNoWindow = true;
                            psi.RedirectStandardOutput = true;
                            psi.RedirectStandardError = true;

                            using (var process = Process.Start(psi))
                            {
                                using (var reader = process.StandardError)
                                {
                                    string error = reader.ReadToEnd();
                                    if (!string.IsNullOrEmpty(error))
                                    {
                                        log.ErrorLog("Python hatası.", error, "Python", threadId);
                                        Serilog.Log.Error("Python hatası: {Error}", error);
                                    }
                                }

                            }

                        }
                        catch (Exception ex)
                        {
                            log.ErrorLog("Hata oluştu.", ex.ToString(), "Python", threadId);
                            Serilog.Log.Error(ex, "Hata oluştu: {ErrorMessage}", ex.Message);
                        }
                        if (File.Exists(csvName))
                        {
                            item.RmsDataPath = csvName;
                            bool result = disturbanceRepository.Update(item);
                            if (!result)
                            {
                                log.ErrorLog("RMS formatındaki CSV dosyasının yolu kaydedilemedi.", "Database hatası", "CSV Dönüştürücü", threadId);
                                Serilog.Log.Error("RMS formatındaki CSV dosyasının yolu kaydedilemedi.");
                            }
                            Serilog.Log.Information($"CSV dosyası RMS formatında başarılı şekilde oluşturuldu : {csvName}");
                        }
                        else
                        {
                            log.ErrorLog($"CSV dosyası oluşturulamadı : {cfgFile} - {datFile}", "Dönüştürme hatası", "CSV Dönüştürücü", threadId);
                            Serilog.Log.Error($"CSV dosyası oluşturulamadı : {cfgFile} - {datFile} ");
                        }
                    }
                    else
                    {
                        log.WarningLog($"CSV dosyası daha önce dönüştürülmüş: {csvName}", "CSV Dönüştürücü", threadId);
                        Serilog.Log.Information($"CSV dosyası daha önce dönüştürülmüş: {csvName}");
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorLog("Veritabanına bağlanırken hata oluştu!", ex.ToString(), "CSV Dönüştürücü", threadId);
                Serilog.Log.Error(ex, "Veritabanına bağlanırken hata oluştu!");
            }
            finally
            {
                log.InformationLog("RMS formatındaki CSV dönüştürücü kapandı.", "CSV Dönüştürücü", threadId);
                Serilog.Log.Information("RMS formatındaki CSV dönüştürücü kapandı.. \n");
            }
        }

        /// <summary>
        /// Bu metot, RMS verileri yerine anlık (instantaneous) verileri içeren .dat ve .cfg dosyalarını dönüştürmek için benzer bir işlemi gerçekleştirir. İşlevi ve kullanımı, yukarıda açıklanan ConvertDatAndCfgFilesToCsvAsRMSDataAsync metoduna benzerdir, ancak dönüştürülen verinin türü farklıdır.
        /// </summary>
        /// <param name="pythonExePath">Python yürütücüsünün yolunu içerir.</param>
        /// <param name="script">Kullanılacak Python betiğinin yolunu içerir.</param>
        /// <param name="csvFilesPath">CSV dosyalarının kaydedileceği dizin yolu.</param>
        /// <returns>Bu metot, async ve asenkron bir işlem gerçekleştirir, ancak herhangi bir değer döndürmez. Bu tür metotlar, işlemleri başlatmak, bitirmek veya asenkron olarak yürütmek için kullanılır, ancak bir sonuç döndürmezler.</returns>
        public async Task ConvertDatAndCfgFilesToCsvAsInstantData(string pythonExePath, string script, string csvFilesPath)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            log.InformationLog("Instantaneous formatında CSV dönüştürücü başladı.", "CSV Dönüştürücü", threadId);
            Serilog.Log.Information("Instantaneous formatında CSV dönüştürücü başladı..");
            try
            {
                var disturbances = await disturbanceRepository.GetAll();
                foreach (var item in disturbances)
                {
                    var historyOfChange = await historyOfChangeRepository.GetByRelayInformationId(item.ID);
                    if (historyOfChange != null && item.IP == historyOfChange.NewIP)
                    {
                        await historyOfChangeRepository.UpdateFolderNames(item, csvFilesPath);
                    }
                    string tmNoFolderName = item.TmNo;
                    string hucreNoFolderName = item.HucreNo;
                    string destFolderPath = Path.Combine(csvFilesPath, $"{item.IP}-{item.TmKvHucre}");
                    string format = "ddMMyy,HHmmssfff";
                    string dateTime = item.FaultTimeStart.ToString(format);
                    string cfgFile = item.CfgFilePath;
                    string datFile = item.DatFilePath;
                    string csvName = destFolderPath + $"\\{dateTime},{item.HucreNo} {item.TmNo},{item.IP},Instantaneous.csv";

                    if (!File.Exists(csvName)) // Dosya zaten varsa dönüştürme işlemi yapma
                    {
                        if (!Directory.Exists(destFolderPath))
                        {
                            Directory.CreateDirectory(destFolderPath);
                        }
                        try
                        {
                            var psi = new ProcessStartInfo();
                            psi.FileName = pythonExePath;
                            psi.Arguments = $"\"{script}\" \"{cfgFile}\" \"{datFile}\" \"{csvName}\"";
                            psi.UseShellExecute = false;
                            psi.CreateNoWindow = true;
                            psi.RedirectStandardOutput = true;
                            psi.RedirectStandardError = true;

                            using (var process = Process.Start(psi))
                            {
                                using (var reader = process.StandardError)
                                {
                                    string error = reader.ReadToEnd();
                                    if (!string.IsNullOrEmpty(error))
                                    {
                                        log.ErrorLog("Python hatası.", error, "Python", threadId);
                                        Serilog.Log.Error("Python hatası: {Error}", error);
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            log.ErrorLog("Hata oluştu.", ex.ToString(), "Python", threadId);
                            Serilog.Log.Error(ex, "Hata oluştu: {ErrorMessage}", ex.Message);
                        }
                        if (File.Exists(csvName))
                        {
                            item.InstantDataPath = csvName;
                            bool result = disturbanceRepository.Update(item);
                            if (!result)
                            {
                                log.ErrorLog("Instant formatındaki CSV dosyasının yolu kaydedilemedi.", "Database hatası", "CSV Dönüştürücü", threadId);
                                Serilog.Log.Error("Instant formatındaki CSV dosyasının yolu kaydedilemedi.");
                            }

                            Serilog.Log.Information($"CSV dosyası Instantaneous formatında başarılı şekilde oluşturuldu : {csvName}");
                        }
                        else
                        {
                            log.ErrorLog($"CSV dosyası oluşturulamadı : {cfgFile} - {datFile}", "Dönüştürme hatası", "CSV Dönüştürücü", threadId);
                            Serilog.Log.Error($"CSV dosyası oluşturulamadı : {cfgFile} - {datFile} ");
                        }
                    }
                    else
                    {
                        log.WarningLog($"CSV dosyası daha önce dönüştürülmüş: {csvName}", "CSV Dönüştürücü", threadId);
                        Serilog.Log.Information($"CSV dosyası daha önce dönüştürülmüş: {csvName}");
                    }

                }
            }
            catch (Exception ex)
            {
                log.ErrorLog("Veritabanına bağlanırken hata oluştu!", ex.ToString(), "CSV Dönüştürücü", threadId);
                Serilog.Log.Error(ex, "Veritabanına bağlanırken hata oluştu!");
            }
            finally
            {
                log.InformationLog("Instant formatındaki CSV dönüştürücü kapandı.", "CSV Dönüştürücü", threadId);
                Serilog.Log.Information("Instant formatındaki CSV dönüştürücü kapandı.. \n");
            }
        }


    }
}
