using ComtradeApp.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComtradeApp
{
    public class MovingFilesService
    {
        private readonly MyDataRepository myDataRepository;
        private readonly DisturbanceRepository disturbanceRepository;

        public MovingFilesService(MyDataRepository myDataRepository, DisturbanceRepository disturbanceRepository)
        {
            this.myDataRepository = myDataRepository;
            this.disturbanceRepository = disturbanceRepository;
        }

        /// <summary>
        /// Move downloaded cfg and dat files to specified folder
        /// </summary>
        /// <param name="localFolder">where to save the downloaded cfg and dat files in local</param> 
        public async Task MovingDownloadedFiles(string localFolder,string roleFolder)
        {
            Serilog.Log.Information("Dosya taşıma işlemleri başladı..");
            try
            {
                // Go to the folder with the downloaded files
                DirectoryInfo directoryInfo = new DirectoryInfo(localFolder);

                // Get all the files in the folder
                FileInfo[] files = directoryInfo.GetFiles();
                if (files != null && files.Length > 0)
                {                    
                    // For each file, move it to the new location if it has the extension ".cfg" or ".dat"
                    foreach (FileInfo file in files)
                    {
                        if (file.Extension.ToLower() == ".cfg")
                        {
                            var disturbance = await disturbanceRepository.GetWhere(x => x.CfgFilePath.Contains(file.Name),x => x.MyData);
                            string tmNoFolderName = disturbance.MyData.AvcilarTM;
                            string hucreNoFolderName = disturbance.MyData.HucreNo;
                            string destFolderPath = Path.Combine(roleFolder, tmNoFolderName, hucreNoFolderName);
                            if (!Directory.Exists(destFolderPath))
                            {
                                Directory.CreateDirectory(destFolderPath);
                            }
                            string destPath = Path.Combine(destFolderPath, file.Name);
                            if (!File.Exists(destPath))
                            {
                                File.Move(file.FullName, destPath);
                                Serilog.Log.Information($"\"{file.Name}\" dosyası \"{destFolderPath}\" klasörüne taşındı.");
                            }
                            else
                            {
                                Serilog.Log.Information($"\"{file.Name}\" dosyası \"{destFolderPath}\" klasöründe zaten mevcut.");
                            }
                        }
                        else if (file.Extension.ToLower() == ".dat")
                        {
                            var disturbance = await disturbanceRepository.GetWhere(x => x.DatFilePath.Contains(file.Name), x => x.MyData);
                            string tmNoFolderName = disturbance.MyData.AvcilarTM;
                            string hucreNoFolderName = disturbance.MyData.HucreNo;
                            string destFolderPath = Path.Combine(roleFolder, tmNoFolderName, hucreNoFolderName);
                            if (!Directory.Exists(destFolderPath))
                            {
                                Directory.CreateDirectory(destFolderPath);
                            }
                            string destPath = Path.Combine(destFolderPath, file.Name);
                            if (!File.Exists(destPath))
                            {
                                File.Move(file.FullName, destPath);
                                Serilog.Log.Information($"\"{file.Name}\" dosyası \"{destFolderPath}\" klasörüne taşındı.");
                            }
                            else
                            {
                                Serilog.Log.Information($"\"{file.Name}\" dosyası \"{destFolderPath}\" klasöründe zaten mevcut.");
                            }
                        }
                    }
                }
                else
                {
                    Serilog.Log.Information("Taşınması gereken dosya bulunmamaktadır.");
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Information("Dosya taşıma sırasında hata alındı.");
                Serilog.Log.Error(ex.ToString());
            }
            finally
            {
                Serilog.Log.Information("Dosya taşıma işlemleri sonlandı.. \n");
            }
        }
    }
}
