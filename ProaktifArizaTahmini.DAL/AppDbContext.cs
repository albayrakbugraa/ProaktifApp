using Microsoft.EntityFrameworkCore;
using ProaktifArizaTahmini.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
        {
            
        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseOracle("User Id=USER1;Password=1234;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=XEPDB1)))");
        }

        public DbSet<MyData> MyDatas { get; set; }
        public DbSet<Disturbance> Disturbances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MyData>().HasIndex(md => new { md.AvcilarTM, md.kV, md.HucreNo }).IsUnique();
            modelBuilder.Entity<Disturbance>().HasOne<MyData>(d => d.MyData).WithMany(m => m.Disturbances).HasForeignKey(d => d.MyDataId);
            modelBuilder.Entity<MyData>().Property(m => m.ID).ValueGeneratedOnAdd();
            base.OnModelCreating(modelBuilder);
        }
    }
}
