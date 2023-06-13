using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComtradeApp
{
    public class CsvConverterService
    {
        /// <summary>
        /// Method to convert dat and cfg files to csv with python
        /// </summary>
        /// <param name="script">Python script local path</param> 
        /// <param name="localRmsCsvFolderPath">Csv local path</param> 
        /// <param name="pythonExePath">Python.exe local path</param> 
        //public static void ConvertDatAndCfgFilesToCsvAsRMSData(string pythonExePath, string script, string localRmsCsvFolderPath)
        //{
        //    Serilog.Log.Information("RMS formatında CSV dönüştürücü başladı..");
        //    //we check the files in the folders if they have been downloaded before
        //    string[] fileNamesLocal = Directory.GetFiles(localRmsCsvFolderPath);

        //    //add downloads to a list
        //    List<string> processedFiles = new List<string>(fileNamesLocal.Select(Path.GetFileName));
        //    List<string> cfgFiles = Directory.GetFiles(localCfgFolderPath).ToList();
        //    List<string> datFiles = Directory.GetFiles(localDatFolderPath).ToList();

        //    try
        //    {
        //        for (int i = 0; i < Math.Min(cfgFiles.Count, datFiles.Count); i++)
        //        {
        //            var cfgFile = cfgFiles[i];
        //            var datFile = datFiles[i];
        //            string cfg = Path.GetFileNameWithoutExtension(cfgFile)+"(RMS).csv";
        //            string dat = Path.GetFileNameWithoutExtension(datFile)+"(RMS).csv";
        //            if (processedFiles.Contains(cfg) || processedFiles.Contains(dat))
        //            {
        //                Serilog.Log.Information($"\"{cfg.Split("(RMS)").First()}.cfg\" - \"{dat.Split("(RMS)").First()}.dat\" daha önce dönüştürüldü !");
        //                continue;
        //            }
        //            var psi = new ProcessStartInfo();
        //            psi.FileName = pythonExePath;
        //            string fileNameDat = Path.GetFileNameWithoutExtension(cfgFile);
        //            string fileNameCfg = Path.GetFileNameWithoutExtension(datFile);
        //            //dat and cfg come with the same name, so it doesn't matter which one we name the csv file
        //            string filePathCsv = localRmsCsvFolderPath + fileNameDat + "(RMS)" + ".csv";
        //            psi.Arguments = $"\"{script}\" \"{cfgFile}\" \"{datFile}\" \"{filePathCsv}\"";
        //            psi.UseShellExecute = false;
        //            psi.CreateNoWindow = true;
        //            psi.RedirectStandardOutput = true;
        //            psi.RedirectStandardError = true;

        //            using (var process = Process.Start(psi))
        //            {
        //                using (var reader = process.StandardError)
        //                {
        //                    string error = reader.ReadToEnd();
        //                    if (!string.IsNullOrEmpty(error))
        //                        Serilog.Log.Error("Python hatası: {Error}", error);
        //                }
        //            }
        //            if (File.Exists(filePathCsv))
        //            {
        //                Serilog.Log.Information($"CSV dosyası RMS formatında başarılı şekilde oluşturuldu : {filePathCsv.Split('\\').Last()}");
        //            }
        //            else
        //            {
        //                Serilog.Log.Error($"CSV dosyası oluşturulamadı : {fileNameDat} - {fileNameCfg} ");
        //            }
        //            processedFiles.Add(cfgFile);
        //            processedFiles.Add(datFile);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Serilog.Log.Error(ex, "Hata oluştu: {ErrorMessage}", ex.Message);
        //    }
        //    finally
        //    {
        //        Serilog.Log.Debug("CSV dönüştürücü kapandı.. \n");
        //    }
        //}

        //public static void ConvertDatAndCfgFilesToCsvAsInstantData(string pythonExePath, string script, string localCfgFolderPath, string localDatFolderPath, string localInstantCsvFolderPath)
        //{
        //    Serilog.Log.Information("INSTANT formatında CSV dönüştürücü başladı..");
        //    //we check the files in the folders if they have been downloaded before
        //    string[] fileNamesLocal = Directory.GetFiles(localInstantCsvFolderPath);

        //    //add downloads to a list
        //    List<string> processedFiles = new List<string>(fileNamesLocal.Select(Path.GetFileName));
        //    List<string> cfgFiles = Directory.GetFiles(localCfgFolderPath).ToList();
        //    List<string> datFiles = Directory.GetFiles(localDatFolderPath).ToList();

        //    try
        //    {
        //        for (int i = 0; i < Math.Min(cfgFiles.Count, datFiles.Count); i++)
        //        {
        //            var cfgFile = cfgFiles[i];
        //            var datFile = datFiles[i];
        //            string cfg = Path.GetFileNameWithoutExtension(cfgFile) + "(INSTANT).csv";
        //            string dat = Path.GetFileNameWithoutExtension(datFile) + "(INSTANT).csv";
        //            if (processedFiles.Contains(cfg) || processedFiles.Contains(dat))
        //            {
        //                Serilog.Log.Information($"\"{cfg.Split("(INSTANT)").First()}.cfg\" - \"{dat.Split("(INSTANT)").First()}.dat\" daha önce dönüştürüldü !");
        //                continue;
        //            }
        //            var psi = new ProcessStartInfo();
        //            psi.FileName = pythonExePath;
        //            string fileNameDat = Path.GetFileNameWithoutExtension(cfgFile);
        //            string fileNameCfg = Path.GetFileNameWithoutExtension(datFile);
        //            //dat and cfg come with the same name, so it doesn't matter which one we name the csv file
        //            string filePathCsv = localInstantCsvFolderPath + fileNameDat + "(INSTANT)" + ".csv";
        //            psi.Arguments = $"\"{script}\" \"{cfgFile}\" \"{datFile}\" \"{filePathCsv}\"";
        //            psi.UseShellExecute = false;
        //            psi.CreateNoWindow = true;
        //            psi.RedirectStandardOutput = true;
        //            psi.RedirectStandardError = true;

        //            using (var process = Process.Start(psi))
        //            {
        //                using (var reader = process.StandardError)
        //                {
        //                    string error = reader.ReadToEnd();
        //                    if (!string.IsNullOrEmpty(error))
        //                        Serilog.Log.Error("Python hatası: {Error}", error);
        //                }
        //            }
        //            if (File.Exists(filePathCsv))
        //            {
        //                Serilog.Log.Information($"CSV dosyası INSTANT formatında başarılı şekilde oluşturuldu : {filePathCsv.Split('\\').Last()}");
        //            }
        //            else
        //            {
        //                Serilog.Log.Error($"CSV dosyası oluşturulamadı : {fileNameDat} - {fileNameCfg} ");
        //            }
        //            processedFiles.Add(cfgFile);
        //            processedFiles.Add(datFile);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Serilog.Log.Error(ex, "Hata oluştu: {ErrorMessage}", ex.Message);
        //    }
        //    finally
        //    {
        //        Serilog.Log.Debug("CSV dönüştürücü kapandı.. \n");
        //    }
        //}

    }
}
