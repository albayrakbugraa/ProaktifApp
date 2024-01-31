using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.CORE.Entities
{
    public class UserLog
    {
        [Column("Id")]
        public int ID { get; set; }
        [Column("Metot_Ismi")]
        public string MethodName { get; set; }
        [Column("Aciklama")]
        public string? Message { get; set; }
        [Column("Hata")]
        public string? Exception { get; set; }
        [Column("Log_Seviyesi")]
        public string LogLevel { get; set; }

        [Column("Tarih")]
        public DateTime LogDate { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
    }
}
