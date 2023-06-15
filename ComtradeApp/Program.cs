using ComtradeApp;
using ComtradeApp.AppDbContext;
using ComtradeApp.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;

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
        services.AddSingleton<FtpService>();
        services.AddSingleton<FtpDeneme3>();
        serviceProvider = services.BuildServiceProvider();
        MyDataRepository myDataRepository = serviceProvider.GetRequiredService<MyDataRepository>();
        DisturbanceRepository disturbanceRepository = serviceProvider.GetRequiredService<DisturbanceRepository>();
        FtpService ftpService = serviceProvider.GetRequiredService<FtpService>();
        FtpDeneme3 ftpService3 = serviceProvider.GetRequiredService<FtpDeneme3>();
        #endregion

        #region Paths
        string localFolder = configuration.GetSection("FilePath")["LocalFolder"];
        string comtradeFilesPath = configuration.GetSection("FilePath")["ComtradeFilesPath"];
        string localCfgFolder = configuration.GetSection("FilePath")["LocalCfgFolder"];
        string localDatFolder = configuration.GetSection("FilePath")["LocalDatFolder"];
        string localRmsCsvFolder = configuration.GetSection("FilePath")["LocalRmsCsvFolder"];
        string localInstantCsvFolder = configuration.GetSection("FilePath")["LocalInstantCsvFolder"];
        string pythonExePath = configuration.GetSection("FilePath")["PythonExePath"];
        string rmsScript = configuration.GetSection("FilePath")["RmsScript"];
        string instantScript = configuration.GetSection("FilePath")["InstantScript"];
        string ftpCsvPath = configuration.GetSection("FilePath")["FtpCsvPath"]; 
        #endregion

        try
        {
            Serilog.Log.Debug("Uygulama başladı... \n");
            //await ftpService.DownloadCfgAndDatFilesEfCoreAsync(comtradeFilesPath);
            await ftpService3.DownloadCfgAndDatFilesEfCoreAsync(comtradeFilesPath);
            CsvConverterService.ConvertDatAndCfgFilesToCsvAsRMSData(pythonExePath, rmsScript, comtradeFilesPath, localRmsCsvFolder);
            CsvConverterService.ConvertDatAndCfgFilesToCsvAsInstantData(pythonExePath, instantScript, comtradeFilesPath, localInstantCsvFolder);
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
 