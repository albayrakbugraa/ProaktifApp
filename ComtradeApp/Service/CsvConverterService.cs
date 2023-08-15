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
        public CsvConverterService(DisturbanceRepository disturbanceRepository, HistoryOfChangeRepository historyOfChangeRepository)
        {
            this.disturbanceRepository = disturbanceRepository;
            this.historyOfChangeRepository = historyOfChangeRepository;
        }

        public async Task ConvertDatAndCfgFilesToCsvAsRMSDataAsync(string pythonExePath, string script, string csvFilesPath)
        {
            Serilog.Log.Information("RMS formatında CSV dönüştürücü başladı..");
            try
            {
                var disturbances = await disturbanceRepository.GetAll();
                foreach (var item in disturbances)
                {
                    var historyOfChange = await historyOfChangeRepository.GetByMyDataId(item.ID);
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
                                        Serilog.Log.Error("Python hatası: {Error}", error);
                                    }
                                }

                            }

                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Error(ex, "Hata oluştu: {ErrorMessage}", ex.Message);
                        }
                        if (File.Exists(csvName))
                        {
                            item.RmsDataPath = csvName;
                            bool result = disturbanceRepository.Update(item);
                            if (!result) Serilog.Log.Error("RMS formatındaki CSV dosyasının yolu kaydedilemedi.");
                            Serilog.Log.Information($"CSV dosyası RMS formatında başarılı şekilde oluşturuldu : {csvName}");
                        }
                        else
                        {
                            Serilog.Log.Error($"CSV dosyası oluşturulamadı : {cfgFile} - {datFile} ");
                        }
                    }
                    else
                    {
                        Serilog.Log.Information($"CSV dosyası daha önce dönüştürülmüş: {csvName}");
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Veritabanına bağlanırken hata oluştu!");
            }
            finally
            {
                Serilog.Log.Debug("CSV dönüştürücü kapandı.. \n");
            }
        }

        public async Task ConvertDatAndCfgFilesToCsvAsInstantData(string pythonExePath, string script, string csvFilesPath)
        {
            Serilog.Log.Information("Instantaneous formatında CSV dönüştürücü başladı..");
            try
            {
                var disturbances = await disturbanceRepository.GetAll();
                foreach (var item in disturbances)
                {
                    var historyOfChange = await historyOfChangeRepository.GetByMyDataId(item.ID);
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
                                        Serilog.Log.Error("Python hatası: {Error}", error);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            Serilog.Log.Error(ex, "Hata oluştu: {ErrorMessage}", ex.Message);
                        }
                        if (File.Exists(csvName))
                        {
                            item.InstantDataPath = csvName;
                            bool result = disturbanceRepository.Update(item);
                            if (!result) Serilog.Log.Error("Instant formatındaki CSV dosyasının yolu kaydedilemedi.");
                            Serilog.Log.Information($"CSV dosyası Instantaneous formatında başarılı şekilde oluşturuldu : {csvName}");
                        }
                        else
                        {
                            Serilog.Log.Error($"CSV dosyası oluşturulamadı : {cfgFile} - {datFile} ");
                        }
                    }
                    else
                    {
                        Serilog.Log.Information($"CSV dosyası daha önce dönüştürülmüş: {csvName}");
                    }

                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Veritabanına bağlanırken hata oluştu!");
            }
            finally
            {
                Serilog.Log.Debug("CSV dönüştürücü kapandı.. \n");
            }
        }


    }
}
