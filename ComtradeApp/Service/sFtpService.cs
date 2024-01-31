using ComtradeApp.Models;
using ComtradeApp.Repository;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace ComtradeApp.Service
{
    public class sFtpService
    {

        private readonly DisturbanceRepository disturbanceRepository;
        private readonly LogRepository log;

        public sFtpService(DisturbanceRepository disturbanceRepository, LogRepository log)
        {
            this.disturbanceRepository = disturbanceRepository;
            this.log = log;
        }
        /// <summary>
        /// WinSCP kütüphanesini kullanarak SFTP (SSH File Transfer Protocol) ile dosya gönderme işlemlerini gerçekleştiren bir hizmeti uygular.
        /// </summary>
        /// <param name="host">SFTP sunucusunun adresi.</param>
        /// <param name="username">SFTP sunucusuna bağlanmak için kullanılacak kullanıcı adı.</param>
        /// <param name="password">SFTP sunucusuna bağlanmak için kullanılacak şifre.</param>
        /// <param name="directory"> Dosyaların gönderileceği uzak dizin.</param>
        /// <param name="port"> SFTP sunucusunun bağlantı noktası.</param>
        /// <param name="sshHostKeyFingerprint">SSH anahtar parmak izi.</param>
        /// <returns>Bu metot, async ve asenkron bir işlem gerçekleştirir, ancak herhangi bir değer döndürmez. Bu tür metotlar, işlemleri başlatmak, bitirmek veya asenkron olarak yürütmek için kullanılır, ancak bir sonuç döndürmezler.</returns>
        public async Task PutCsvFiles(string host, string username, string password, string directory, int port, string sshHostKeyFingerprint)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            try
            {
                await log.InformationLog("sFTP ile dosya gönderme işlemleri başladı.", "SFTP", threadId);
                Serilog.Log.Information("sFTP ile dosya gönderme işlemleri başladı..");

                var disturbances = disturbanceRepository.GetBySftpStatus(false);

                if (disturbances.Count == 0)
                {
                    await log.InformationLog("Henüz gönderilecek dosya yok", "SFTP", threadId);
                    Serilog.Log.Information("Henüz gönderilecek dosya yok");
                }

                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = host,
                    UserName = username,
                    Password = password,
                    SshHostKeyFingerprint = sshHostKeyFingerprint,
                };

                using (Session session = new Session())
                {
                    session.Open(sessionOptions);

                    foreach (var item in disturbances)
                    {

                        if (!string.IsNullOrEmpty(item.RmsDataPath) && !string.IsNullOrEmpty(item.InstantDataPath))
                        {

                            string destFolderPath = $"{item.IP}-{item.TmKvHucre}";
                            string rmsName = Path.GetFileName(item.RmsDataPath);
                            string instantName = Path.GetFileName(item.InstantDataPath);
                            string remoteFilePathRms = $"csv-files/{destFolderPath}/{rmsName}";
                            string remoteFilePathInstant = $"csv-files/{destFolderPath}/{instantName}";

                            if (!session.FileExists($"csv-files/{destFolderPath}"))
                            {
                                session.CreateDirectory($"csv-files/{destFolderPath}");
                            }
                            // Rms dosyasını gönderme
                            TransferOptions rmsTransferOptions = new TransferOptions();
                            TransferOperationResult rmsTransferResult = session.PutFiles(item.RmsDataPath, remoteFilePathRms, false, rmsTransferOptions);

                            if (rmsTransferResult.IsSuccess)
                            {
                                await log.InformationLog($"RMS dosyası {item.RmsDataPath} başarıyla gönderildi.", "SFTP", threadId);
                                Serilog.Log.Information($"RMS dosyası {item.RmsDataPath} başarıyla gönderildi.");
                            }
                            else
                            {
                                await log.ErrorLog($"RMS dosyası {item.RmsDataPath} gönderimi başarısız oldu.", rmsTransferResult.Failures[0].Message, "SFTP", threadId);
                                Serilog.Log.Error($"RMS dosyası {item.RmsDataPath} gönderimi başarısız oldu: {rmsTransferResult.Failures[0].Message}");
                            }

                            // Instant dosyasını gönderme
                            TransferOptions instantTransferOptions = new TransferOptions();
                            TransferOperationResult instantTransferResult = session.PutFiles(item.InstantDataPath, remoteFilePathInstant, false, instantTransferOptions);

                            if (instantTransferResult.IsSuccess)
                            {
                                await log.InformationLog($"Instant dosyası {item.InstantDataPath} başarıyla gönderildi.", "SFTP", threadId);
                                Serilog.Log.Information($"Instant dosyası {item.InstantDataPath} başarıyla gönderildi.");
                            }
                            else
                            {
                                await log.ErrorLog($"Instant dosyası {item.InstantDataPath} gönderimi başarısız oldu.", instantTransferResult.Failures[0].Message, "SFTP", threadId);
                                Serilog.Log.Error($"Instant dosyası {item.InstantDataPath} gönderimi başarısız oldu: {instantTransferResult.Failures[0].Message}");
                            }

                            if (rmsTransferResult.IsSuccess && instantTransferResult.IsSuccess)
                            {
                                // İki dosya da başarıyla gönderildiyse item'ın durumunu güncelle.
                                item.sFtpStatus = true;
                                item.PutTime = DateTime.Now;
                                bool result = disturbanceRepository.Update(item);
                                if (!result)
                                {
                                    await log.ErrorLog("sFtpStatus durumu güncellenirken hata oluştu.", "Database güncelleme hatası", "SFTP", threadId);
                                    Serilog.Log.Error("sFtpStatus durumu güncellenirken hata oluştu.Database güncelleme hatası");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await log.ErrorLog("SFTP işlemleri başlamadı.", ex.ToString(), "SFTP", threadId);
                Serilog.Log.Error($"Hata oluştu: {ex.Message}");
            }
            finally
            {
                await log.InformationLog("sFTP ile dosya gönderme işlemleri sonlandı.", "SFTP", threadId);
                Serilog.Log.Information("sFTP ile dosya gönderme işlemleri sonlandı.. \n");
            }

        }
    }
}
