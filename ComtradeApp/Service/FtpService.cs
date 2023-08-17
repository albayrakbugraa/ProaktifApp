using ComtradeApp.Models;
using ComtradeApp.Repository;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using WinSCP;
using static System.Collections.Specialized.BitVector32;

namespace ComtradeApp.Service
{
    public class FtpService
    {
        private readonly RelayInformationRepository relayInformationRepository;
        private readonly DisturbanceRepository disturbanceRepository;
        private readonly HistoryOfChangeRepository historyOfChangeRepository;
        private readonly LogRepository log;
        public FtpService(RelayInformationRepository relayInformationRepository, DisturbanceRepository disturbanceRepository, HistoryOfChangeRepository historyOfChangeRepository, LogRepository log)
        {
            this.relayInformationRepository = relayInformationRepository;
            this.disturbanceRepository = disturbanceRepository;
            this.historyOfChangeRepository = historyOfChangeRepository;
            this.log = log;
        }
        public async Task DownloadCfgAndDatFilesEfCoreAsync(string comtradeFilesPath)
        {
            try
            {
                await log.InformationLog("FTP işlemleri başladı.", "FTP");
                Serilog.Log.Information("FTP işlemleri başladı..");

                //var data = await relayInformationRepository.GetWhere(x => x.ID == 1);
                //List<RelayInformation> relayInformations = new List<RelayInformation>();
                //relayInformations.Add(data);
                var relayInformations = await relayInformationRepository.GetAll();

                foreach (var item in relayInformations)
                {
                    await log.InformationLog($"Bağlantı Kurulan Röle Bilgileri \r Tm_kV_Hücre : {item.TmKvHucre} \r IP : {item.IP} \r Röle Model : {item.RoleModel} \n", "FTP");
                    Serilog.Log.Information($"Bağlantı Kurulan Röle Bilgileri \r Tm_kV_Hücre : {item.TmKvHucre} \r IP : {item.IP} \r Röle Model : {item.RoleModel} \n");
                    var historyOfChange = await historyOfChangeRepository.GetByRelayInformationId(item.ID);
                    if (historyOfChange != null && item.IP == historyOfChange.NewIP)
                    {
                        await historyOfChangeRepository.UpdateFolderNames(item, comtradeFilesPath);
                    }
                    string host = $"ftp://{item.IP}:{item.Port}/";
                    string tmNoFolderName = item.TmNo;
                    string hucreNoFolderName = item.HucreNo;
                    string destFolderPath = Path.Combine(comtradeFilesPath, $"{item.IP}-{item.TmKvHucre}");

                    if (!Directory.Exists(destFolderPath))
                    {
                        Directory.CreateDirectory(destFolderPath);
                    }

                    ParametersModel parameters = new ParametersModel();
                    string[] fileNamesLocal = Directory.GetFiles(destFolderPath);
                    parameters.DownloadedFiles = new List<string>(fileNamesLocal.Select(Path.GetFileName));
                    parameters.RelayInformation = item;
                    parameters.Host = host;
                    parameters.ComtradeFilesPath = comtradeFilesPath;
                    parameters.DestFolderPath = destFolderPath;
                    if (item.User == "supervisor")
                    {
                        parameters = await DownloadWithWinScp(parameters);
                        if (parameters.FilesToDownload.Count > 0 && parameters.FilesToDownload != null)
                        {
                            await CheckMissingFiles(parameters);
                            await CreateDisturbancesAsync(parameters);
                        }
                    }
                    else
                    {
                        parameters.FilesToDownload = await GetFilesToDownloadFromFtp(parameters);
                        Serilog.Log.Information($"İndirelecek dosya sayısı : {parameters.FilesToDownload.Count}");

                        if (parameters.FilesToDownload.Count > 0)
                        {
                            List<string>[] downloadedFilesArray = await DownloadFilesFromFtp(parameters);
                            parameters.DownloadedCfgFiles = downloadedFilesArray[0];
                            parameters.DownloadedDatFiles = downloadedFilesArray[1];
                            await CheckMissingFiles(parameters);
                            await CreateDisturbancesAsync(parameters);
                        }
                    }
                }
                await log.InformationLog("FTP sunucusundan dosyaları indirme tamamlandı.", "FTP");
                Serilog.Log.Information("FTP sunucusundan dosyaları indirme tamamlandı.");
            }
            catch (Exception ex)
            {
                await log.ErrorLog("Veritabanına bağlanırken hata oluştu!", ex.ToString(),"FTP");
                Serilog.Log.Error(ex, "Veritabanına bağlanırken hata oluştu!");
            }
            finally
            {
                await log.InformationLog("FTP işlemleri sonlandı.", "FTP");
                Serilog.Log.Information("FTP işlemleri sonlandı.. \n");
            }
        }

