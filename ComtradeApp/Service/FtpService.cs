using ComtradeApp.Helper;
using ComtradeApp.Models;
using ComtradeApp.Repository;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using WinSCP;
using static System.Collections.Specialized.BitVector32;

namespace ComtradeApp.Service
{
    /// <summary>
    /// Bu sınıf, FTP sunucuları ile etkileşimde bulunmak ve Comtrade verilerini indirmek için kullanılır. Bu sınıfın ana görevi, FTP sunucuları üzerinden verileri çekmek ve bu verileri belirli bir klasöre indirmek, eksik dosyaları kontrol etmek ve ardından bu verilere dayalı olarak arıza kayıtları oluşturmaktır.
    /// </summary>
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
        /// <summary>
        /// Comtrade verilerini FTP sunucularından indirmek için kullanılır. 
        /// Her bir röle için FTP sunucusuna bağlanır. 
        /// Belirtilen klasöre, rölenin IP ve TmKvHucre bilgilerini içeren bir alt klasör oluşturur. 
        /// WinSCP veya WebClient kullanarak belirtilen dosyaları indirir.
        /// Eksik dosyaları kontrol eder.
        /// Arıza kayıtları oluşturur.
        /// </summary>
        /// <param name="comtradeFilesPath">Dosyaların indireleceği hedef klasör.</param>
        /// <param name="relayInformations">Her bir thread için ayrı ayrı gelen röle listesi.</param>
        /// <returns>Bu metot, async ve asenkron bir işlem gerçekleştirir, ancak herhangi bir değer döndürmez. Bu tür metotlar, işlemleri başlatmak, bitirmek veya asenkron olarak yürütmek için kullanılır, ancak bir sonuç döndürmezler.</returns>
        public async Task DownloadCfgAndDatFilesEfCoreAsync(string comtradeFilesPath, List<RelayInformation> relayInformations)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            try
            {
                await log.InformationLog($"FTP işlemleri başladı.", "FTP", threadId);
                Serilog.Log.Information($"FTP işlemleri Thread:{threadId} iş parçacığıyla başladı.");

                foreach (var item in relayInformations)
                {
                    await log.InformationLog($"Bağlantı Kurulan Röle Bilgileri \r Tm_kV_Hücre : {item.TmKvHucre} \r IP : {item.IP} \r Röle Model : {item.RoleModel} \n", "FTP", threadId);
                    Serilog.Log.Information($"Bağlantı Kurulan Röle Bilgileri \r Tm_kV_Hücre : {item.TmKvHucre} \r IP : {item.IP} \r Röle Model : {item.RoleModel} \n");
                    var historyOfChange = await historyOfChangeRepository.GetByRelayInformationId(item.ID);
                    if (historyOfChange != null && item.IP == historyOfChange.NewIP)
                    {
                        try
                        {
                            await historyOfChangeRepository.UpdateFolderNames(item, comtradeFilesPath);
                        }
                        catch (Exception ex)
                        {
                            await log.ErrorLog("Dosya ismi güncellenirken hata oluştu.", ex.ToString(), "FTP", threadId);
                            Serilog.Log.Error(ex, "Dosya ismi güncellenirken hata oluştu.");
                        }
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
                            await log.InformationLog($"Tm_kV_Hücre : {item.TmKvHucre}  IP : {item.IP}  Röle Model : {item.RoleModel} İndilecek dosya sayısı : {parameters.FilesToDownload.Count}", "FTP", threadId);
                            Serilog.Log.Information($"Tm_kV_Hücre : {item.TmKvHucre}  IP : {item.IP}  Röle Model : {item.RoleModel} İndilecek dosya sayısı : {parameters.FilesToDownload.Count}");
                            await CheckMissingFiles(parameters);
                            await CreateDisturbancesAsync(parameters);
                        }
                    }
                    else
                    {
                        parameters.FilesToDownload = await GetFilesToDownloadFromFtp(parameters);

                        if (parameters.FilesToDownload.Count > 0)
                        {

                            Serilog.Log.Information($"Tm_kV_Hücre : {item.TmKvHucre}  IP : {item.IP}  Röle Model : {item.RoleModel} İndilecek dosya sayısı : {parameters.FilesToDownload.Count}");
                            await log.InformationLog($"Tm_kV_Hücre : {item.TmKvHucre}  IP : {item.IP}  Röle Model : {item.RoleModel} İndilecek dosya sayısı : {parameters.FilesToDownload.Count}", "FTP", threadId);
                            List<string>[] downloadedFilesArray = await DownloadFilesFromFtp(parameters);
                            parameters.DownloadedCfgFiles = downloadedFilesArray[0];
                            parameters.DownloadedDatFiles = downloadedFilesArray[1];
                            await CheckMissingFiles(parameters);
                            await CreateDisturbancesAsync(parameters);
                        }
                    }
                }
                await log.InformationLog("FTP sunucusundan dosyaları indirme tamamlandı.", "FTP", threadId);
                Serilog.Log.Information("FTP sunucusundan dosyaları indirme tamamlandı.");
            }
            catch (Exception ex)
            {
                await log.ErrorLog("Veritabanına bağlanırken hata oluştu!", ex.ToString(), "FTP", threadId);
                Serilog.Log.Error(ex, "Veritabanına bağlanırken hata oluştu!");
            }
            finally
            {
                await log.InformationLog("FTP işlemleri sonlandı.", "FTP", threadId);
                Serilog.Log.Information("FTP işlemleri sonlandı.. \n");
            }
        }

        /// <summary>
        ///  WinSCP kullanarak FTP sunucusundan veri indirmek için kullanılan özel bir metottur. Verileri indirirken WinSCP oturumu oluşturur, dosyaları indirir ve indirme işlemi sırasında hataları kontrol eder.
        /// </summary>
        /// <param name="parameters">
        /// Kullanılan parametreleri ve verileri gruplamak ve iletmek için tasarlanmıştır.
        /// FilesToDownload: İndirilecek dosyaların listesini içerir. 
        /// DownloadedCfgFiles: İndirilen CFG dosyalarının listesini içerir.       
        /// DownloadedDatFiles: İndirilen DAT dosyalarının listesini içerir.       
        /// RelayInformation: RelayInformation adlı başka bir sınıfın özelliklerini içeren bir nesnedir.Bu özellik, röle bilgilerini temsil eder.      
        /// Host: FTP sunucusunun adresini içerir.     
        /// ComtradeFilesPath: Comtrade verilerinin kaydedildiği klasörün yolu.      
        /// DestFolderPath: Verilerin indirildiği hedef klasörün yolu.   
        /// DownloadedFiles: Zaten indirilmiş dosyaların listesini içerir.
        /// </param>
        /// <returns>ParametersModel</returns>
        private async Task<ParametersModel> DownloadWithWinScp(ParametersModel parameters)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            try
            {
                // WinSCP oturum nesnesini oluşturun
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Ftp,
                    HostName = parameters.RelayInformation.IP,
                    UserName = parameters.RelayInformation.User,
                    Password = Encryption.Decrypt(parameters.RelayInformation.Password),
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
                                await log.InformationLog($"İndirilen dosya : {fileInfo.Name}", "FTP", threadId);
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
                                await log.ErrorLog($" Röleden {fileInfo.Name} isimli dosyayı indirirken hata oluştu! \r Tm_kV_Hücre : {parameters.RelayInformation.TmKvHucre} \r IP : {parameters.RelayInformation.IP} \r Röle Model : {parameters.RelayInformation.RoleModel} \r Dosya Adı: {fileInfo.Name} \r ", "Dosyayı indirirken hata oluştu!", "FTP", threadId);

                                Serilog.Log.Error($" Röle bilgisi aşağıdaki gibi olan röleden {fileInfo.Name} isimli dosyayı indirirken hata oluştu! \r Tm_kV_Hücre : {parameters.RelayInformation.TmKvHucre} \r IP : {parameters.RelayInformation.IP} \r Röle Model : {parameters.RelayInformation.RoleModel} \r Dosya Adı: {fileInfo.Name} \r ");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await log.ErrorLog($"{parameters.RelayInformation.TmKvHucre} - {parameters.RelayInformation.IP} - {parameters.RelayInformation.RoleModel} : FTP sunucusuna bağlanırken hata oluştu.", ex.ToString(), "FTP", threadId);
                Serilog.Log.Error($"{parameters.RelayInformation.TmKvHucre} - {parameters.RelayInformation.IP} - {parameters.RelayInformation.RoleModel} : FTP sunucusuna bağlanırken hata oluştu." + ex.Message);
            }
            return parameters;
        }

        /// <summary>
        /// FtpWebRequest kullanarak FTP sunucusundan indirilecek dosyaların listesini almak için kullanılır. Bağlantı yapıp, sunucudan dosya listesini alır ve daha önce indirilmiş dosyaları hariç tutarak yeni dosyaları belirler.
        /// </summary>
        /// <param name="parameters">
        /// Kullanılan parametreleri ve verileri gruplamak ve iletmek için tasarlanmıştır.
        /// FilesToDownload: İndirilecek dosyaların listesini içerir. 
        /// DownloadedCfgFiles: İndirilen CFG dosyalarının listesini içerir.       
        /// DownloadedDatFiles: İndirilen DAT dosyalarının listesini içerir.       
        /// RelayInformation: RelayInformation adlı başka bir sınıfın özelliklerini içeren bir nesnedir.Bu özellik, röle bilgilerini temsil eder.      
        /// Host: FTP sunucusunun adresini içerir.     
        /// ComtradeFilesPath: Comtrade verilerinin kaydedildiği klasörün yolu.      
        /// DestFolderPath: Verilerin indirildiği hedef klasörün yolu.   
        /// DownloadedFiles: Zaten indirilmiş dosyaların listesini içerir.
        /// </param>
        /// <returns> Bu tür, bir liste içinde string dizilerini (string[]) temsil eder. İndirilecek .cfg dosyalarını ve .dat dosyalarını içerir.</returns>
        private async Task<List<string>> GetFilesToDownloadFromFtp(ParametersModel parameters)
        {
            List<string> filesToDownload = new List<string>();
            int threadId = Thread.CurrentThread.ManagedThreadId;
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(parameters.Host + parameters.RelayInformation.Path);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.UsePassive = true;
                request.Credentials = new NetworkCredential(parameters.RelayInformation.User, Encryption.Decrypt(parameters.RelayInformation.Password));

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);

                string fileName = reader.ReadLine();
                while (!string.IsNullOrEmpty(fileName))
                {
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
                await log.ErrorLog($"Röleye bağlanırken hata oluştu! \r Tm_kV_Hücre : {parameters.RelayInformation.TmKvHucre} \r IP : {parameters.RelayInformation.IP} \r Röle Model : {parameters.RelayInformation.RoleModel}", ex.ToString(), "FTP", threadId);
                Serilog.Log.Error(ex, $"Aşağıda bilgileri bulunan röleye bağlanırken hata oluştu! \r Tm_kV_Hücre : {parameters.RelayInformation.TmKvHucre} \r IP : {parameters.RelayInformation.IP} \r Röle Model : {parameters.RelayInformation.RoleModel}");
            }

            return filesToDownload;
        }

        /// <summary>
        /// FTP sunucusundan belirli dosyaları WebClient kullanarak indirmek için kullanılır. Bu metot, verilen dosya listesini alır ve her dosyayı indirirken hataları kontrol eder.
        /// </summary>
        /// <param name="parameters">
        /// Kullanılan parametreleri ve verileri gruplamak ve iletmek için tasarlanmıştır.
        /// FilesToDownload: İndirilecek dosyaların listesini içerir. 
        /// DownloadedCfgFiles: İndirilen CFG dosyalarının listesini içerir.       
        /// DownloadedDatFiles: İndirilen DAT dosyalarının listesini içerir.       
        /// RelayInformation: RelayInformation adlı başka bir sınıfın özelliklerini içeren bir nesnedir.Bu özellik, röle bilgilerini temsil eder.      
        /// Host: FTP sunucusunun adresini içerir.     
        /// ComtradeFilesPath: Comtrade verilerinin kaydedildiği klasörün yolu.      
        /// DestFolderPath: Verilerin indirildiği hedef klasörün yolu.   
        /// DownloadedFiles: Zaten indirilmiş dosyaların listesini içerir.
        /// </param>
        /// <returns>Bu tür, bir liste içinde string dizilerini (string[]) temsil eder. İndirilen .cfg dosyalarını ve .dat dosyalarını içerir.</returns>
        private async Task<List<string>[]> DownloadFilesFromFtp(ParametersModel parameters)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            List<string> downloadedCfgFiles = new List<string>();
            List<string> downloadedDatFiles = new List<string>();
            foreach (string file in parameters.FilesToDownload)
            {
                try
                {
                    WebClient client = new WebClient();
                    client.Credentials = new NetworkCredential(parameters.RelayInformation.User, Encryption.Decrypt(parameters.RelayInformation.Password));
                    string remoteFilePath = parameters.Host + parameters.RelayInformation.Path + file;
                    string localFolderPath = Path.Combine(parameters.DestFolderPath, file);
                    if (!string.IsNullOrEmpty(file))
                    {
                        client.DownloadFile(remoteFilePath, localFolderPath);
                        await log.InformationLog($"İndirilen dosya : {file}", "FTP", threadId);
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
                    await log.ErrorLog($" Röleden {file} isimli dosyayı indirirken hata oluştu! \r Tm_kV_Hücre : {parameters.RelayInformation.TmKvHucre} \r IP : {parameters.RelayInformation.IP} \r Röle Model : {parameters.RelayInformation.RoleModel} \r Dosya Adı: {file} \r ", ex.ToString(), "FTP", threadId);
                    Serilog.Log.Error(ex, $" Röle bilgisi aşağıdaki gibi olan röleden {file} isimli dosyayı indirirken hata oluştu! \r Tm_kV_Hücre : {parameters.RelayInformation.TmKvHucre} \r IP : {parameters.RelayInformation.IP} \r Röle Model : {parameters.RelayInformation.RoleModel} \r Dosya Adı: {file} \r ");
                }
            }

            return new List<string>[] { downloadedCfgFiles, downloadedDatFiles };
        }

        /// <summary>
        /// İndirilen CFG ve DAT dosyaları arasında eksik dosyaları kontrol etmek için kullanılır. Eksik dosyaları tespit eder ve gerekirse bir hata kaydeder.
        /// </summary>
        /// <param name="parameters">
        /// Kullanılan parametreleri ve verileri gruplamak ve iletmek için tasarlanmıştır.
        /// FilesToDownload: İndirilecek dosyaların listesini içerir. 
        /// DownloadedCfgFiles: İndirilen CFG dosyalarının listesini içerir.       
        /// DownloadedDatFiles: İndirilen DAT dosyalarının listesini içerir.       
        /// RelayInformation: RelayInformation adlı başka bir sınıfın özelliklerini içeren bir nesnedir.Bu özellik, röle bilgilerini temsil eder.      
        /// Host: FTP sunucusunun adresini içerir.     
        /// ComtradeFilesPath: Comtrade verilerinin kaydedildiği klasörün yolu.      
        /// DestFolderPath: Verilerin indirildiği hedef klasörün yolu.   
        /// DownloadedFiles: Zaten indirilmiş dosyaların listesini içerir.
        /// </param>
        /// <returns>Bu metot, async ve asenkron bir işlem gerçekleştirir, ancak herhangi bir değer döndürmez. Bu tür metotlar, işlemleri başlatmak, bitirmek veya asenkron olarak yürütmek için kullanılır, ancak bir sonuç döndürmezler.</returns>
        private async Task CheckMissingFiles(ParametersModel parameters)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            try
            {
                foreach (string cfgFile in parameters.DownloadedCfgFiles)
                {
                    string datFileName = Path.ChangeExtension(cfgFile, ".dat");

                    if (!parameters.DownloadedDatFiles.Contains(datFileName, StringComparer.OrdinalIgnoreCase))
                    {
                        await log.WarningLog($"{cfgFile} dosyasına karşılık gelen DAT dosyası eksik: {datFileName}", "Arıza Kaydı", threadId);
                        Serilog.Log.Warning($"{cfgFile} dosyasına karşılık gelen DAT dosyası eksik: {datFileName}");
                    }
                }
                foreach (string datFile in parameters.DownloadedDatFiles)
                {
                    string cfgFileName = Path.ChangeExtension(datFile, ".cfg");

                    if (!parameters.DownloadedCfgFiles.Contains(cfgFileName, StringComparer.OrdinalIgnoreCase))
                    {
                        await log.WarningLog($"{datFile} dosyasına karşılık gelen DAT dosyası eksik: {cfgFileName}", "Arıza Kaydı", threadId);
                        Serilog.Log.Warning($"{datFile} dosyasına karşılık gelen CFG dosyası eksik: {cfgFileName}");
                    }
                }
            }
            catch (Exception ex)
            {
                await log.ErrorLog($"Eksik dosyalar kontrol edilirken hata oluştu! \r Dizin: {parameters.DestFolderPath}", ex.ToString(), "Arıza Kaydı", threadId);
                Serilog.Log.Error(ex, $"Eksik dosyalar kontrol edilirken hata oluştu! \r Dizin: {parameters.DestFolderPath}");
            }
        }

        /// <summary>
        ///  İndirilen CFG ve DAT dosyaları üzerinden arıza kayıtları oluşturur. Her bir CFG dosyasının karşılık gelen DAT dosyası olup olmadığını kontrol eder ve arıza kayıtlarını oluşturur.
        /// </summary>
        /// <param name="parameters">
        /// Kullanılan parametreleri ve verileri gruplamak ve iletmek için tasarlanmıştır.
        /// FilesToDownload: İndirilecek dosyaların listesini içerir. 
        /// DownloadedCfgFiles: İndirilen CFG dosyalarının listesini içerir.       
        /// DownloadedDatFiles: İndirilen DAT dosyalarının listesini içerir.       
        /// RelayInformation: RelayInformation adlı başka bir sınıfın özelliklerini içeren bir nesnedir.Bu özellik, röle bilgilerini temsil eder.      
        /// Host: FTP sunucusunun adresini içerir.     
        /// ComtradeFilesPath: Comtrade verilerinin kaydedildiği klasörün yolu.      
        /// DestFolderPath: Verilerin indirildiği hedef klasörün yolu.   
        /// DownloadedFiles: Zaten indirilmiş dosyaların listesini içerir.
        /// </param>
        /// <returns>Bu metot, async ve asenkron bir işlem gerçekleştirir, ancak herhangi bir değer döndürmez. Bu tür metotlar, işlemleri başlatmak, bitirmek veya asenkron olarak yürütmek için kullanılır, ancak bir sonuç döndürmezler.</returns>
        private async Task CreateDisturbancesAsync(ParametersModel parameters)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            try
            {
                foreach (string cfgFile in parameters.DownloadedCfgFiles)
                {
                    try
                    {
                        string datFileName = Path.ChangeExtension(cfgFile, ".dat");
                        string cfgFilePath = Path.Combine(parameters.DestFolderPath, cfgFile);
                        string datFilePath = Path.Combine(parameters.DestFolderPath, datFileName);

                        if (File.Exists(Path.Combine(parameters.DestFolderPath, datFileName)))
                        {
                            string cfgFileData = File.ReadAllText(cfgFilePath);
                            byte[] datFileData = File.ReadAllBytes(datFilePath);

                            string[] lines = cfgFileData.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                            DateTime faultTimeEnd = DateTime.ParseExact(lines[lines.Length - 3], "d/M/yyyy,H:m:s.ffffff", CultureInfo.InvariantCulture, DateTimeStyles.None);
                            DateTime faultTimeStart = DateTime.ParseExact(lines[lines.Length - 4], "d/M/yyyy,H:m:s.ffffff", CultureInfo.InvariantCulture, DateTimeStyles.None);
                            TimeSpan totalTime = faultTimeEnd - faultTimeStart;
                            double totalFaultTime = totalTime.TotalSeconds;

                            Disturbance disturbance = new Disturbance();
                            disturbance.IP = parameters.RelayInformation.IP;
                            disturbance.TmNo = parameters.RelayInformation.TmNo;
                            disturbance.CfgFilePath = cfgFilePath;
                            disturbance.FaultTimeStart = faultTimeStart;
                            disturbance.FaultTimeEnd = faultTimeEnd;
                            disturbance.TotalFaultTime = totalFaultTime;
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
                                await log.InformationLog($"Arıza kaydı oluşturuldu. \r Tm_kV_Hücre : {parameters.RelayInformation.TmKvHucre} \r IP : {parameters.RelayInformation.IP} \r Röle Model : {parameters.RelayInformation.RoleModel} \r CFG dosyası : {cfgFile} \r DAT dosyası : {datFileName}", "Arıza Kaydı", threadId);
                                Serilog.Log.Information($"Arıza kaydı oluşturuldu. \r Tm_kV_Hücre : {parameters.RelayInformation.TmKvHucre} \r IP : {parameters.RelayInformation.IP} \r Röle Model : {parameters.RelayInformation.RoleModel} \r CFG dosyası : {cfgFile} \r DAT dosyası : {datFileName}");
                            }
                            else
                            {
                                await log.ErrorLog($"Arıza kaydı oluşturulurken hata oluştu! \r Tm_kV_Hücre : {parameters.RelayInformation.TmKvHucre} \r IP : {parameters.RelayInformation.IP} \r Röle Model : {parameters.RelayInformation.RoleModel} \r CFG dosyası : {cfgFile} \r DAT dosyası : {datFileName}", "Veritabanına kaydederken hata oluştu.", "Arıza Kaydı", threadId);
                                Serilog.Log.Error($"Arıza kaydı oluşturulurken hata oluştu! \r Tm_kV_Hücre : {parameters.RelayInformation.TmKvHucre} \r IP : {parameters.RelayInformation.IP} \r Röle Model : {parameters.RelayInformation.RoleModel} \r CFG dosyası : {cfgFile} \r DAT dosyası : {datFileName}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await log.ErrorLog($"Arıza kaydı oluşturulurken hata oluştu! \r Tm_kV_Hücre : {parameters.RelayInformation.TmKvHucre} \r IP : {parameters.RelayInformation.IP} \r Röle Model : {parameters.RelayInformation.RoleModel} \r Comtrade Dosya Adı : {Path.ChangeExtension(cfgFile, ".dat")}", ex.ToString(), "Arıza Kaydı", threadId);
                        Serilog.Log.Error(ex, $"Arıza kaydı oluşturulurken hata oluştu! \r Tm_kV_Hücre : {parameters.RelayInformation.TmKvHucre} \r IP : {parameters.RelayInformation.IP} \r Röle Model : {parameters.RelayInformation.RoleModel} \r Comtrade Dosya Adı : {Path.ChangeExtension(cfgFile, ".dat")}", ex.ToString(), "Arıza Kaydı");
                    }
                }
            }
            catch (Exception ex)
            {
                await log.ErrorLog($"Arıza kayıtları oluşturulamadı! \r Tm_kV_Hücre : {parameters.RelayInformation.TmKvHucre} \r IP : {parameters.RelayInformation.IP} \r Röle Model : {parameters.RelayInformation.RoleModel}", ex.ToString(), "Arıza Kaydı", threadId);
                Serilog.Log.Error(ex, $"Arıza kayıtları oluşturulamadı! \r Tm_kV_Hücre : {parameters.RelayInformation.TmKvHucre} \r IP : {parameters.RelayInformation.IP} \r Röle Model : {parameters.RelayInformation.RoleModel}");
            }
        }

    }
}
