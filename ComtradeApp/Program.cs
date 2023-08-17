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
using ComtradeApp.Jobs;
using Serilog.Core;
using System.Diagnostics;

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
        services.AddTransient<RelayInformationRepository>();
        services.AddTransient<DisturbanceRepository>();
        services.AddTransient<HistoryOfChangeRepository>();
        services.AddTransient<LogRepository>();
        services.AddTransient<FtpService>();
        services.AddTransient<CsvConverterService>();
        services.AddTransient<sFtpService>();
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();
            
            //Ftp job
            var ftpJobKey = new JobKey("Ftp Dosya Çekme");
            q.AddJob<FtpAndCsvJob>(opts => opts.WithIdentity(ftpJobKey).SetJobData(jobDataMap));
            
            // Sabah 8:00 için trigger oluşturma
            q.AddTrigger(options =>
            {
                options.ForJob(ftpJobKey)
                    .WithIdentity($"{ftpJobKey}-Trigger")
                    .WithCronSchedule(hostContext.Configuration.GetSection("QuartzSettings")["MorningCronSchedule"]);
            });

            // Akşam 8:00 için trigger oluşturma
            q.AddTrigger(options =>
            {
                options.ForJob(ftpJobKey)
                    .WithIdentity($"{ftpJobKey}-Trigger")
                    .WithCronSchedule(hostContext.Configuration.GetSection("QuartzSettings")["EveningCronSchedule"]);
            });


            //sFtp job
            var sFtpJobKey = new JobKey("sFtp Dosya Gönderme");
            q.AddJob<sFtpJob>(opts => opts.WithIdentity(sFtpJobKey).SetJobData(jobDataMap));

            // Akşam 8,30 için trigger oluşturma
            q.AddTrigger(options =>
            {
                options.ForJob(sFtpJobKey)
                    .WithIdentity($"{sFtpJobKey}-Trigger")
                    .WithCronSchedule(hostContext.Configuration.GetSection("QuartzSettings")["sFtpCronSchedule"]);
            });
        });
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        services.AddTransient<FtpAndCsvJob>();
        services.AddTransient<sFtpJob>();
    }).Build();

host.Run();

