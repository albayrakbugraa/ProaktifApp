using Azure;
using ComtradeApp.Models;
using ComtradeApp.Repository;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ComtradeApp
{
    public class FtpService
    {
        private readonly MyDataRepository myDataRepository;
        private readonly DisturbanceRepository disturbanceRepository;
        public FtpService(MyDataRepository myDataRepository, DisturbanceRepository disturbanceRepository)
        {
            this.myDataRepository = myDataRepository;
            this.disturbanceRepository = disturbanceRepository;
        }
        
        /// <summary>
        /// It takes the path of the local cfg and dat folders and checks if it has been downloaded before.
        /// </summary>
        /// <param name="roleFolder">Path to save downloaded files</param>
        public async Task DownloadCfgAndDatFilesEfCoreAsync(string roleFolder)
        {
            try
            {
                Serilog.Log.Information("FTP işlemleri başladı..");
                //var myDatas = await myDataRepository.GetAll();
                var data = await myDataRepository.GetWhere(x => x.ID == 5277);
                List<MyData> myDatas = new List<MyData>();
                myDatas.Add(data);
                foreach (var item in myDatas)
                {
                    Serilog.Log.Information($"Bağlantı Kurulan Röle Bilgileri \r Tm_kV_Hücre : {item.TmKvHucre} \r IP : {item.IP} \r Röle Model : {item.RoleModel} \n");
                    string ip = item.IP;
                    int port = item.Port;
                    string username = item.User;
                    string password = item.Password;
                    string remoteFolder = item.Path;
                    string host = $"ftp://{ip}:{port}/";
                    //we check the files in the folders if they have been downloaded before
                    string tmNoFolderName = item.AvcilarTM;
                    string hucreNoFolderName = item.HucreNo;
                    string destFolderPath = Path.Combine(roleFolder, tmNoFolderName, hucreNoFolderName);
                    if (!Directory.Exists(destFolderPath))
                    {
                        Directory.CreateDirectory(destFolderPath);
                    }                   
                    string[] fileNamesLocal = Directory.GetFiles(destFolderPath);

                    //add downloads to a list
                    List<string> downloadedFiles = new List<string>(fileNamesLocal.Select(Path.GetFileName));
                    Serilog.Log.Information($"Daha önce indirilen dosya sayısı : {downloadedFiles.Count}");

                    try
                    {
                        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(host + remoteFolder);
                        request.Method = WebRequestMethods.Ftp.ListDirectory;
                        request.UsePassive = true;
                        request.Credentials = new NetworkCredential(username, password);
                        FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                        Stream responseStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(responseStream);

                        // list the files to download 
                        List<string> filesToDownload = new List<string>();
                        string fileName = reader.ReadLine();
                        while (!string.IsNullOrEmpty(fileName))
                        {
                            if (!downloadedFiles.Contains(fileName))
                            {
                                filesToDownload.Add(fileName);
                            }
                            fileName = reader.ReadLine();
                        }
                        reader.Close();
                        response.Close();
                        Serilog.Log.Information($"İndirelecek dosya sayısı : {filesToDownload.Count}");

                        if (filesToDownload.Count > 0)
                        {
                            List<string> downloadedCfgFiles = new List<string>();
                            List<string> downloadedDatFiles = new List<string>();

                            foreach (string file in filesToDownload)
                            {
                                try
                                {
                                    WebClient client = new WebClient();
                                    client.Credentials = new NetworkCredential(username, password);
                                    string remoteFilePath = host + remoteFolder + file;
                                    string localFolderPath = destFolderPath + "\\" + file;
                                    if (!string.IsNullOrEmpty(file))
                                    {
                                        client.DownloadFile(remoteFilePath, localFolderPath);
                                        Serilog.Log.Information($"İndirilen dosya : {file}");
                                        if (file.ToLower().EndsWith(".cfg"))
                                        {
                                            downloadedCfgFiles.Add(file);
                                        }
                                        else if (file.ToLower().EndsWith(".dat"))
                                        {
                                            downloadedDatFiles.Add(file);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Serilog.Log.Error(ex, $" Röle bilgisi aşağıdaki gibi olan röleden {file} isimli dosyayı indirirken hata oluştu! \r Tm_kV_Hücre : {item.TmKvHucre} \r IP : {item.IP} \r Röle Model : {item.RoleModel} \r Dosya Adı: {file} \r ");
                                }
                            }
                            string[] localCfgFiles = Directory.GetFiles(destFolderPath, "*.cfg");
                            string[] localDatFiles = Directory.GetFiles(destFolderPath, "*.dat");
                            List<string> missingFiles = new List<string>();

                            // Kontrol edilmemiş .cfg dosyalarını bul
                            foreach (string cfgFile in localCfgFiles)
                            {
                                string cfgFileName = Path.GetFileName(cfgFile);
                                string correspondingDatFileName = $"{Path.GetFileNameWithoutExtension(cfgFile)}.dat";

                                if (!localDatFiles.Contains(Path.Combine(destFolderPath, correspondingDatFileName)))
                                {
                                    missingFiles.Add(correspondingDatFileName);
                                }
                            }

                            // Kontrol edilmemiş .dat dosyalarını bul
                            foreach (string datFile in localDatFiles)
                            {
                                string datFileName = Path.GetFileName(datFile);
                                string correspondingCfgFileName = $"{Path.GetFileNameWithoutExtension(datFile)}.cfg";

                                if (!localCfgFiles.Contains(Path.Combine(destFolderPath, correspondingCfgFileName)))
                                {
                                    missingFiles.Add(correspondingCfgFileName);
                                }
                            }

                            // Eksik dosyaları kullanıcıya bildir
                            if (missingFiles.Count > 0)
                            {
                                Serilog.Log.Warning("CFG'si veya DAT dosyası bulunup karşılığı eksik olan dosyalar :");
                                foreach (string missingFile in missingFiles)
                                {
                                    Serilog.Log.Warning(missingFile);
                                }
                                Serilog.Log.Warning("Eksik dosyaların arıza kaydı oluşturulmamıştır!");
                            }

                            // Eşleşen dosyaların üzerinden geçerek disturbance oluştur
                            foreach (string cfgFile in localCfgFiles)
                            {
                                string cfgFileName = Path.GetFileName(cfgFile);
                                string correspondingDatFileName = $"{Path.GetFileNameWithoutExtension(cfgFile)}.dat";

                                if (localDatFiles.Contains(Path.Combine(destFolderPath, correspondingDatFileName)))
                                {
                                    Disturbance disturbance = new Disturbance();
                                    disturbance.IP = item.IP;
                                    disturbance.AvcilarTM = item.AvcilarTM;
                                    disturbance.CfgFilePath = cfgFile;
                                    disturbance.FaultTime = GetFileCreationTime(Path.Combine(host, remoteFolder, cfgFileName), username, password);
                                    disturbance.HucreNo = item.HucreNo;
                                    disturbance.FiderName = item.FiderName;
                                    disturbance.RoleModel = item.RoleModel;
                                    disturbance.DatFilePath = Path.Combine(destFolderPath, correspondingDatFileName);
                                    disturbance.TmKvHucre = item.TmKvHucre;
                                    disturbance.kV = item.kV;
                                    disturbance.Status = true;
                                    disturbance.MyDataId = item.ID;
                                    bool result = await disturbanceRepository.Create(disturbance);
                                    if (result) 
                                    {
                                        Serilog.Log.Information($"Arıza kaydı oluşturulmuştur. \r Tm_kV_Hücre : {item.TmKvHucre} \r IP : {item.IP} \r Röle Model : {item.RoleModel} \r Arıza Saati : {disturbance.FaultTime} \r Comtrade Dosyası : {Path.GetFileNameWithoutExtension(cfgFile)} \n ");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Error(ex, $"Aşağıda bilgileri bulunan röleye bağlanırken hata oluştu! \r Tm_kV_Hücre : {item.TmKvHucre} \r IP : {item.IP} \r Röle Model : {item.RoleModel}");
                    }
                }
                Serilog.Log.Information("FTP sunucusundan dosyaları indirme tamamlandı.");
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Veritabanına bağlanırken hata oluştu!");
            }
            finally
            {
                Serilog.Log.Information("FTP işlemleri sonlandı.. \n");
            }
        }

        static DateTime GetFileCreationTime(string remoteFilePath, string username, string password)
        {

            FtpWebRequest dateRequest = (FtpWebRequest)WebRequest.Create(remoteFilePath);
            dateRequest.Method = WebRequestMethods.Ftp.GetDateTimestamp;
            dateRequest.Credentials = new NetworkCredential(username, password);

            FtpWebResponse dateResponse = (FtpWebResponse)dateRequest.GetResponse();
            DateTime fileCreationDateTime = dateResponse.LastModified;
            dateResponse.Close();

            return fileCreationDateTime;

            //FtpWebRequest request = (FtpWebRequest)WebRequest.Create(remoteFilePath);
            //request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            //request.UsePassive = true;
            //request.Credentials = new NetworkCredential(username, password);

            //using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            //{
            //    Stream responseStream = response.GetResponseStream();
            //    StreamReader reader = new StreamReader(responseStream);
            //    string fileDetails = reader.ReadToEnd();
            //    reader.Close();
            //    response.Close();
            //    string[] lines = fileDetails.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            //    foreach (string line in lines)
            //    {
            //        string[] columns = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //        string datePart = columns[0];
            //        string timePart = columns[1];

            //        // Oluşturma tarihini ve saati ayrıştırma
            //        string dateString = $"{datePart} {timePart}";
            //        DateTime fileCreationDateTime;
            //        if (DateTime.TryParseExact(dateString, "dd/MM/yy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out fileCreationDateTime))
            //        {
            //            // Tarihi ISO 8601 formatına çevirme
            //            string isoDateString = fileCreationDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            //            // ISO 8601 formatındaki tarihi DateTime nesnesine dönüştürme
            //            if (DateTime.TryParseExact(isoDateString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out fileCreationDateTime))
            //            {
            //                return fileCreationDateTime;
            //            }
            //        }
            //    }
            //}

            //return DateTime.MinValue; // Eğer dosya ayrıntıları bulunamazsa varsayılan bir değer döndürebilirsiniz
        }



        ////const string host = "ftp://ftp.dlptest.com/";
        ////const string username = "dlpuser";
        ////const string password = "rNrKYTX9g7z3RgJRmxWuGHbeu";
        ////const string remoteFolder = "bugra-test/";
        ////const string username = "ADMINISTRATOR";
        ////const string password = "remote0004";
        ////const string remoteFolder = "COMTRADE/";
        ////const string host = "ftp://10.212.72.222/";
        ///// <summary>
        ///// It downloads dat and cfg files by taking the necessary information for ftp from the csv file.
        ///// It takes the path of the local cfg and dat folders and checks if it has been downloaded before.
        ///// </summary>
        ///// <param name="ftpCsvPath">The path to the csv file with the information required for ftp</param>
        ///// <param name="localFolder">Path to save downloaded files</param>
        ///// <param name="localCfgFolderPath">Path to local CFG folder</param>
        ///// <param name="localDatFolderPath">Path to local DAT folder</param>
        //public static void DownloadFiles(string ftpCsvPath, string localFolder, string localCfgFolderPath, string localDatFolderPath)
        //{
        //    try
        //    {
        //        Serilog.Log.Information("FTP işlemleri başladı..");
        //        using (var streamReader = new StreamReader(ftpCsvPath))
        //        {
        //            var headerLine = streamReader.ReadLine();
        //            while (!streamReader.EndOfStream)
        //            {
        //                var line = streamReader.ReadLine();
        //                var values = line.Split(';');
        //                string ip = values[0];
        //                string port = values[1];
        //                string username = values[2];
        //                string password = values[3];
        //                string remoteFolder = values[4];
        //                string host = $"ftp://{ip}:{port}";
        //                //we check the files in the folders if they have been downloaded before
        //                string[] fileNamesLocal = Directory.GetFiles(localFolder);
        //                string[] fileNamesCfg = Directory.GetFiles(localCfgFolderPath);
        //                string[] fileNamesDat = Directory.GetFiles(localDatFolderPath);
        //                string[] allFileNames = fileNamesLocal.Concat(fileNamesCfg).Concat(fileNamesDat).ToArray();

        //                //add downloads to a list
        //                List<string> downloadedFiles = new List<string>(allFileNames.Select(Path.GetFileName));
        //                Serilog.Log.Information($"Daha önce indirilen dosya sayısı : {downloadedFiles.Count}");
        //                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(host + remoteFolder);
        //                request.Method = WebRequestMethods.Ftp.ListDirectory;
        //                request.UsePassive = true;
        //                request.Credentials = new NetworkCredential(username, password);
        //                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        //                Serilog.Log.Information(response.StatusCode.ToString());
        //                Stream responseStream = response.GetResponseStream();
        //                StreamReader reader = new StreamReader(responseStream);

        //                // list the files to download 
        //                List<string> filesToDownload = new List<string>();
        //                string fileName = reader.ReadLine();
        //                while (!string.IsNullOrEmpty(fileName))
        //                {
        //                    if (!downloadedFiles.Contains(fileName))
        //                    {
        //                        filesToDownload.Add(fileName);
        //                    }
        //                    fileName = reader.ReadLine();
        //                }
        //                reader.Close();
        //                response.Close();
        //                Serilog.Log.Information($"İndirelecek dosya sayısı : {filesToDownload.Count}");

        //                if (filesToDownload.Count > 0)
        //                {
        //                    foreach (string file in filesToDownload)
        //                    {
        //                        WebClient client = new WebClient();
        //                        client.Credentials = new NetworkCredential(username, password);
        //                        string remoteFilePath = host + remoteFolder + file;
        //                        string localFolderPath = localFolder + @"\" + file;
        //                        if (!string.IsNullOrEmpty(file))
        //                        {
        //                            client.DownloadFile(remoteFilePath, localFolderPath);
        //                            Serilog.Log.Information($"İndirilen dosya : {file}");
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        Serilog.Log.Information("FTP sunucusundan dosyaları indirme tamamlandı.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Serilog.Log.Error(ex, "Dosyaları indirirken hata oluştu.");
        //    }
        //    finally
        //    {
        //        Serilog.Log.Information("FTP işlemleri sonlandı.. \n");
        //    }

        //}
        ///// <summary>
        ///// It downloads dat and cfg files by taking the necessary information for ftp from the csv file.
        ///// It takes the path of the local cfg and dat folders and checks if it has been downloaded before.
        ///// </summary>
        ///// <param name="ftpCsvPath">The path to the csv file with the information required for ftp</param>
        ///// <param name="localFolder">Path to save downloaded files</param>
        ///// <param name="localCfgFolderPath">Path to local CFG folder</param>
        ///// <param name="localDatFolderPath">Path to local DAT folder</param>
        //public static void DownloadCfgAndDatFiles(string ftpCsvPath, string localFolder, string localCfgFolderPath, string localDatFolderPath)
        //{
        //    try
        //    {
        //        Serilog.Log.Information("FTP işlemleri başladı..");
        //        using (var streamReader = new StreamReader(ftpCsvPath))
        //        {
        //            var lines = File.ReadAllLines(ftpCsvPath);
        //            for (int i = 1; i < lines.Length; i++) // İlk satırı atla
        //            {
        //                var line = lines[i];
        //                if (string.IsNullOrEmpty(line)) break;
        //                var values = line.Split(',');
        //                string ip = values[0];
        //                string port = values[1];
        //                string username = values[2];
        //                string password = values[3];
        //                string remoteFolder = values[4];
        //                string host = $"ftp://{ip}:{port}/";
        //                //we check the files in the folders if they have been downloaded before
        //                string[] fileNamesLocal = Directory.GetFiles(localFolder);
        //                string[] fileNamesCfg = Directory.GetFiles(localCfgFolderPath);
        //                string[] fileNamesDat = Directory.GetFiles(localDatFolderPath);
        //                string[] allFileNames = fileNamesLocal.Concat(fileNamesCfg).Concat(fileNamesDat).ToArray();

        //                //add downloads to a list
        //                List<string> downloadedFiles = new List<string>(allFileNames.Select(Path.GetFileName));
        //                Serilog.Log.Information($"Daha önce indirilen dosya sayısı : {downloadedFiles.Count}");
        //                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(host + remoteFolder);
        //                request.Method = WebRequestMethods.Ftp.ListDirectory;
        //                request.UsePassive = true;
        //                request.Credentials = new NetworkCredential(username, password);
        //                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        //                Serilog.Log.Information(response.StatusCode.ToString());
        //                Stream responseStream = response.GetResponseStream();
        //                StreamReader reader = new StreamReader(responseStream);

        //                // list the files to download 
        //                List<string> filesToDownload = new List<string>();
        //                string fileName = reader.ReadLine();
        //                while (!string.IsNullOrEmpty(fileName))
        //                {
        //                    if (!downloadedFiles.Contains(fileName))
        //                    {
        //                        filesToDownload.Add(fileName);
        //                    }
        //                    fileName = reader.ReadLine();
        //                }
        //                reader.Close();
        //                response.Close();
        //                Serilog.Log.Information($"İndirelecek dosya sayısı : {filesToDownload.Count}");

        //                if (filesToDownload.Count > 0)
        //                {
        //                    foreach (string file in filesToDownload)
        //                    {
        //                        WebClient client = new WebClient();
        //                        client.Credentials = new NetworkCredential(username, password);
        //                        string remoteFilePath = host + remoteFolder + file;
        //                        string localFolderPath = localFolder + @"\" + "(" + DateTime.Now.ToString("yyyy.MM.ddTHH.mm.ss") + ")" + "-" + file;
        //                        if (!string.IsNullOrEmpty(file))
        //                        {
        //                            client.DownloadFile(remoteFilePath, localFolderPath);
        //                            Serilog.Log.Information($"İndirilen dosya : {file}");
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        Serilog.Log.Information("FTP sunucusundan dosyaları indirme tamamlandı.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Serilog.Log.Error(ex, "Dosyaları indirirken hata oluştu.");
        //    }
        //    finally
        //    {
        //        Serilog.Log.Information("FTP işlemleri sonlandı.. \n");
        //    }
        //}
    }

}
