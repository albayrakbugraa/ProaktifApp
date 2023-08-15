using ComtradeApp.AppDbContext;
using ComtradeApp.Repository;
using ComtradeApp.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz.Impl;
using Quartz;
using Serilog;
using static Quartz.Logging.OperationName;
using ComtradeApp.Job;
using Quartz.Spi;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System;

var host = Host.CreateDefaultBuilder(args)

    .ConfigureServices((hostContext, services) =>
    {
        Serilog.Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(hostContext.Configuration)
                    .CreateLogger();


        JobDataMap jobDataMap = new JobDataMap
                {
                    { "PythonExePath", hostContext.Configuration.GetSection("FilePath")["PythonExePath"] },
                    { "RmsScript", hostContext.Configuration.GetSection("FilePath")["RmsScript"] },
                    { "InstantScript", hostContext.Configuration.GetSection("FilePath")["InstantScript"] },
                    { "ComtradeFilesPath", hostContext.Configuration.GetSection("FilePath")["ComtradeFilesPath"] },
                    { "CsvFilesPath", hostContext.Configuration.GetSection("FilePath")["CsvFilesPath"] },
                    { "HostName", hostContext.Configuration.GetSection("sFtpSettings")["HostName"] },
                    { "Username", hostContext.Configuration.GetSection("sFtpSettings")["Username"] },
                    { "Password", hostContext.Configuration.GetSection("sFtpSettings")["Password"] },
                    { "HomeDirectory", hostContext.Configuration.GetSection("sFtpSettings")["HomeDirectory"] },
                    { "Port", hostContext.Configuration.GetSection("sFtpSettings").GetValue<int>("Port") },
                    { "SshHostKeyFingerprint", hostContext.Configuration.GetSection("sFtpSettings")["SshHostKeyFingerprint"] }
                };

        services.AddDbContext<AppDbContext>(options => options.UseOracle(hostContext.Configuration.GetConnectionString("DefaultConnection")));
        services.AddSingleton<MyDataRepository>();
        services.AddSingleton<DisturbanceRepository>();
        services.AddSingleton<HistoryOfChangeRepository>();
        services.AddSingleton<FtpService>();
        services.AddSingleton<CsvConverterService>();
        services.AddSingleton<sFtpService>();
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();

            var jobKey = new JobKey("MyJob");

            q.AddJob<MyJob>(opts => opts.WithIdentity(jobKey).SetJobData(jobDataMap));

            q.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity($"{jobKey}-trigger")
            .StartNow()
            .WithSimpleSchedule(x => x.WithIntervalInSeconds(10)
            .RepeatForever()));
        });
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        services.AddTransient<MyJob>();
    }).Build();




host.Run();

//public class Program
//{




//using var scope = host.Services.CreateScope();
//MyDataRepository myDataRepository = scope.ServiceProvider.GetRequiredService<MyDataRepository>();
//DisturbanceRepository disturbanceRepository = scope.ServiceProvider.GetRequiredService<DisturbanceRepository>();
//HistoryOfChangeRepository historyOfChangeRepository = scope.ServiceProvider.GetRequiredService<HistoryOfChangeRepository>();
//FtpService ftpService = scope.ServiceProvider.GetRequiredService<FtpService>();
//CsvConverterService csvConverterService = scope.ServiceProvider.GetRequiredService<CsvConverterService>();
//sFtpService sFtpService = scope.ServiceProvider.GetRequiredService<sFtpService>();
//    private static IServiceProvider serviceProvider;
//    public static async Task Main(string[] args)
//    {
//        #region Configuration

//        IConfiguration configuration = new ConfigurationBuilder()
//        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//        .Build();

//        Serilog.Log.Logger = new LoggerConfiguration()
//                    .ReadFrom.Configuration(configuration)
//                    .CreateLogger();

