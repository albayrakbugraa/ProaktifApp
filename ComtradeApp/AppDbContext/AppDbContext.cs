using ComtradeApp.Models;
using Microsoft.EntityFrameworkCore;
using ProaktifArizaTahmini.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ComtradeApp.AppDbContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<RelayInformation> RelayInformations { get; set; }
        public DbSet<Disturbance> Disturbances { get; set; }
        public DbSet<HistoryOfChange> HistoryOfChanges { get; set; }
        public DbSet<ServiceLog> ServiceLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RelayInformation>().HasIndex(md => new { md.TmNo, md.kV, md.HucreNo }).IsUnique();
            base.OnModelCreating(modelBuilder);
        }
    }
}
