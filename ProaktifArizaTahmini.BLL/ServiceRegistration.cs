using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProaktifArizaTahmini.BLL.Services;
using ProaktifArizaTahmini.CORE.IRepository;
using ProaktifArizaTahmini.DAL;
using ProaktifArizaTahmini.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.BLL
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                string connectionString = configuration.GetConnectionString("DefaultConnection");
                options.UseOracle(connectionString);
            });
            services.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddTransient<IDisturbanceRepository, DisturbanceRepository>();
            services.AddTransient<IMyDataRepository, MyDataRepository>();
            services.AddTransient<IHistoryOfChangeRepository, HistoryOfChangeRepository>();
            services.AddScoped<IMyDataService, MyDataService>();
            services.AddScoped<IDisturbanceService, DisturbanceService>();
            services.AddScoped<IHistoryOfChangeService, HistoryOfChangeService>();
        }
    }
}