//        JobDataMap jobDataMap = new JobDataMap
//        {
//            { "PythonExePath", configuration.GetSection("FilePath")["PythonExePath"] },
//            { "RmsScript", configuration.GetSection("FilePath")["RmsScript"] },
//            { "InstantScript", configuration.GetSection("FilePath")["InstantScript"] },
//            { "ComtradeFilesPath", configuration.GetSection("FilePath")["ComtradeFilesPath"] },
//            { "CsvFilesPath", configuration.GetSection("FilePath")["CsvFilesPath"] },
//            { "HostName", configuration.GetSection("sFtpSettings")["HostName"] },
//            { "Username", configuration.GetSection("sFtpSettings")["Username"] },
//            { "Password", configuration.GetSection("sFtpSettings")["Password"] },
//            { "HomeDirectory", configuration.GetSection("sFtpSettings")["HomeDirectory"] },
//            { "Port", configuration.GetSection("sFtpSettings").GetValue<int>("Port") },
//            { "SshHostKeyFingerprint", configuration.GetSection("sFtpSettings")["SshHostKeyFingerprint"] }
//        };

//        // Yaşam döngüsünü burada yapılandırabilirsiniz
//        IServiceCollection services = new ServiceCollection();
//        services.AddDbContext<AppDbContext>(options => options.UseOracle(configuration.GetConnectionString("DefaultConnection")));
//        services.AddSingleton<MyDataRepository>();
//        services.AddSingleton<DisturbanceRepository>();
//        services.AddSingleton<HistoryOfChangeRepository>();
//        services.AddSingleton<FtpService>();
//        services.AddSingleton<CsvConverterService>();
//        services.AddSingleton<sFtpService>();
//        services.AddQuartz(options =>
//        {
//            options.UseMicrosoftDependencyInjectionJobFactory();

//            var jobKey = new JobKey("Deneme");

//            options.AddJob<MyJob>(options => options
//                .WithIdentity(jobKey)
//             );

//            options.AddTrigger(triggerOptions => triggerOptions
//                .ForJob(jobKey)
//                .WithIdentity("MyJob-trigger")
//                .WithSimpleSchedule(scheduleBuilder =>
//                    scheduleBuilder.WithIntervalInSeconds(1)  // Her saniye
//                           .RepeatForever())          // Sonsuz tekrar
//                .WithDescription("MyJob Trigger")
//             );
//        });
//        services.AddTransient<MyJob>();
//        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

//        serviceProvider = services.BuildServiceProvider();
//        MyDataRepository myDataRepository = serviceProvider.GetRequiredService<MyDataRepository>();
//        DisturbanceRepository disturbanceRepository = serviceProvider.GetRequiredService<DisturbanceRepository>();
//        HistoryOfChangeRepository historyOfChangeRepository = serviceProvider.GetRequiredService<HistoryOfChangeRepository>();
//        FtpService ftpService = serviceProvider.GetRequiredService<FtpService>();
//        CsvConverterService csvConverterService = serviceProvider.GetRequiredService<CsvConverterService>();
//        sFtpService sFtpService = serviceProvider.GetRequiredService<sFtpService>();

//        #endregion

//        try
//        {
//            Serilog.Log.Debug("Uygulama başladı... \n");

//            StdSchedulerFactory factory = new StdSchedulerFactory();
//            IScheduler scheduler = await factory.GetScheduler();

//            // and start it off
//            await scheduler.Start();

//            // define the job and tie it to our HelloJob class
//            IJobDetail job = JobBuilder.Create<MyJob>()
//                .WithIdentity("job1", "group1")
//                .Build();

//            // Trigger the job to run now, and then repeat every 10 seconds
//            ITrigger trigger = TriggerBuilder.Create()
//                .WithIdentity("trigger1", "group1")
//                .StartNow()
//                .WithSimpleSchedule(x => x
//                    .WithIntervalInSeconds(60)
//                    .RepeatForever())
//                .Build();

//            // Tell Quartz to schedule the job using our trigger
//            await scheduler.ScheduleJob(job, trigger);


//            Console.WriteLine("Press any key to close the application");
//            Console.ReadKey();


//            Console.ReadLine();

//        }
//        catch (Exception ex)
//        {
//            Serilog.Log.Error(ex.ToString());
//        }
//        finally
//        {
//            Serilog.Log.Debug("Uygulama kapandı... \n");
//            Serilog.Log.CloseAndFlush();
//        }
//    }



//}
