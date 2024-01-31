using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComtradeApp.AppDbContext
{
    /// <summary>
    /// DbContextFactory sınıfı, Entity Framework ile bir veritabanı bağlantısını temsil eden bir AppDbContext örneği oluşturmak için kullanılan bir yardımcı sınıftır. Bu sınıf, bağımlılık enjeksiyonu (dependency injection) kullanarak IServiceScopeFactory aracılığıyla bir AppDbContext örneği oluşturur. 
    /// Her istemcinin kendi bağlamına ihtiyacı olduğunda veya işlem başına yeni bir bağlam oluşturulması gerektiğinde kullanışlıdır. Bağlamın ömrünü yönetmek ve işlemler arasında veritabanı bağlantısını izole etmek için bu tür bir yapıyı kullanmak, performans ve güvenlik açısından önemlidir.
    /// </summary>
    public class DbContextFactory
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public DbContextFactory(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public AppDbContext CreateDbContext()
        {
            var scope = serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return dbContext;
        }
    }
}
