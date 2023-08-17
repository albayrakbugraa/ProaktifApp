using ComtradeApp.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComtradeApp.Service
{
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

        public async Task ConvertDatAndCfgFilesToCsvAsRMSDataAsync(string pythonExePath, string script, string csvFilesPath)
        {
            log.InformationLog("RMS formatında CSV dönüştürücü başladı.", "CSV Dönüştürücü");

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
                                        log.ErrorLog("Python hatası.", error, "Python");
                                        Serilog.Log.Error("Python hatası: {Error}", error);
                                    }
                                }

                            }

                        }
                        catch (Exception ex)
                        {
                            log.ErrorLog("Hata oluştu.", ex.ToString(), "Python");
                            Serilog.Log.Error(ex, "Hata oluştu: {ErrorMessage}", ex.Message);
                        }
                        if (File.Exists(csvName))
                        {
                            item.RmsDataPath = csvName;
                            bool result = disturbanceRepository.Update(item);
                            if (!result)
                            {
                                log.ErrorLog("RMS formatındaki CSV dosyasının yolu kaydedilemedi.", "Database hatası", "CSV Dönüştürücü");
                                Serilog.Log.Error("RMS formatındaki CSV dosyasının yolu kaydedilemedi.");
                            }
                            Serilog.Log.Information($"CSV dosyası RMS formatında başarılı şekilde oluşturuldu : {csvName}");
                        }
                        else
                        {
                            log.ErrorLog($"CSV dosyası oluşturulamadı : {cfgFile} - {datFile}","Dönüştürme hatası", "CSV Dönüştürücü");
                            Serilog.Log.Error($"CSV dosyası oluşturulamadı : {cfgFile} - {datFile} ");
                        }
                    }
                    else
                    {
                        log.WarningLog($"CSV dosyası daha önce dönüştürülmüş: {csvName}", "CSV Dönüştürücü");
                        Serilog.Log.Information($"CSV dosyası daha önce dönüştürülmüş: {csvName}");
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorLog("Veritabanına bağlanırken hata oluştu!", ex.ToString(), "CSV Dönüştürücü");
                Serilog.Log.Error(ex, "Veritabanına bağlanırken hata oluştu!");
            }
            finally
            {
                log.InformationLog("RMS formatındaki CSV dönüştürücü kapandı.", "CSV Dönüştürücü");
                Serilog.Log.Debug("RMS formatındaki CSV dönüştürücü kapandı.. \n");
            }
        }

        public async Task ConvertDatAndCfgFilesToCsvAsInstantData(string pythonExePath, string script, string csvFilesPath)
        {
            log.InformationLog("Instantaneous formatında CSV dönüştürücü başladı.", "CSV Dönüştürücü");
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
                                        log.ErrorLog("Python hatası.", error, "Python");
                                        Serilog.Log.Error("Python hatası: {Error}", error);
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            log.ErrorLog("Hata oluştu.", ex.ToString(), "Python");
                            Serilog.Log.Error(ex, "Hata oluştu: {ErrorMessage}", ex.Message);
                        }
                        if (File.Exists(csvName))
                        {
                            item.InstantDataPath = csvName;
                            bool result = disturbanceRepository.Update(item);
                            if (!result)
                            {
                                log.ErrorLog("Instant formatındaki CSV dosyasının yolu kaydedilemedi.", "Database hatası", "CSV Dönüştürücü");
                                Serilog.Log.Error("Instant formatındaki CSV dosyasının yolu kaydedilemedi.");
                            }

                            Serilog.Log.Information($"CSV dosyası Instantaneous formatında başarılı şekilde oluşturuldu : {csvName}");
                        }
                        else
                        {
                            log.ErrorLog($"CSV dosyası oluşturulamadı : {cfgFile} - {datFile}", "Dönüştürme hatası", "CSV Dönüştürücü");
                            Serilog.Log.Error($"CSV dosyası oluşturulamadı : {cfgFile} - {datFile} ");
                        }
                    }
                    else
                    {
                        log.WarningLog($"CSV dosyası daha önce dönüştürülmüş: {csvName}", "CSV Dönüştürücü");
                        Serilog.Log.Information($"CSV dosyası daha önce dönüştürülmüş: {csvName}");
                    }

                }
            }
            catch (Exception ex)
            {
                log.ErrorLog("Veritabanına bağlanırken hata oluştu!", ex.ToString(), "CSV Dönüştürücü");
                Serilog.Log.Error(ex, "Veritabanına bağlanırken hata oluştu!");
            }
            finally
            {
                log.InformationLog("Instant formatındaki CSV dönüştürücü kapandı.", "CSV Dönüştürücü");
                Serilog.Log.Debug("Instant formatındaki CSV dönüştürücü kapandı.. \n");
            }
        }


    }
}
