using Microsoft.EntityFrameworkCore;
using ProaktifArizaTahmini.CORE.Entities;
using ProaktifArizaTahmini.CORE.Enums;
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
        public DbSet<HistoryOfChange> HistoryOfChanges { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserLog> UserLogs { get; set; }
        public DbSet<UserType> UserTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MyData>().HasIndex(md => new { md.TmNo, md.kV, md.HucreNo }).IsUnique();
            modelBuilder.Entity<Disturbance>().HasOne<MyData>(d => d.MyData).WithMany(m => m.Disturbances).HasForeignKey(d => d.MyDataId);
            modelBuilder.Entity<HistoryOfChange>().HasOne<MyData>(d => d.MyData).WithMany(m => m.HistoryOfChanges).HasForeignKey(d => d.MyDataId);
            modelBuilder.Entity<MyData>().Property(m => m.ID).ValueGeneratedOnAdd();
            modelBuilder.Entity<UserLog>().HasOne<User>(u => u.User).WithMany(u => u.UserLogs).HasForeignKey(u => u.UserId);
            modelBuilder.Entity<User>().HasOne<UserType>(u => u.UserType).WithMany(u => u.Users).HasForeignKey(u => u.UserTypeId);

            modelBuilder.Entity<UserType>().HasData(
                new UserType()
                {
                    ID = 1,
                    UserTypeName = UserTypeNames.Domain,
                    Description = "Domain"
                },
                new UserType()
                {
                    ID = 2,
                    UserTypeName = UserTypeNames.Misafir,
                    Description = "Misafir"
                },
                new UserType()
                {
                    ID = 3,
                    UserTypeName = UserTypeNames.Test,
                    Description = "Test"
                },
                new UserType()
                {
                    ID = 4,
                    UserTypeName = UserTypeNames.KurumDisi,
                    Description = "Kurum Dışı"
                },
                new UserType()
                {
                    ID = 5,
                    UserTypeName = UserTypeNames.KurumIciDomainsiz,
                    Description = "Kurum İçi Domainsiz"
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}
