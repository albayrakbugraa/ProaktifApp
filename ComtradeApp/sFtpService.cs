using ComtradeApp.Models;
using ComtradeApp.Repository;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace ComtradeApp
{
    public class sFtpService
    {

        private readonly DisturbanceRepository disturbanceRepository;

        public sFtpService(DisturbanceRepository disturbanceRepository)
        {
            this.disturbanceRepository = disturbanceRepository;
        }
        public async Task PutCsvFiles(string host, string username, string password, string directory, int port, string sshHostKeyFingerprint)
        {
            try
            {
                Serilog.Log.Information("sFTP ile dosya gönderme işlemleri başladı..");

                var disturbances = await disturbanceRepository.GetBySftpStatus(false);

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
                                Serilog.Log.Information($"RMS dosyası {item.RmsDataPath} başarıyla gönderildi.");
                            }
                            else
                            {
                                Serilog.Log.Error($"RMS dosyası {item.RmsDataPath} gönderimi başarısız oldu: {rmsTransferResult.Failures[0].Message}");
                            }

                            // Instant dosyasını gönderme
                            TransferOptions instantTransferOptions = new TransferOptions();
                            TransferOperationResult instantTransferResult = session.PutFiles(item.InstantDataPath, remoteFilePathInstant, false, instantTransferOptions);

                            if (instantTransferResult.IsSuccess)
                            {
                                Serilog.Log.Information($"Instant dosyası {item.InstantDataPath} başarıyla gönderildi.");
                            }
                            else
                            {
                                Serilog.Log.Error($"Instant dosyası {item.InstantDataPath} gönderimi başarısız oldu: {instantTransferResult.Failures[0].Message}");
                            }

                            if (rmsTransferResult.IsSuccess && instantTransferResult.IsSuccess)
                            {
                                // İki dosya da başarıyla gönderildiyse item'ın durumunu güncelle.
                                item.sFtpStatus = true;
                                item.PutTime = DateTime.Now;
                                bool result = disturbanceRepository.Update(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Error($"Hata oluştu: {ex.Message}");
            }
            finally
            {
                Serilog.Log.Information("sFTP ile dosya gönderme işlemleri sonlandı.. \n");
            }

        }
    }
}
