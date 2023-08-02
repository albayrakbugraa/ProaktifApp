using ComtradeApp.Models;
using Microsoft.EntityFrameworkCore;
using ProaktifArizaTahmini.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComtradeApp.Repository
{
    public class HistoryOfChangeRepository : BaseRepository<HistoryOfChange>
    {
        private readonly AppDbContext.AppDbContext db;
        public HistoryOfChangeRepository(AppDbContext.AppDbContext db) : base(db)
        {
            this.db = db;
        }
        public async Task<HistoryOfChange> GetByMyDataId (int id)
        {
            return await  db.HistoryOfChanges.Where(x => x.MyDataId == id).OrderByDescending(x => x.ChangedDate).FirstOrDefaultAsync();                        
        }
        
        public async Task UpdateFolderNames(MyData myData,string comtradeFilesPath)
        {
            var ipChanges = await db.HistoryOfChanges.OrderBy(x=>x.ChangedDate).ToListAsync();
            foreach (var item in ipChanges)
            {
                string oldIP = item.OldIP;
                string newIP = item.NewIP;
                string oldFolderPath = Path.Combine(comtradeFilesPath, $"{oldIP}-{myData.TmKvHucre}");
                string newFolderPath = Path.Combine(comtradeFilesPath, $"{newIP}-{myData.TmKvHucre}");
                if (Directory.Exists(oldFolderPath))
                {
                    Directory.Move(oldFolderPath, newFolderPath);
                }
            }
        }
        public async Task UpdateFolderNames(Disturbance disturbance, string csvFilesPath)
        {
            var ipChanges = await db.HistoryOfChanges.OrderBy(x => x.ChangedDate).ToListAsync();
            foreach (var item in ipChanges)
            {
                string oldIP = item.OldIP;
                string newIP = item.NewIP;
                string oldFolderPath = Path.Combine(csvFilesPath, $"{oldIP}-{disturbance.TmKvHucre}");
                string newFolderPath = Path.Combine(csvFilesPath, $"{newIP}-{disturbance.TmKvHucre}");
                if (Directory.Exists(oldFolderPath))
                {
                    Directory.Move(oldFolderPath, newFolderPath);
                }
            }
        }
    }
}