        private async Task<ParametersModel> DownloadWithWinScp(ParametersModel parameters)
        {
            try
            {
                // WinSCP oturum nesnesini oluşturun
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Ftp,
                    HostName = parameters.RelayInformation.IP,
                    UserName = parameters.RelayInformation.User,
                    Password = parameters.RelayInformation.Password,
                };
                using (Session session = new Session())
                {
                    parameters.FilesToDownload = new List<string>();
                    parameters.DownloadedCfgFiles = new List<string>();
                    parameters.DownloadedDatFiles = new List<string>();

                    // FTP sunucusuna bağlan
                    session.Open(sessionOptions);

                    // Dosyaların indirileceği yerel klasör
                    string localFolderPath = parameters.DestFolderPath + "\\";

                    // İndirme işlemi için oturumun transfer opsiyonlarını 
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;

                    // FTP sunucusundaki klasörün adı
                    string remoteFolderPath = parameters.RelayInformation.Path;

                    // Klasördeki tüm dosyaların listesini al
                    RemoteDirectoryInfo directoryInfo = session.ListDirectory(remoteFolderPath);
                    foreach (RemoteFileInfo fileInfo in directoryInfo.Files)
                    {
                        // Eğer dosya, bir dosya (Directory olmayan) ise, dosyayı indir
                        if (!fileInfo.IsDirectory && !parameters.DownloadedFiles.Contains(fileInfo.Name))
                        {
                            if (!fileInfo.IsDirectory) parameters.FilesToDownload.Add(fileInfo.Name);

                            string remoteFilePath = remoteFolderPath + fileInfo.Name;
                            string localFilePath = localFolderPath + fileInfo.Name;
                            TransferOperationResult transferResult = session.GetFiles(remoteFilePath, localFilePath, false, transferOptions);

                            // Transfer işleminin başarılı olup olmadığını kontrol et
                            if (transferResult.IsSuccess)
                            {
                                Serilog.Log.Information($"İndirilen dosya : {fileInfo.Name}");
                                if (fileInfo.Name.ToLower().EndsWith(".cfg"))
                                {
                                    parameters.DownloadedCfgFiles.Add(fileInfo.Name);
                                }
                                else if (fileInfo.Name.ToLower().EndsWith(".dat"))
                                {
                                    parameters.DownloadedDatFiles.Add(fileInfo.Name);
                                }

                            }
                            else
                            {
                                await log.ErrorLog($" Röle bilgisi aşağıdaki gibi olan röleden {fileInfo.Name} isimli dosyayı indirirken hata oluştu! \r Tm_kV_Hücre : {parameters.RelayInformation.TmKvHucre} \r IP : {parameters.RelayInformation.IP} \r Röle Model : {parameters.RelayInformation.RoleModel} \r Dosya Adı: {fileInfo.Name} \r ", "Dosyayı indirirken hata oluştu!", "FTP");

                                Serilog.Log.Error($" Röle bilgisi aşağıdaki gibi olan röleden {fileInfo.Name} isimli dosyayı indirirken hata oluştu! \r Tm_kV_Hücre : {parameters.RelayInformation.TmKvHucre} \r IP : {parameters.RelayInformation.IP} \r Röle Model : {parameters.RelayInformation.RoleModel} \r Dosya Adı: {fileInfo.Name} \r ");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await log.ErrorLog("Hata", ex.ToString(), "FTP");
                Serilog.Log.Error("Hata: " + ex.Message);
            }
            return parameters;
        }

        private async Task<List<string>> GetFilesToDownloadFromFtp(ParametersModel parameters)
        {
            List<string> filesToDownload = new List<string>();

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(parameters.Host + parameters.RelayInformation.Path);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.UsePassive = true;
                request.Credentials = new NetworkCredential(parameters.RelayInformation.User, parameters.RelayInformation.Password);

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);

                string fileName = reader.ReadLine();
                while (!string.IsNullOrEmpty(fileName))
                {
                    //string[] itemParts = fileName.Split(' ');
                    //string fileName2 = itemParts[itemParts.Length - 1];
                    if (!parameters.DownloadedFiles.Contains(fileName))
                    {
                        filesToDownload.Add(fileName);
                    }
                    fileName = reader.ReadLine();
                }

                reader.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                await log.ErrorLog($"Aşağıda bilgileri bulunan röleye bağlanırken hata oluştu! \r Tm_kV_Hücre : {parameters.RelayInformation.TmKvHucre} \r IP : {parameters.RelayInformation.IP} \r Röle Model : {parameters.RelayInformation.RoleModel}", ex.ToString(), "FTP");
                Serilog.Log.Error(ex, $"Aşağıda bilgileri bulunan röleye bağlanırken hata oluştu! \r Tm_kV_Hücre : {parameters.RelayInformation.TmKvHucre} \r IP : {parameters.RelayInformation.IP} \r Röle Model : {parameters.RelayInformation.RoleModel}");
            }

            return filesToDownload;
        }

        private async Task<List<string>[]> DownloadFilesFromFtp(ParametersModel parameters)
        {
            List<string> downloadedCfgFiles = new List<string>();
            List<string> downloadedDatFiles = new List<string>();
            foreach (string file in parameters.FilesToDownload)
            {
                try
                {
                    WebClient client = new WebClient();
                    client.Credentials = new NetworkCredential(parameters.RelayInformation.User, parameters.RelayInformation.Password);
                    string remoteFilePath = parameters.Host + parameters.RelayInformation.Path + file;
                    string localFolderPath = Path.Combine(parameters.DestFolderPath, file);
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
                    await log.ErrorLog($" Röle bilgisi aşağıdaki gibi olan röleden {file} isimli dosyayı indirirken hata oluştu! \r Tm_kV_Hücre : {parameters.RelayInformation.TmKvHucre} \r IP : {parameters.RelayInformation.IP} \r Röle Model : {parameters.RelayInformation.RoleModel} \r Dosya Adı: {file} \r ", ex.ToString(),"FTP");
                    Serilog.Log.Error(ex, $" Röle bilgisi aşağıdaki gibi olan röleden {file} isimli dosyayı indirirken hata oluştu! \r Tm_kV_Hücre : {parameters.RelayInformation.TmKvHucre} \r IP : {parameters.RelayInformation.IP} \r Röle Model : {parameters.RelayInformation.RoleModel} \r Dosya Adı: {file} \r ");
                }
            }

            return new List<string>[] { downloadedCfgFiles, downloadedDatFiles };
        }

        private async Task CheckMissingFiles(ParametersModel parameters)
        {
            try
            {
                foreach (string cfgFile in parameters.DownloadedCfgFiles)
                {
                    string datFileName = Path.ChangeExtension(cfgFile, ".dat");

                    if (!parameters.DownloadedDatFiles.Contains(datFileName, StringComparer.OrdinalIgnoreCase))
                    {
                        await log.WarningLog($"{cfgFile} dosyasına karşılık gelen DAT dosyası eksik: {datFileName}", "Arıza Kaydı");
                        Serilog.Log.Warning($"{cfgFile} dosyasına karşılık gelen DAT dosyası eksik: {datFileName}");
                    }
                }
                foreach (string datFile in parameters.DownloadedDatFiles)
                {
                    string cfgFileName = Path.ChangeExtension(datFile, ".cfg");

                    if (!parameters.DownloadedCfgFiles.Contains(cfgFileName, StringComparer.OrdinalIgnoreCase))
                    {
                        await log.WarningLog($"{datFile} dosyasına karşılık gelen DAT dosyası eksik: {cfgFileName}", "Arıza Kaydı");
                        Serilog.Log.Warning($"{datFile} dosyasına karşılık gelen CFG dosyası eksik: {cfgFileName}");
                    }
                }
            }
            catch (Exception ex)
            {
                await log.ErrorLog($"Eksik dosyalar kontrol edilirken hata oluştu! \r Dizin: {parameters.DestFolderPath}", ex.ToString(),"Arıza Kaydı");
                Serilog.Log.Error(ex, $"Eksik dosyalar kontrol edilirken hata oluştu! \r Dizin: {parameters.DestFolderPath}");
            }
        }

        private async Task CreateDisturbancesAsync(ParametersModel parameters)
        {
            try
            {
                foreach (string cfgFile in parameters.DownloadedCfgFiles)
                {
                    string datFileName = Path.ChangeExtension(cfgFile, ".dat");
                    string cfgFilePath = Path.Combine(parameters.DestFolderPath, cfgFile);
                    string datFilePath = Path.Combine(parameters.DestFolderPath, datFileName);

                    if (File.Exists(Path.Combine(parameters.DestFolderPath, datFileName)))
                    {
                        string cfgFileData = File.ReadAllText(cfgFilePath);
                        byte[] datFileData = File.ReadAllBytes(datFilePath);

                        string[] lines = cfgFileData.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        DateTime faultTimeEnd = DateTime.ParseExact(lines[lines.Length - 3], "dd/MM/yyyy,HH:mm:ss.ffffff", CultureInfo.InvariantCulture, DateTimeStyles.None);
                        DateTime faultTimeStart = DateTime.ParseExact(lines[lines.Length - 4], "dd/MM/yyyy,HH:mm:ss.ffffff", CultureInfo.InvariantCulture, DateTimeStyles.None);
                        TimeSpan totalTime = faultTimeEnd - faultTimeStart;
                        double totalFaultTime = totalTime.TotalSeconds;

                        Disturbance disturbance = new Disturbance();
                        disturbance.IP = parameters.RelayInformation.IP;
                        disturbance.TmNo = parameters.RelayInformation.TmNo;
                        disturbance.CfgFilePath = cfgFilePath;
                        disturbance.FaultTimeStart = faultTimeStart;
                        disturbance.FaultTimeEnd = faultTimeEnd;   
                        disturbance.TotalFaultTime = totalFaultTime;
                        //if (parameters.RelayInformation.User == "supervisor")
                        //{
                        //    FileInfo fileInfo = new FileInfo(Path.Combine(parameters.DestFolderPath, datFileName));
                        //    disturbance.FaultTimeStart = fileInfo.LastWriteTime;
                        //}
                        //else
                        //{
                        //    disturbance.FaultTimeStart = GetFileCreationTime(parameters, cfgFile);
                        //}
                        disturbance.HucreNo = parameters.RelayInformation.HucreNo;
                        disturbance.FiderName = parameters.RelayInformation.FiderName;
                        disturbance.RoleModel = parameters.RelayInformation.RoleModel;
                        disturbance.DatFilePath = datFilePath;
                        disturbance.TmKvHucre = parameters.RelayInformation.TmKvHucre;
                        disturbance.kV = parameters.RelayInformation.kV;
                        disturbance.Status = true;
                        disturbance.RelayInformationId = parameters.RelayInformation.ID;
                        disturbance.CfgFileData = cfgFileData;
                        disturbance.DatFileData = datFileData;
                        disturbance.ComtradeName = Path.GetFileNameWithoutExtension(cfgFile);
                        disturbance.sFtpStatus = false;
                        disturbance.PutTime = DateTime.MinValue;

                        bool result = await disturbanceRepository.Create(disturbance);
                        if (result)
                        {
                            Serilog.Log.Information($"Arıza kaydı oluşturuldu. \r Tm_kV_Hücre : {parameters.RelayInformation.TmKvHucre} \r IP : {parameters.RelayInformation.IP} \r Röle Model : {parameters.RelayInformation.RoleModel} \r CFG dosyası : {cfgFile} \r DAT dosyası : {datFileName}");
                        }
                        else
                        {
                            await log.ErrorLog($"Arıza kaydı oluşturulurken hata oluştu! \r Tm_kV_Hücre : {parameters.RelayInformation.TmKvHucre} \r IP : {parameters.RelayInformation.IP} \r Röle Model : {parameters.RelayInformation.RoleModel} \r CFG dosyası : {cfgFile} \r DAT dosyası : {datFileName}", "Veritabanına kaydederken hata oluştu.", "Arıza Kaydı");
                            Serilog.Log.Error($"Arıza kaydı oluşturulurken hata oluştu! \r Tm_kV_Hücre : {parameters.RelayInformation.TmKvHucre} \r IP : {parameters.RelayInformation.IP} \r Röle Model : {parameters.RelayInformation.RoleModel} \r CFG dosyası : {cfgFile} \r DAT dosyası : {datFileName}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await log.ErrorLog($"Arıza kaydı oluşturulurken hata oluştu! \r Tm_kV_Hücre : {parameters.RelayInformation.TmKvHucre} \r IP : {parameters.RelayInformation.IP} \r Röle Model : {parameters.RelayInformation.RoleModel}", ex.ToString(), "Arıza Kaydı");
                Serilog.Log.Error(ex, $"Arıza kaydı oluşturulurken hata oluştu! \r Tm_kV_Hücre : {parameters.RelayInformation.TmKvHucre} \r IP : {parameters.RelayInformation.IP} \r Röle Model : {parameters.RelayInformation.RoleModel}");
            }
        }

        private DateTime GetFileCreationTime(ParametersModel parameters, string fileName)
        {
            try
            {
                //FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{parameters.RelayInformation.IP}:{parameters.RelayInformation.Port}/{parameters.RelayInformation.Path}/{fileName}");
                //request.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                //request.UsePassive = true;
                //request.Credentials = new NetworkCredential(parameters.RelayInformation.User, parameters.RelayInformation.Password);

                //FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                //DateTime creationTime = response.LastModified;
                //response.Close();
                //return creationTime;


                FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{parameters.RelayInformation.IP}:{parameters.RelayInformation.Port}/{parameters.RelayInformation.Path}/{fileName}");
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.UsePassive = true;
                request.Credentials = new NetworkCredential(parameters.RelayInformation.User, parameters.RelayInformation.Password);

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Stream responseStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(responseStream);
                    string fileDetails = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                    string[] lines = fileDetails.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string line in lines)
                    {
                        string[] columns = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        string datePart = columns[0];
                        string timePart = columns[1];

                        // Oluşturma tarihini ve saati ayrıştırma
                        string dateString = $"{datePart} {timePart}";
                        DateTime fileCreationDateTime;
                        if (DateTime.TryParseExact(dateString, "dd/MM/yy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out fileCreationDateTime))
                        {
                            // Tarihi ISO 8601 formatına çevirme
                            string isoDateString = fileCreationDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                            // ISO 8601 formatındaki tarihi DateTime nesnesine dönüştürme
                            if (DateTime.TryParseExact(isoDateString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out fileCreationDateTime))
                            {
                                return fileCreationDateTime;
                            }
                        }
                    }
                }

                return DateTime.MinValue; // Eğer dosya ayrıntıları bulunamazsa varsayılan bir değer döndürür
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, $"Dosya oluşturma zamanı alınırken hata oluştu! \r Dosya Adı: {fileName}");
                return DateTime.MinValue;
            }
        }

    }
}
