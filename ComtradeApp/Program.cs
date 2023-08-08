using ComtradeApp;
using ComtradeApp.AppDbContext;
using ComtradeApp.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

//Slash('\') is very important, pay attention to correct spelling of paths
//Please provide correct file paths.
//Don't forget to also change the file path in converter.py for the python module { sys.path.insert(0, 'C:\\Users\\Bugra Albayrak\\Desktop\\python_module') }
public class Program
{
    private static IServiceProvider serviceProvider;
    public static async Task Main(string[] args)
    {
        #region Configuration

        IConfiguration configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();
        Serilog.Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();

        // Yaşam döngüsünü burada yapılandırabilirsiniz
        IServiceCollection services = new ServiceCollection();
        services.AddDbContext<AppDbContext>(options => options.UseOracle(configuration.GetConnectionString("DefaultConnection")));
        services.AddSingleton<MyDataRepository>();
        services.AddSingleton<DisturbanceRepository>();
        services.AddSingleton<HistoryOfChangeRepository>();
        services.AddSingleton<FtpService>();
        services.AddSingleton<CsvConverterService>();
        services.AddSingleton<sFtpService>();

        serviceProvider = services.BuildServiceProvider();
        MyDataRepository myDataRepository = serviceProvider.GetRequiredService<MyDataRepository>();
        DisturbanceRepository disturbanceRepository = serviceProvider.GetRequiredService<DisturbanceRepository>();
        HistoryOfChangeRepository historyOfChangeRepository = serviceProvider.GetRequiredService<HistoryOfChangeRepository>();
        FtpService ftpService = serviceProvider.GetRequiredService<FtpService>();
        CsvConverterService csvConverterService = serviceProvider.GetRequiredService<CsvConverterService>();
        sFtpService sFtpService = serviceProvider.GetRequiredService<sFtpService>();
        #endregion

        #region Paths
        string comtradeFilesPath = configuration.GetSection("FilePath")["ComtradeFilesPath"];
        string csvFilesPath = configuration.GetSection("FilePath")["CsvFilesPath"];
        string pythonExePath = configuration.GetSection("FilePath")["PythonExePath"];
        string rmsScript = configuration.GetSection("FilePath")["RmsScript"];
        string instantScript = configuration.GetSection("FilePath")["InstantScript"];
        #endregion

        #region sFtpSettings
        string host = configuration.GetSection("sFtpSettings")["HostName"];
        string username = configuration.GetSection("sFtpSettings")["Username"];
        string password = configuration.GetSection("sFtpSettings")["Password"];
        string homeDirectory = configuration.GetSection("sFtpSettings")["HomeDirectory"];
        string sshHostKey = configuration.GetSection("sFtpSettings")["SshHostKeyFingerprint"];
        int port = configuration.GetSection("sFtpSettings").GetValue<int>("Port");
        #endregion

        try
        {
            Serilog.Log.Debug("Uygulama başladı... \n");
            await ftpService.DownloadCfgAndDatFilesEfCoreAsync(comtradeFilesPath);
            await csvConverterService.ConvertDatAndCfgFilesToCsvAsRMSDataAsync(pythonExePath, rmsScript, csvFilesPath);
            await csvConverterService.ConvertDatAndCfgFilesToCsvAsInstantData(pythonExePath, instantScript, csvFilesPath);
            await sFtpService.PutCsvFiles(host, username, password, homeDirectory, port, sshHostKey);
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex.ToString());
        }
        finally
        {
            Serilog.Log.Debug("Uygulama kapandı... \n");
            Serilog.Log.CloseAndFlush();
        }
    }
}
